namespace MOM
{
    using DBDef;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using WorldCode;

    public class FOW : MonoBehaviour
    {
        public const bool DEBUG_AI = false;
        private static FOW instance;
        private Material material;
        private MeshRenderer meshRenderer;
        private Texture2D dataTexture;
        private Texture2D minimapDataTexture;
        private UnityEngine.Color[] arcanusData;
        private UnityEngine.Color[] myrrorData;
        private bool arcanusOutdated;
        private bool myrrorOutdated;

        public static void CleanupSequence()
        {
            if (instance != null)
            {
                if (instance.dataTexture != null)
                {
                    Destroy(instance.dataTexture);
                    instance.dataTexture = null;
                }
                if (instance.minimapDataTexture != null)
                {
                    Destroy(instance.minimapDataTexture);
                    instance.minimapDataTexture = null;
                }
                instance.arcanusData = null;
                instance.myrrorData = null;
            }
        }

        private void DiscoverPosition(WorldCode.Plane plane, Vector3i pos)
        {
            MOM.Location location = GameManager.GetLocationsOfThePlane(plane).Find(o => !o.discovered && (o.GetPosition() == pos));
            if (location != null)
            {
                location.MakeDiscovered();
            }
            MOM.Group group = GameManager.GetGroupsOfPlane(plane).Find(o => o.GetPosition() == pos);
            if ((group != null) && (group.locationHost == null))
            {
                group.GetMapFormation(true);
            }
            Hex hexAt = plane.GetHexAt(pos);
            if (hexAt?.Resource != null)
            {
                VerticalMarkerManager.Get().Addmarker(hexAt);
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

        public static FOW Get()
        {
            return instance;
        }

        public UnityEngine.Color[] GetArcanusData()
        {
            return this.arcanusData;
        }

        public Texture2D GetDatatexture()
        {
            return this.dataTexture;
        }

        public Texture2D GetMinimapDataTexture()
        {
            return this.minimapDataTexture;
        }

        public UnityEngine.Color[] GetMyrrorData()
        {
            return this.myrrorData;
        }

        private void GroupMoved(object sender, object e)
        {
            MOM.Group group = sender as MOM.Group;
            if (((group != null) && (group.GetOwnerID() == PlayerWizard.HumanID())) && ((e is List<Vector3i>) || !group.alive))
            {
                WorldCode.Plane objA = group.GetPlane();
                if (ReferenceEquals(objA, World.GetActivePlane()))
                {
                    this.UpdateFogForPlane(objA);
                }
            }
        }

        public bool IsDiscovered(Vector3i a, WorldCode.Plane p)
        {
            if (!p.area.IsInside(a, false))
            {
                return false;
            }
            int num = (a.x + (2 * this.dataTexture.width)) % this.dataTexture.width;
            int num2 = (a.y + (2 * this.dataTexture.height)) % this.dataTexture.height;
            return (!p.arcanusType ? (this.myrrorData[num + (num2 * this.dataTexture.width)].r > 0f) : (this.arcanusData[num + (num2 * this.dataTexture.width)].r > 0f));
        }

        public bool IsVisible(Vector3i a, WorldCode.Plane p)
        {
            int num = (a.x + (2 * this.dataTexture.width)) % this.dataTexture.width;
            int num2 = (a.y + (2 * this.dataTexture.height)) % this.dataTexture.height;
            return (!p.arcanusType ? (this.myrrorData[num + (num2 * this.dataTexture.width)].r > 0.6) : (this.arcanusData[num + (num2 * this.dataTexture.width)].r > 0.6));
        }

        private void MarkMisted(UnityEngine.Color[] data, int id)
        {
            data[id] = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void MarkPlaneVisible(bool arcanus)
        {
            List<MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(arcanus ? World.GetArcanus() : World.GetMyrror());
            if (locationsOfThePlane != null)
            {
                using (List<MOM.Location>.Enumerator enumerator = locationsOfThePlane.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.MakeDiscovered();
                    }
                }
            }
            UnityEngine.Color[] colorArray = arcanus ? this.arcanusData : this.myrrorData;
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = UnityEngine.Color.white;
            }
        }

        public void MarkVisible(Vector3i pos, bool arcanus)
        {
            int width = this.dataTexture.width;
            int height = this.dataTexture.height;
            UnityEngine.Color[] data = arcanus ? this.arcanusData : this.myrrorData;
            this.MarkVisible(data, ((pos.x + width) % width) + (((pos.y + height) % height) * width), pos);
        }

        private void MarkVisible(UnityEngine.Color[] data, int id, Vector3i position)
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
            data[id] = UnityEngine.Color.white;
        }

        private void OnDestroy()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        private unsafe void PlaneChanged(object sender, object e)
        {
            WorldCode.Plane plane = e as WorldCode.Plane;
            if (plane != null)
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
                this.material.SetVector("_Battle", new Vector4(plane.battlePlane ? ((float) plane.pathfindingArea.width) : ((float) 0), plane.battlePlane ? ((float) plane.pathfindingArea.height) : ((float) 0), 0f, 0f));
                Vector3 vector = HexCoordinates.HexToWorld3D(plane.area.A00);
                Vector3 vector2 = HexCoordinates.HexToWorld3D(plane.area.A11);
                if (plane.battlePlane)
                {
                    float* singlePtr1 = &vector2.x;
                    singlePtr1[0] -= 2.5f;
                    float* singlePtr2 = &vector2.z;
                    singlePtr2[0] -= 3f;
                    float* singlePtr3 = &vector.x;
                    singlePtr3[0] += 2.5f;
                    float* singlePtr4 = &vector.z;
                    singlePtr4[0] += 3f;
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

        public void ResetMap(int w, int h, bool focusArcanus, bool fullReset)
        {
            if ((this.dataTexture == null) || ((this.dataTexture.width != w) || (this.dataTexture.height != h)))
            {
                if (this.dataTexture != null)
                {
                    Destroy(this.dataTexture);
                }
                this.dataTexture = new Texture2D(w, h, TextureFormat.Alpha8, false);
                this.dataTexture.filterMode = FilterMode.Point;
                this.dataTexture.wrapMode = TextureWrapMode.Repeat;
                if (this.minimapDataTexture != null)
                {
                    Destroy(this.minimapDataTexture);
                }
                this.minimapDataTexture = new Texture2D(w, h, TextureFormat.Alpha8, false);
                this.minimapDataTexture.filterMode = FilterMode.Point;
                this.minimapDataTexture.wrapMode = TextureWrapMode.Repeat;
            }
            if (fullReset)
            {
                this.arcanusData = new UnityEngine.Color[w * h];
                this.myrrorData = new UnityEngine.Color[w * h];
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

        public void SetArcanusData(float[] f)
        {
            UnityEngine.Color[] colorArray = new UnityEngine.Color[f.Length / 4];
            for (int i = 0; i < f.Length; i += 4)
            {
                UnityEngine.Color color = new UnityEngine.Color(f[i], f[i + 1], f[i + 2], f[i + 3]);
                colorArray[i / 4] = color;
            }
            this.arcanusData = colorArray;
        }

        public void SetMyrrorData(float[] f)
        {
            UnityEngine.Color[] colorArray = new UnityEngine.Color[f.Length / 4];
            for (int i = 0; i < f.Length; i += 4)
            {
                UnityEngine.Color color = new UnityEngine.Color(f[i], f[i + 1], f[i + 2], f[i + 3]);
                colorArray[i / 4] = color;
            }
            this.myrrorData = colorArray;
        }

        private void Start()
        {
            this.meshRenderer = base.GetComponent<MeshRenderer>();
            this.material = this.meshRenderer.material;
            instance = this;
            MHEventSystem.RegisterListener<World>(new EventFunction(this.PlaneChanged), this);
            MHEventSystem.RegisterListener<MOM.Group>(new EventFunction(this.GroupMoved), this);
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

        public void UpdateFogDataToArcanus()
        {
            List<MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(World.GetArcanus());
            this.UpdateFogDataToPlane(World.GetArcanus().temporaryVisibleArea, groupsOfPlane, GameManager.GetLocationsOfThePlane(World.GetArcanus()), this.arcanusData);
        }

        public void UpdateFogDataToMyrror()
        {
            List<MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(World.GetMyrror());
            this.UpdateFogDataToPlane(World.GetMyrror().temporaryVisibleArea, groupsOfPlane, GameManager.GetLocationsOfThePlane(World.GetMyrror()), this.myrrorData);
        }

        private void UpdateFogDataToPlane(HashSet<Vector3i> aVisLocations, List<MOM.Group> groups, List<MOM.Location> locations, UnityEngine.Color[] data)
        {
            int num = PlayerWizard.HumanID();
            int width = this.dataTexture.width;
            int height = this.dataTexture.height;
            int maxDistance = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == UnityEngine.Color.white)
                {
                    this.MarkMisted(data, i);
                }
            }
            if (groups != null)
            {
                for (int j = 0; j < groups.Count; j++)
                {
                    MOM.Group group = groups[j];
                    if ((group.locationHost == null) && (group.GetOwnerID() == num))
                    {
                        maxDistance = group.GetSightRange();
                        foreach (Vector3i vectori in HexNeighbors.GetRange(group.GetPosition(), maxDistance))
                        {
                            Vector3i worldPosition = group.GetPlane().area.KeepHorizontalInside(vectori);
                            if (group.GetPlane().area.IsInside(worldPosition, false))
                            {
                                int num7 = (worldPosition.x + (2 * width)) % width;
                                this.MarkVisible(data, num7 + (((worldPosition.y + (2 * height)) % height) * width), worldPosition);
                            }
                        }
                    }
                }
            }
            if (locations != null)
            {
                for (int j = locations.Count; j >= 0; j--)
                {
                    if (locations.Count > j)
                    {
                        MOM.Location location = locations[j];
                        if (location.GetOwnerID() == num)
                        {
                            maxDistance = 3 + location.GetLocalGroup().GetSightRangeBonus();
                            IEnchantableExtension.ProcessIntigerScripts(location, EEnchantmentType.VisibilityRangeModifier, ref maxDistance);
                            foreach (Vector3i vectori3 in HexNeighbors.GetRange(location.GetPosition(), maxDistance))
                            {
                                Vector3i worldPosition = location.GetPlane().area.KeepHorizontalInside(vectori3);
                                if (location.GetPlane().area.IsInside(worldPosition, false))
                                {
                                    int num10 = (worldPosition.x + (2 * width)) % width;
                                    this.MarkVisible(data, num10 + (((worldPosition.y + (2 * height)) % height) * width), worldPosition);
                                }
                            }
                        }
                    }
                }
            }
            if (aVisLocations != null)
            {
                foreach (Vector3i vectori5 in aVisLocations)
                {
                    int num12 = (vectori5.x + width) % width;
                    int num13 = (vectori5.y + height) % height;
                    this.MarkVisible(data, num12 + (num13 * width), vectori5);
                }
            }
            IEnchantableExtension.TriggerScripts(GameManager.GetHumanWizard(), EEnchantmentType.WizardVisibilityModifier, this, null);
        }

        public void UpdateFogForPlane(WorldCode.Plane plane)
        {
            if (ReferenceEquals(plane, World.GetActivePlane()) && !plane.battlePlane)
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
    }
}

