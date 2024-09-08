using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DBDef;
using MHUtils;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class MinimapManager : MonoBehaviour
    {
        public struct MapSettings
        {
            public bool showGates;

            public bool showMagicNodes;

            public bool showSoultrapped;

            public bool hideNeutrals;

            public bool hideTownArea;

            public List<int> hideWizards;

            public bool showBossLair;
        }

        public const float ZOOM_OUT = 0.9f;

        public const float ZOOM_IN = 2.5f;

        private static MinimapManager instance;

        private Dictionary<string, Color> colorDictionary;

        private Texture2D arcanusMap;

        private Texture2D myrrorMap;

        private Texture2D alterArcanusMap;

        private Texture2D alterMyrrorMap;

        private Texture2D poiMap;

        private Color[] clearColor;

        private bool aMapReady;

        private bool mMapReady;

        private bool zoomOutState = true;

        private RawImage minimapRef;

        private Material minimap;

        public bool dirty;

        private bool dirtyFOW;

        private bool allowTurnDirtyDuringAITurn;

        private global::WorldCode.Plane activePlan;

        private float zoom;

        private float targetZoom;

        private Coroutine zoomTransition;

        private Vector3 cameraOverride;

        private bool useCameraOverride;

        private bool bigMode;

        public bool simpleColor;

        public MapSettings mapSettings;

        private void Start()
        {
            MinimapManager.instance = this;
            this.allowTurnDirtyDuringAITurn = false;
        }

        private void PlaneChanged(object sender, object e)
        {
            this.SetPlane(World.GetActivePlane());
        }

        private void GroupMovement(object sender, object e)
        {
            Group group = sender as Group;
            if (group == null && sender is Location location)
            {
                group = location.GetGroup();
            }
            if (group == null)
            {
                Debug.LogError("unknown sender, expected group!");
            }
            else if (group.GetPlane() == this.activePlan)
            {
                this.dirty = true;
            }
        }

        public void RegisterEvents()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
            MHEventSystem.RegisterListener<Group>(GroupMovement, this);
            MHEventSystem.RegisterListener<World>(PlaneChanged, this);
            MHEventSystem.RegisterListener<Location>(GroupMovement, this);
        }

        public static MinimapManager Get()
        {
            return MinimapManager.instance;
        }

        private void OnDestroy()
        {
            MinimapManager.instance = null;
        }

        public void SetPlane(global::WorldCode.Plane p)
        {
            this.useCameraOverride = false;
            this.dirty = true;
            if (this.activePlan != p && !p.battlePlane)
            {
                this.activePlan = p;
            }
        }

        private void Initialize(global::WorldCode.Plane p, ref Texture2D t2d, ref Texture2D at2d)
        {
            int areaWidth = p.area.AreaWidth;
            int areaHeight = p.area.AreaHeight;
            if (t2d == null)
            {
                t2d = new Texture2D(areaWidth, areaHeight, TextureFormat.RGBA32, mipChain: false);
                t2d.filterMode = FilterMode.Point;
                t2d.wrapMode = TextureWrapMode.Repeat;
            }
            if (at2d == null)
            {
                at2d = new Texture2D(areaWidth, areaHeight, TextureFormat.RGBA32, mipChain: false);
                at2d.filterMode = FilterMode.Point;
                at2d.wrapMode = TextureWrapMode.Repeat;
            }
            if (this.poiMap == null)
            {
                this.poiMap = new Texture2D(areaWidth, areaHeight, TextureFormat.RGBA32, mipChain: false);
                this.poiMap.filterMode = FilterMode.Point;
                this.poiMap.wrapMode = TextureWrapMode.Repeat;
                this.clearColor = new Color[areaWidth * areaHeight];
            }
            foreach (KeyValuePair<Vector3i, Hex> hex in p.GetHexes())
            {
                Vector2i vector2i = p.area.ConvertHexTo2DIntUVLoation(hex.Key);
                Color color = this.GetColor(hex.Value);
                if (hex.Value.IsLand())
                {
                    at2d.SetPixel(vector2i.x, vector2i.y, new Color(0.4f, 0.4f, 0.4f, 1f));
                }
                else
                {
                    at2d.SetPixel(vector2i.x, vector2i.y, color);
                }
                t2d.SetPixel(vector2i.x, vector2i.y, color);
            }
            t2d.Apply();
            at2d.Apply();
        }

        private Color GetColor(Hex source)
        {
            string minimapColor = source.GetTerrain().minimapColor;
            if (string.IsNullOrEmpty(minimapColor))
            {
                Debug.LogError("map color not found for terrain " + source.GetTerrain().dbName);
                return Color.red;
            }
            if (!source.IsLand())
            {
                List<Vector3i> range = HexNeighbors.GetRange(source.Position, 2);
                bool flag = false;
                foreach (Vector3i item in range)
                {
                    Hex hexAt = this.activePlan.GetHexAt(item);
                    if (hexAt != null && hexAt.IsLand())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Color color = this.Get(minimapColor);
                    return new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f, color.a);
                }
            }
            return this.Get(minimapColor);
        }

        private Color Get(string name)
        {
            if (this.colorDictionary == null)
            {
                this.colorDictionary = new Dictionary<string, Color>();
            }
            if (!this.colorDictionary.ContainsKey(name))
            {
                if (int.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                {
                    Color value = new Color((float)((result >> 16) & 0xFF) / 255f, (float)((result >> 8) & 0xFF) / 255f, (float)(result & 0xFF) / 255f);
                    this.colorDictionary[name] = value;
                }
                else
                {
                    Debug.LogError("Failed to translate " + name + " to Color");
                    this.colorDictionary[name] = Color.red;
                }
            }
            return this.colorDictionary[name];
        }

        public void TerrainDirty(bool arcanus)
        {
            if (arcanus)
            {
                this.aMapReady = false;
            }
            else
            {
                this.mMapReady = false;
            }
        }

        private void UpdatePOI()
        {
            global::WorldCode.Plane plane = this.activePlan;
            if (plane.battlePlane || this.poiMap == null)
            {
                return;
            }
            this.poiMap.SetPixels(this.clearColor);
            _ = plane.area.AreaWidth;
            _ = plane.area.AreaHeight;
            List<Vector3i> list = new List<Vector3i>();
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                Group group = null;
                if (!(entity.Value is Group group2) || (group2.GetPlane() != plane && !(group2.GetLocationHostSmart()?.otherPlaneLocation != null)))
                {
                    continue;
                }
                group = group2;
                if (!FOW.Get().IsDiscovered(group.GetPosition(), group.GetPlane()) || (this.mapSettings.hideWizards != null && this.mapSettings.hideWizards.Contains(group.GetOwnerID())))
                {
                    continue;
                }
                Vector2i vector2i = plane.area.ConvertHexTo2DIntUVLoation(group.GetPosition());
                Color color = WizardColors.GetColor(group.GetOwnerID());
                if (group.GetLocationHostSmart() != null)
                {
                    if (group.GetLocationHostSmart() is TownLocation)
                    {
                        color.a = 0.8f;
                        foreach (Vector3i item in HexNeighbors.GetRange(group.GetPosition(), 2))
                        {
                            Vector3i positionWrapping = plane.GetPositionWrapping(item);
                            if (!list.Contains(positionWrapping))
                            {
                                Vector2i vector2i2 = plane.area.ConvertHexTo2DIntUVLoation(positionWrapping);
                                this.poiMap.SetPixel(vector2i2.x, vector2i2.y, color);
                            }
                        }
                        color.a = 1f;
                    }
                    else if (group.GetLocationHostSmart().power > 0)
                    {
                        if (this.mapSettings.showMagicNodes)
                        {
                            color.a = 0.4f;
                        }
                        else
                        {
                            color.a = 0f;
                        }
                    }
                    else if (group.GetLocationHostSmart().locationType == ELocationType.MidGameLair)
                    {
                        if (this.mapSettings.showSoultrapped)
                        {
                            color.a = 0.3f;
                        }
                        else
                        {
                            color.a = 0f;
                        }
                    }
                    else if (group.GetLocationHostSmart().locationType == ELocationType.BossLair)
                    {
                        if (this.mapSettings.showBossLair)
                        {
                            color.a = 0.5f;
                        }
                        else
                        {
                            color.a = 0f;
                        }
                    }
                    else
                    {
                        if (!(group.GetLocationHostSmart().otherPlaneLocation != null))
                        {
                            continue;
                        }
                        if (this.mapSettings.showGates)
                        {
                            color.a = 0.2f;
                        }
                        else
                        {
                            color.a = 0f;
                        }
                    }
                }
                else
                {
                    if (!FOW.Get().IsVisible(group.GetPosition(), group.GetPlane()))
                    {
                        continue;
                    }
                    bool flag = true;
                    if (group != null)
                    {
                        Group group3 = group;
                        foreach (Reference<Unit> unit in group3.GetUnits())
                        {
                            if (!unit.Get().IsInvisibleUnit())
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        continue;
                    }
                    color.a = 0.6f;
                }
                list.Add(group.GetPosition());
                this.poiMap.SetPixel(vector2i.x, vector2i.y, color);
            }
            this.poiMap.Apply();
        }

        public void SetMinimap(RawImage image)
        {
            this.minimapRef = image;
            this.minimap = this.minimapRef.material;
            image.texture = UIReferences.GetTransparent();
        }

        public void Clear()
        {
            this.minimap = null;
            this.activePlan = null;
            if (this.arcanusMap != null)
            {
                Object.Destroy(this.arcanusMap);
            }
            if (this.myrrorMap != null)
            {
                Object.Destroy(this.myrrorMap);
            }
            if (this.alterArcanusMap != null)
            {
                Object.Destroy(this.alterArcanusMap);
            }
            if (this.alterMyrrorMap != null)
            {
                Object.Destroy(this.alterMyrrorMap);
            }
            if (this.poiMap != null)
            {
                Object.Destroy(this.poiMap);
            }
            this.arcanusMap = null;
            this.myrrorMap = null;
            this.alterArcanusMap = null;
            this.alterMyrrorMap = null;
            this.poiMap = null;
        }

        public void BigMapMode(bool b, MapSettings settings)
        {
            this.mapSettings = settings;
            this.bigMode = b;
            this.dirty = true;
        }

        private void Update()
        {
            if (this.minimap == null || (this.allowTurnDirtyDuringAITurn && TurnManager.Get().aiTurn))
            {
                return;
            }
            global::WorldCode.Plane plane = this.activePlan;
            if (plane.battlePlane)
            {
                return;
            }
            if (this.dirty)
            {
                this.dirty = false;
                if (plane.arcanusType)
                {
                    if (!this.aMapReady)
                    {
                        this.aMapReady = true;
                        this.Initialize(plane, ref this.arcanusMap, ref this.alterArcanusMap);
                    }
                    this.minimap.SetTexture("_WorldData", this.simpleColor ? this.alterArcanusMap : this.arcanusMap);
                }
                else
                {
                    if (!this.mMapReady)
                    {
                        this.mMapReady = true;
                        this.Initialize(plane, ref this.myrrorMap, ref this.alterMyrrorMap);
                    }
                    this.minimap.SetTexture("_WorldData", this.simpleColor ? this.alterMyrrorMap : this.myrrorMap);
                }
                this.UpdatePOI();
                this.minimap.SetTexture("_POI", this.poiMap);
                this.minimap.SetTexture("_FOW", FOW.Get().GetMinimapDataTexture());
                Vector3 vector = HexCoordinates.HexToWorld3D(plane.area.A00);
                Vector3 vector2 = HexCoordinates.HexToWorld3D(plane.area.A11);
                this.minimap.SetVector("_WorldArea", new Vector4(vector.x, vector.z, vector2.x, vector2.z));
                if (this.arcanusMap != null)
                {
                    this.minimap.SetVector("_TextureArea", new Vector4(this.arcanusMap.width, this.arcanusMap.height, 1f / (float)this.arcanusMap.width, 1f / (float)this.arcanusMap.height));
                }
                else
                {
                    this.minimap.SetVector("_TextureArea", new Vector4(this.myrrorMap.width, this.myrrorMap.height, 1f / (float)this.myrrorMap.width, 1f / (float)this.myrrorMap.height));
                }
            }
            if (this.bigMode)
            {
                this.minimap.SetVector("_Zoom", new Vector4(0.9f, 0.9f, 2.5f, 1f));
                this.minimap.SetVector("_Settings2", new Vector4(0f, 0f, 0f, 1f));
                return;
            }
            if (this.useCameraOverride)
            {
                this.minimap.SetVector("_Camera", this.cameraOverride);
                this.minimap.SetVector("_Zoom", new Vector4(2.5f, 0.9f, 2.5f, 1f));
                this.minimap.SetVector("_Settings2", new Vector4(0f, 0f, 0f, 1f));
                return;
            }
            this.minimap.SetVector("_Camera", CameraController.GetCameraPosition() + new Vector3(0f, 0f, 12f));
            Vector3 clickWorldPositionUV = CameraController.GetClickWorldPositionUV(new Vector2(0f, 1f), flat: true);
            Vector3 clickWorldPositionUV2 = CameraController.GetClickWorldPositionUV(new Vector2(1f, 1f), flat: true);
            this.minimap.SetVector("_WorldViewTop", new Vector4(clickWorldPositionUV.x, clickWorldPositionUV.z, clickWorldPositionUV2.x, clickWorldPositionUV2.z));
            Vector3 clickWorldPositionUV3 = CameraController.GetClickWorldPositionUV(new Vector2(0f, 0f), flat: true);
            Vector3 clickWorldPositionUV4 = CameraController.GetClickWorldPositionUV(new Vector2(1f, 0f), flat: true);
            this.minimap.SetVector("_WorldViewBot", new Vector4(clickWorldPositionUV3.x, clickWorldPositionUV3.z, clickWorldPositionUV4.x, clickWorldPositionUV4.z));
            this.minimap.SetVector("_Zoom", new Vector4(this.zoom, 0.9f, 2.5f, 1f));
            this.minimap.SetVector("_Settings2", new Vector4(1f, 0f, 0f, 1f));
        }

        public void FocusMinimap(global::WorldCode.Plane plane, Vector3i pos)
        {
            this.SetPlane(plane);
            this.cameraOverride = HexCoordinates.HexToWorld3D(pos);
            this.useCameraOverride = true;
            this.minimap.SetVector("_AnimatedPOI", new Vector4(pos.x, pos.y, pos.z, 1f));
        }

        public bool InitializeZoom()
        {
            if (this.zoomOutState)
            {
                this.ChangeZoom(0.9f);
            }
            else
            {
                this.ChangeZoom(2.5f);
            }
            return this.zoomOutState;
        }

        public void ChangeZoom(float value)
        {
            this.zoomOutState = value == 0.9f;
            if (this.targetZoom != value)
            {
                if (this.zoomTransition != null)
                {
                    base.StopCoroutine(this.zoomTransition);
                }
                this.targetZoom = value;
                this.zoomTransition = base.StartCoroutine(this.ChangeZoom());
            }
            if (HUD.Get() != null)
            {
                HUD.Get().MinimapInMode(value == 2.5f);
            }
        }

        private IEnumerator ChangeZoom()
        {
            for (int j = 0; j < 10; j++)
            {
                this.zoom = Mathf.Lerp(this.zoom, this.targetZoom, 0.2f);
                yield return null;
            }
            for (int j = 0; j < 5; j++)
            {
                this.zoom = Mathf.Lerp(this.zoom, this.targetZoom, 0.2f * (float)j);
                yield return null;
            }
            this.zoom = this.targetZoom;
            this.zoomTransition = null;
        }

        public bool MapInZoomoutMode()
        {
            return this.zoom == 0.9f;
        }
    }
}
