using System.Collections.Generic;
using DBDef;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using WorldCode;

namespace MOM
{
    public class FOW : MonoBehaviour
    {
        public const bool DEBUG_AI = false;

        private static FOW instance;

        private Material material;

        private MeshRenderer meshRenderer;

        private Texture2D dataTexture;

        private Texture2D minimapDataTexture;

        private Color[] arcanusData;

        private Color[] myrrorData;

        private bool arcanusOutdated;

        private bool myrrorOutdated;

        private void Start()
        {
            this.meshRenderer = base.GetComponent<MeshRenderer>();
            this.material = this.meshRenderer.material;
            FOW.instance = this;
            MHEventSystem.RegisterListener<World>(PlaneChanged, this);
            MHEventSystem.RegisterListener<Group>(GroupMoved, this);
        }

        public Color[] GetArcanusData()
        {
            return this.arcanusData;
        }

        public Color[] GetMyrrorData()
        {
            return this.myrrorData;
        }

        public Texture2D GetDatatexture()
        {
            return this.dataTexture;
        }

        public Texture2D GetMinimapDataTexture()
        {
            return this.minimapDataTexture;
        }

        public void SetArcanusData(float[] f)
        {
            Color[] array = new Color[f.Length / 4];
            for (int i = 0; i < f.Length; i += 4)
            {
                Color color = new Color(f[i], f[i + 1], f[i + 2], f[i + 3]);
                array[i / 4] = color;
            }
            this.arcanusData = array;
        }

        public void SetMyrrorData(float[] f)
        {
            Color[] array = new Color[f.Length / 4];
            for (int i = 0; i < f.Length; i += 4)
            {
                Color color = new Color(f[i], f[i + 1], f[i + 2], f[i + 3]);
                array[i / 4] = color;
            }
            this.myrrorData = array;
        }

        private void GroupMoved(object sender, object e)
        {
            if (sender is Group group && group.GetOwnerID() == PlayerWizard.HumanID() && (e is List<Vector3i> || !group.alive))
            {
                global::WorldCode.Plane plane = group.GetPlane();
                if (plane == World.GetActivePlane())
                {
                    this.UpdateFogForPlane(plane);
                }
            }
        }

        private void PlaneChanged(object sender, object e)
        {
            if (e is global::WorldCode.Plane plane)
            {
                if (plane.arcanusType)
                {
                    this.meshRenderer.material = AssetManager.Get().arcanusFOWMaterial;
                    this.material = this.meshRenderer.material;
                }
                else
                {
                    this.meshRenderer.material = AssetManager.Get().myrrorFOWMaterial;
                    this.material = this.meshRenderer.material;
                }
                this.material.SetVector("_Battle", new Vector4(plane.battlePlane ? plane.pathfindingArea.width : 0, plane.battlePlane ? plane.pathfindingArea.height : 0, 0f, 0f));
                Vector3 vector = HexCoordinates.HexToWorld3D(plane.area.A00);
                Vector3 vector2 = HexCoordinates.HexToWorld3D(plane.area.A11);
                if (plane.battlePlane)
                {
                    vector2.x -= 2.5f;
                    vector2.z -= 3f;
                    vector.x += 2.5f;
                    vector.z += 3f;
                }
                if (plane.area.horizontalWrap)
                {
                    vector.x = 0f;
                    vector2.x = 0f;
                }
                this.material.SetVector("_WorldArea", new Vector4(vector.x, vector.z, vector2.x, vector2.z));
                this.UpdateFogForPlane(plane);
            }
        }

        private void OnDestroy()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public static FOW Get()
        {
            return FOW.instance;
        }

        public void ResetMap(int w, int h, bool focusArcanus, bool fullReset = true)
        {
            if (this.dataTexture == null || this.dataTexture.width != w || this.dataTexture.height != h)
            {
                if (this.dataTexture != null)
                {
                    Object.Destroy(this.dataTexture);
                }
                this.dataTexture = new Texture2D(w, h, TextureFormat.Alpha8, mipChain: false);
                this.dataTexture.filterMode = FilterMode.Point;
                this.dataTexture.wrapMode = TextureWrapMode.Repeat;
                if (this.minimapDataTexture != null)
                {
                    Object.Destroy(this.minimapDataTexture);
                }
                this.minimapDataTexture = new Texture2D(w, h, TextureFormat.Alpha8, mipChain: false);
                this.minimapDataTexture.filterMode = FilterMode.Point;
                this.minimapDataTexture.wrapMode = TextureWrapMode.Repeat;
            }
            if (fullReset)
            {
                this.arcanusData = new Color[w * h];
                this.myrrorData = new Color[w * h];
            }
            if (focusArcanus)
            {
                this.dataTexture.SetPixels(this.arcanusData);
                this.minimapDataTexture.SetPixels(this.arcanusData);
            }
            else
            {
                this.dataTexture.SetPixels(this.myrrorData);
                this.minimapDataTexture.SetPixels(this.myrrorData);
            }
            this.dataTexture.Apply();
            this.minimapDataTexture.Apply();
            this.material.SetTexture("_DataTexture", this.dataTexture);
            this.arcanusOutdated = false;
            this.myrrorOutdated = false;
        }

        public void SwitchFogMiniMapTo(bool arcanus)
        {
            this.minimapDataTexture.SetPixels(arcanus ? this.arcanusData : this.myrrorData);
            this.minimapDataTexture.Apply();
        }

        public void UpdateFogArcanus()
        {
            if (this.arcanusOutdated)
            {
                this.arcanusOutdated = false;
                this.dataTexture.SetPixels(this.arcanusData);
                this.dataTexture.Apply();
                this.minimapDataTexture.SetPixels(this.arcanusData);
                this.minimapDataTexture.Apply();
                this.material.SetTexture("_DataTexture", this.dataTexture);
            }
        }

        public void UpdateFogMyrror()
        {
            if (this.myrrorOutdated)
            {
                this.myrrorOutdated = false;
                this.dataTexture.SetPixels(this.myrrorData);
                this.minimapDataTexture.SetPixels(this.myrrorData);
                this.dataTexture.Apply();
                this.minimapDataTexture.Apply();
                this.material.SetTexture("_DataTexture", this.dataTexture);
            }
        }

        private void UpdateFogDataToPlane(HashSet<Vector3i> aVisLocations, List<Group> groups, List<Location> locations, Color[] data)
        {
            int num = PlayerWizard.HumanID();
            int width = this.dataTexture.width;
            int height = this.dataTexture.height;
            int num2 = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == Color.white)
                {
                    this.MarkMisted(data, i);
                }
            }
            if (groups != null)
            {
                for (int j = 0; j < groups.Count; j++)
                {
                    Group group = groups[j];
                    if (group.locationHost != null || group.GetOwnerID() != num)
                    {
                        continue;
                    }
                    num2 = group.GetSightRange();
                    foreach (Vector3i item in HexNeighbors.GetRange(group.GetPosition(), num2))
                    {
                        Vector3i vector3i = group.GetPlane().area.KeepHorizontalInside(item);
                        if (group.GetPlane().area.IsInside(vector3i))
                        {
                            int num3 = (vector3i.x + 2 * width) % width;
                            int num4 = (vector3i.y + 2 * height) % height;
                            this.MarkVisible(data, num3 + num4 * width, vector3i);
                        }
                    }
                }
            }
            if (locations != null)
            {
                for (int num5 = locations.Count; num5 >= 0; num5--)
                {
                    if (locations.Count > num5)
                    {
                        Location location = locations[num5];
                        if (location.GetOwnerID() == num)
                        {
                            Vector3i position = location.GetPosition();
                            num2 = 3;
                            num2 += location.GetLocalGroup().GetSightRangeBonus();
                            location.ProcessIntigerScripts(EEnchantmentType.VisibilityRangeModifier, ref num2);
                            foreach (Vector3i item2 in HexNeighbors.GetRange(position, num2))
                            {
                                Vector3i vector3i2 = location.GetPlane().area.KeepHorizontalInside(item2);
                                if (location.GetPlane().area.IsInside(vector3i2))
                                {
                                    int num6 = (vector3i2.x + 2 * width) % width;
                                    int num7 = (vector3i2.y + 2 * height) % height;
                                    this.MarkVisible(data, num6 + num7 * width, vector3i2);
                                }
                            }
                        }
                    }
                }
            }
            if (aVisLocations != null)
            {
                foreach (Vector3i aVisLocation in aVisLocations)
                {
                    int num8 = (aVisLocation.x + width) % width;
                    int num9 = (aVisLocation.y + height) % height;
                    this.MarkVisible(data, num8 + num9 * width, aVisLocation);
                }
            }
            GameManager.GetHumanWizard().TriggerScripts(EEnchantmentType.WizardVisibilityModifier, this);
        }

        public void MarkVisible(Vector3i pos, bool arcanus)
        {
            int width = this.dataTexture.width;
            int height = this.dataTexture.height;
            Color[] data = (arcanus ? this.arcanusData : this.myrrorData);
            int num = (pos.x + width) % width;
            int num2 = (pos.y + height) % height;
            this.MarkVisible(data, num + num2 * width, pos);
        }

        public void MarkPlaneVisible(bool arcanus)
        {
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(arcanus ? World.GetArcanus() : World.GetMyrror());
            if (locationsOfThePlane != null)
            {
                foreach (Location item in locationsOfThePlane)
                {
                    item.MakeDiscovered();
                }
            }
            Color[] array = (arcanus ? this.arcanusData : this.myrrorData);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Color.white;
            }
        }

        public void UpdateFogDataToArcanus()
        {
            List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(World.GetArcanus());
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(World.GetArcanus());
            this.UpdateFogDataToPlane(World.GetArcanus().temporaryVisibleArea, groupsOfPlane, locationsOfThePlane, this.arcanusData);
        }

        public void UpdateFogDataToMyrror()
        {
            List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(World.GetMyrror());
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(World.GetMyrror());
            this.UpdateFogDataToPlane(World.GetMyrror().temporaryVisibleArea, groupsOfPlane, locationsOfThePlane, this.myrrorData);
        }

        public void UpdateFogForPlane(global::WorldCode.Plane plane)
        {
            if (plane == World.GetActivePlane() && !plane.battlePlane)
            {
                if (plane.arcanusType)
                {
                    this.arcanusOutdated = true;
                    this.UpdateFogDataToArcanus();
                    this.UpdateFogArcanus();
                }
                else
                {
                    this.myrrorOutdated = true;
                    this.UpdateFogDataToMyrror();
                    this.UpdateFogMyrror();
                }
            }
        }

        public void ForceFogToOutdated()
        {
            this.arcanusOutdated = true;
            this.UpdateFogDataToArcanus();
            this.myrrorOutdated = true;
            this.UpdateFogDataToMyrror();
            if (World.GetActivePlane().arcanusType)
            {
                this.UpdateFogArcanus();
            }
            else
            {
                this.UpdateFogMyrror();
            }
        }

        private void MarkVisible(Color[] data, int id, Vector3i position)
        {
            if (data[id].r < 1f)
            {
                if (this.arcanusData == data)
                {
                    this.DiscoverPosition(World.GetArcanus(), position);
                }
                else
                {
                    this.DiscoverPosition(World.GetMyrror(), position);
                }
            }
            data[id] = Color.white;
        }

        private void MarkMisted(Color[] data, int id)
        {
            data[id] = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public bool IsVisible(Vector3i a, global::WorldCode.Plane p)
        {
            int num = (a.x + 2 * this.dataTexture.width) % this.dataTexture.width;
            int num2 = (a.y + 2 * this.dataTexture.height) % this.dataTexture.height;
            if (p.arcanusType)
            {
                return (double)this.arcanusData[num + num2 * this.dataTexture.width].r > 0.6;
            }
            return (double)this.myrrorData[num + num2 * this.dataTexture.width].r > 0.6;
        }

        public bool IsDiscovered(Vector3i a, global::WorldCode.Plane p)
        {
            if (!p.area.IsInside(a))
            {
                return false;
            }
            int num = (a.x + 2 * this.dataTexture.width) % this.dataTexture.width;
            int num2 = (a.y + 2 * this.dataTexture.height) % this.dataTexture.height;
            if (p.arcanusType)
            {
                return this.arcanusData[num + num2 * this.dataTexture.width].r > 0f;
            }
            return this.myrrorData[num + num2 * this.dataTexture.width].r > 0f;
        }

        private void DiscoverPosition(global::WorldCode.Plane plane, Vector3i pos)
        {
            GameManager.GetLocationsOfThePlane(plane).Find((Location o) => !o.discovered && o.GetPosition() == pos)?.MakeDiscovered();
            Group group = GameManager.GetGroupsOfPlane(plane).Find((Group o) => o.GetPosition() == pos);
            if (group != null && group.locationHost == null)
            {
                group.GetMapFormation();
            }
            Hex hexAt = plane.GetHexAt(pos);
            if (hexAt?.Resource != null)
            {
                VerticalMarkerManager.Get().Addmarker(hexAt);
            }
        }

        public static void CleanupSequence()
        {
            if (FOW.instance != null)
            {
                if (FOW.instance.dataTexture != null)
                {
                    Object.Destroy(FOW.instance.dataTexture);
                    FOW.instance.dataTexture = null;
                }
                if (FOW.instance.minimapDataTexture != null)
                {
                    Object.Destroy(FOW.instance.minimapDataTexture);
                    FOW.instance.minimapDataTexture = null;
                }
                FOW.instance.arcanusData = null;
                FOW.instance.myrrorData = null;
            }
        }
    }
}
