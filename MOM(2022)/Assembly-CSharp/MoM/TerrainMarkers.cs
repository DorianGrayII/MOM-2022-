using System;
using System.Collections.Generic;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class TerrainMarkers
    {
        public enum MarkerType
        {
            Highlight = 5,
            Friendly = 7,
            HexHighlight = 6,
            Embark = 8,
            Path = 1,
            Borders = 12,
            MovementBorders = 25,
            Roads = 38,
            Roads2 = 51,
            PathWithMP = 16
        }

        public static bool TRUE = true;

        private static int atlasSizeX = 8;

        private static int atlasSizeY = 8;

        private static int atlasSize = TerrainMarkers.atlasSizeX * TerrainMarkers.atlasSizeY;

        private Dictionary<MarkerType, List<Vector3i>> typeToPosition = new Dictionary<MarkerType, List<Vector3i>>();

        private Vector2i size;

        public Texture2D dataTexture;

        public Vector4 merkerResolutionTexture;

        public int turns;

        public Vector3i destination;

        private bool dirty;

        private Vector2i dataSize;

        private Dictionary<int, bool[][]> layouts = new Dictionary<int, bool[][]>
        {
            {
                1,
                new bool[1][] { new bool[6]
                {
                    TerrainMarkers.TRUE,
                    false,
                    false,
                    false,
                    false,
                    false
                } }
            },
            {
                2,
                new bool[3][]
                {
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        false,
                        false,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        false,
                        TerrainMarkers.TRUE,
                        false,
                        false,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        false,
                        false,
                        TerrainMarkers.TRUE,
                        false,
                        false
                    }
                }
            },
            {
                3,
                new bool[4][]
                {
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        false,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        TerrainMarkers.TRUE,
                        false,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        false,
                        TerrainMarkers.TRUE,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        false,
                        TerrainMarkers.TRUE,
                        false,
                        TerrainMarkers.TRUE,
                        false
                    }
                }
            },
            {
                4,
                new bool[3][]
                {
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        TerrainMarkers.TRUE,
                        false
                    },
                    new bool[6]
                    {
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false,
                        TerrainMarkers.TRUE,
                        TerrainMarkers.TRUE,
                        false
                    }
                }
            },
            {
                5,
                new bool[1][] { new bool[6]
                {
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    false
                } }
            },
            {
                6,
                new bool[1][] { new bool[6]
                {
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE,
                    TerrainMarkers.TRUE
                } }
            }
        };

        private Dictionary<int, Multitype<int, int>> layoutsHashed;

        public List<Vector3i> pathLocations;

        private Vector3i highlightPosition;

        private List<Vector3i> movementAreaLocations;

        private global::WorldCode.Plane plane;

        private HashSet<MarkerType> requiredUpdate = new HashSet<MarkerType>();

        public TerrainMarkers()
        {
            MHEventSystem.RegisterListener<FSMSelectionManager>(AreaMarkerUpdate, this);
            MHEventSystem.RegisterListener<FSMMapGame>(AreaMarkerUpdate, this);
            MHEventSystem.RegisterListener<FSMBattleTurn>(AreaMarkerUpdate, this);
            MHEventSystem.RegisterListener<TurnManager>(AreaMarkerUpdate, this);
            MHEventSystem.RegisterListener<BattleHUD>(AreaMarkerUpdate, this);
            MHEventSystem.RegisterListener<HUD>(AreaMarkerUpdate, this);
            MHEventSystem.RegisterListener<World>(GeneralUpdateMarkers, this);
            MHEventSystem.RegisterListener<BattleUnit>(GeneralUpdateMarkers, this);
            MHEventSystem.RegisterListener<Group>(SelectionUpdated, this);
        }

        public void Destroy()
        {
            if (this.dataTexture != null)
            {
                global::UnityEngine.Object.Destroy(this.dataTexture);
            }
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public void Initialize(int sizeW, int sizeH, global::WorldCode.Plane p)
        {
            this.size.x = sizeW;
            this.size.y = sizeH;
            this.plane = p;
            Color32[] pixels = new Color32[this.size.x * 2 * this.size.y * 2];
            this.dataTexture = new Texture2D(this.size.x * 2, this.size.y * 2, TextureFormat.ARGB32, mipChain: false, linear: true);
            this.dataTexture.filterMode = FilterMode.Point;
            this.dataTexture.SetPixels32(pixels);
            this.dataTexture.Apply();
            foreach (MarkerType value in Enum.GetValues(typeof(MarkerType)))
            {
                this.typeToPosition[value] = new List<Vector3i>();
            }
            this.merkerResolutionTexture.x = this.size.x * 2;
            this.merkerResolutionTexture.y = this.size.y * 2;
            this.merkerResolutionTexture.z = TerrainMarkers.atlasSizeX;
            this.merkerResolutionTexture.w = TerrainMarkers.atlasSizeY;
            this.pathLocations = new List<Vector3i>();
        }

        private void SelectionUpdated(object sender, object e)
        {
            if (this.movementAreaLocations != null)
            {
                this.GeneralUpdateMarkers(sender, e);
            }
        }

        public void GeneralUpdateMarkers(object sender, object e)
        {
            if ((!(sender is IPlanePosition) || ((sender as IPlanePosition).GetPlane() != null && (sender as IPlanePosition).GetPlane() == this.plane)) && World.GetActivePlane() == this.plane)
            {
                this.MarkersUpdate(sender, e);
                this.AreaMarkerUpdate(sender, e);
                this.UpdateMarkers();
            }
        }

        private Dictionary<int, Multitype<int, int>> GetHashedLayouts()
        {
            if (this.layoutsHashed == null)
            {
                this.layoutsHashed = new Dictionary<int, Multitype<int, int>>();
                int num = -1;
                foreach (KeyValuePair<int, bool[][]> layout in this.layouts)
                {
                    for (int i = 0; i < layout.Value.Length; i++)
                    {
                        num++;
                        int num2 = this.ConvertToHashLayout(layout.Value[i]);
                        for (int j = 0; j < 6; j++)
                        {
                            int key = 0x3F & ((num2 << j) | (num2 >> 6 - j));
                            if (!this.layoutsHashed.ContainsKey(key))
                            {
                                this.layoutsHashed[key] = new Multitype<int, int>(num, j);
                            }
                        }
                    }
                }
            }
            return this.layoutsHashed;
        }

        private int ConvertToHashLayout(bool[] c)
        {
            int num = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i])
                {
                    num |= 1 << i;
                }
            }
            return num;
        }

        private void GetlayoutType(out int rotationOffset, out int typeIndex, bool[] directionSource, int neighbourCount = -1)
        {
            if (directionSource == null)
            {
                rotationOffset = 0;
                typeIndex = -1;
                return;
            }
            if (neighbourCount == -1)
            {
                neighbourCount = 0;
                for (int i = 0; i < directionSource.Length; i++)
                {
                    if (directionSource[i])
                    {
                        neighbourCount++;
                    }
                }
            }
            if (neighbourCount == 0)
            {
                rotationOffset = 0;
                typeIndex = -1;
                return;
            }
            int key = this.ConvertToHashLayout(directionSource);
            Multitype<int, int> multitype = this.GetHashedLayouts()[key];
            typeIndex = multitype.t0;
            rotationOffset = multitype.t1;
        }

        public void Highlight(Vector3i pos)
        {
            this.HideHighlight();
            this.SetBasicMarker(pos, MarkerType.Highlight, visible: true);
            this.highlightPosition = pos;
        }

        public void HideHighlight()
        {
            this.SetBasicMarker(this.highlightPosition, MarkerType.Highlight, visible: false);
        }

        public void ShowPath(List<Vector3i> path, RequestDataV2 rd)
        {
            IPlanePosition planePosition = null;
            FInt fInt = FInt.ONE * 1000;
            FInt fInt2 = FInt.ONE * 1000;
            if (this.plane.battlePlane)
            {
                planePosition = BattleHUD.GetSelectedUnit();
                fInt = (planePosition as BattleUnit).Mp;
            }
            else if (FSMSelectionManager.Get() != null)
            {
                planePosition = FSMSelectionManager.Get().GetSelectedGroup();
                Group group = planePosition as Group;
                fInt = group.CurentMP();
                fInt2 = new FInt(group.GetMaxMP());
            }
            bool flag = true;
            if (this.pathLocations != null)
            {
                flag = false;
                if (path.Count == this.pathLocations.Count)
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (path[i] != this.pathLocations[i])
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    this.HidePath(hideMarker: false);
                }
            }
            if (path.Count <= 1 || !(fInt2 > 0))
            {
                return;
            }
            if (flag)
            {
                int num = 1;
                FInt fInt3 = fInt;
                if (fInt3 == 0)
                {
                    fInt3 = fInt2;
                }
                bool[] array = new bool[6];
                Vector3i vector3i = Vector3i.zero;
                Vector3i vector3i2 = Vector3i.zero;
                bool flag2 = false;
                bool roadBuildingMode = FSMSelectionManager.Get().roadBuildingMode;
                for (int j = 0; j < path.Count; j++)
                {
                    if (flag2)
                    {
                        num++;
                    }
                    if (j > 0)
                    {
                        FInt fInt4 = rd.GetEntryCost(path[j - 1], path[j]);
                        if (fInt4 < 0)
                        {
                            fInt4 = FInt.ONE;
                        }
                        fInt3 -= fInt4;
                        flag2 = fInt3 <= 0;
                        if (flag2)
                        {
                            if (j == path.Count - 1)
                            {
                                flag2 = false;
                            }
                            fInt3 = fInt2;
                        }
                    }
                    if (j > 0)
                    {
                        vector3i = planePosition.GetPlane().pathfindingArea.KeepHorizontalInside(path[j - 1] - path[j]);
                        if (vector3i.x > 1)
                        {
                            vector3i.x -= 80;
                            vector3i.y += 40;
                            vector3i.z += 40;
                        }
                        else if (vector3i.x < -1)
                        {
                            vector3i.x += 80;
                            vector3i.y -= 40;
                            vector3i.z -= 40;
                        }
                    }
                    if (j < path.Count - 1)
                    {
                        vector3i2 = planePosition.GetPlane().pathfindingArea.KeepHorizontalInside(path[j + 1] - path[j]);
                        if (vector3i2.x > 1)
                        {
                            vector3i2.x -= 80;
                            vector3i2.y += 40;
                            vector3i2.z += 40;
                        }
                        else if (vector3i2.x < -1)
                        {
                            vector3i2.x += 80;
                            vector3i2.y -= 40;
                            vector3i2.z -= 40;
                        }
                    }
                    for (int k = 0; k < 6; k++)
                    {
                        if ((j > 0 && HexNeighbors.neighbours[k] == vector3i) || (j < path.Count - 1 && HexNeighbors.neighbours[k] == vector3i2))
                        {
                            array[k] = true;
                        }
                        else
                        {
                            array[k] = false;
                        }
                    }
                    MarkerType type = ((!flag2 || roadBuildingMode) ? MarkerType.Path : MarkerType.PathWithMP);
                    this.SetAdvancedMarker(path[j], type, visible: true, array);
                }
                if (!this.plane.battlePlane && (num > 1 || fInt == 0))
                {
                    this.destination = path[path.Count - 1];
                    this.turns = num;
                    VerticalMarkerManager.Get().DestroyMarker(this);
                    if (!roadBuildingMode)
                    {
                        VerticalMarkerManager.Get().Addmarker(this);
                    }
                }
            }
            this.pathLocations = path;
        }

        public void HidePath(bool hideMarker = true)
        {
            VerticalMarkerManager.Get().DestroyMarker(this);
            if (this.pathLocations == null || this.pathLocations.Count <= 0)
            {
                return;
            }
            foreach (Vector3i pathLocation in this.pathLocations)
            {
                this.SetAdvancedMarker(pathLocation, MarkerType.Path, visible: false);
                this.SetAdvancedMarker(pathLocation, MarkerType.PathWithMP, visible: false);
            }
            this.pathLocations.Clear();
        }

        public void ChangeTownArea(List<Vector3i> area, Color c, bool show = true)
        {
            if (area.Count <= 0)
            {
                return;
            }
            bool[] array = new bool[6];
            global::WorldCode.Plane activePlane = World.GetActivePlane();
            for (int i = 0; i < area.Count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Vector3i vector3i = area[i] + HexNeighbors.neighbours[j];
                    if (activePlane != null)
                    {
                        vector3i = activePlane.area.KeepHorizontalInside(vector3i);
                    }
                    if (!area.Contains(vector3i))
                    {
                        array[j] = true;
                    }
                    else
                    {
                        array[j] = false;
                    }
                }
                this.SetAdvancedMarker(area[i], MarkerType.Borders, show, array, c);
            }
        }

        public void ShowMovementArea(List<Vector3i> area)
        {
            bool flag = true;
            if (this.movementAreaLocations != null)
            {
                flag = false;
                if (area.Count == this.movementAreaLocations.Count)
                {
                    for (int i = 0; i < area.Count; i++)
                    {
                        if (area[i] != this.movementAreaLocations[i])
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    this.HideMovementArea();
                }
            }
            if (area.Count <= 0)
            {
                return;
            }
            if (flag)
            {
                bool[] array = new bool[6];
                global::WorldCode.Plane activePlane = World.GetActivePlane();
                for (int j = 0; j < area.Count; j++)
                {
                    if (activePlane != null && !activePlane.area.horizontalWrap && !activePlane.area.IsInside(area[j], allowOutWrapping: true))
                    {
                        continue;
                    }
                    for (int k = 0; k < 6; k++)
                    {
                        Vector3i vector3i = area[j] + HexNeighbors.neighbours[k];
                        if (activePlane != null)
                        {
                            vector3i = activePlane.area.KeepHorizontalInside(vector3i);
                        }
                        if (!area.Contains(vector3i))
                        {
                            array[k] = true;
                        }
                        else
                        {
                            array[k] = false;
                        }
                    }
                    this.SetAdvancedMarker(area[j], MarkerType.MovementBorders, visible: true, array, Color.white);
                }
            }
            this.movementAreaLocations = area;
        }

        public void HideMovementArea(bool update = false)
        {
            if (this.movementAreaLocations != null && this.movementAreaLocations.Count > 0)
            {
                foreach (Vector3i movementAreaLocation in this.movementAreaLocations)
                {
                    this.SetAdvancedMarker(movementAreaLocation, MarkerType.MovementBorders, visible: false);
                }
                this.movementAreaLocations.Clear();
            }
            if (update)
            {
                this.UpdateMarkers();
            }
        }

        public Vector2i HexToPixel(Vector3i pos)
        {
            Vector2i vector2i = HexCoordinates.HexToPixelSpace(pos);
            int num = (vector2i.x + this.size.x) % this.size.x;
            int num2 = (vector2i.y + this.size.y) % this.size.y;
            if (num < 0 || num2 < 0)
            {
                Debug.LogWarning("[Warning] SetBasicMarker at position: " + num + ", " + num2);
            }
            return new Vector2i(num, num2);
        }

        public void SetBasicMarker(Vector3i pos, MarkerType type, bool visible)
        {
            Vector2i vector2i = this.HexToPixel(pos);
            vector2i *= 2;
            if (vector2i.x < 0 || vector2i.y < 0)
            {
                return;
            }
            if (!visible && this.typeToPosition[type].Contains(pos))
            {
                this.typeToPosition[type].Remove(pos);
            }
            else if (visible && !this.typeToPosition[type].Contains(pos))
            {
                this.typeToPosition[type].Add(pos);
            }
            Color pixel = this.dataTexture.GetPixel(vector2i.x, vector2i.y);
            switch (type)
            {
            case MarkerType.Highlight:
                if (visible)
                {
                    pixel.r = 5f / (float)TerrainMarkers.atlasSize;
                }
                else
                {
                    pixel.r = 0f;
                }
                break;
            case MarkerType.HexHighlight:
                if (visible)
                {
                    pixel.g = 6f / (float)TerrainMarkers.atlasSize;
                }
                else
                {
                    pixel.g = 0f;
                }
                break;
            case MarkerType.Friendly:
                if (visible)
                {
                    pixel.b = 7f / (float)TerrainMarkers.atlasSize;
                }
                else
                {
                    pixel.b = 0f;
                }
                break;
            case MarkerType.Embark:
                if (visible)
                {
                    pixel.a = 8f / (float)TerrainMarkers.atlasSize;
                }
                else
                {
                    pixel.a = 0f;
                }
                break;
            }
            this.dataTexture.SetPixel(vector2i.x, vector2i.y, pixel);
            this.dirty = true;
        }

        public void ClearOwnershipMarker(Vector3i pos)
        {
            this.SetBasicMarker(pos, MarkerType.Friendly, visible: false);
        }

        public void SetAdvancedMarker(Vector3i pos, MarkerType type, bool visible, bool[] directionalData = null, Color color = default(Color))
        {
            Vector2i vector2i = this.HexToPixel(pos);
            vector2i *= 2;
            if (vector2i.x < 0 || vector2i.y < 0)
            {
                return;
            }
            if (!this.typeToPosition.ContainsKey(type))
            {
                this.typeToPosition[type] = new List<Vector3i>();
            }
            if (!visible && this.typeToPosition[type].Contains(pos))
            {
                this.typeToPosition[type].Remove(pos);
            }
            else if (visible && !this.typeToPosition[type].Contains(pos))
            {
                this.typeToPosition[type].Add(pos);
            }
            Color pixel = this.dataTexture.GetPixel(vector2i.x + 1, vector2i.y);
            Color pixel2 = this.dataTexture.GetPixel(vector2i.x, vector2i.y + 1);
            Color color2 = this.dataTexture.GetPixel(vector2i.x + 1, vector2i.y + 1);
            Color color3 = pixel;
            Color color4 = pixel2;
            Color color5 = color2;
            this.GetlayoutType(out var rotationOffset, out var typeIndex, directionalData);
            switch (type)
            {
            case MarkerType.Path:
            case MarkerType.PathWithMP:
                if (visible && typeIndex > -1)
                {
                    pixel.r = (float)(typeIndex + type) / (float)TerrainMarkers.atlasSize;
                    pixel2.r = (float)rotationOffset / 6f;
                }
                else
                {
                    pixel.r = 0f;
                    pixel2.r = 0f;
                }
                break;
            case MarkerType.Borders:
                if (visible && typeIndex > -1)
                {
                    pixel.g = (float)(typeIndex + 12) / (float)TerrainMarkers.atlasSize;
                    pixel2.g = (float)rotationOffset / 6f;
                    color2 = color;
                }
                else
                {
                    pixel.g = 0f;
                    pixel2.g = 0f;
                }
                break;
            case MarkerType.MovementBorders:
                if (visible && typeIndex > -1)
                {
                    pixel.b = (float)(typeIndex + 25) / (float)TerrainMarkers.atlasSize;
                    pixel2.b = (float)rotationOffset / 6f;
                }
                else
                {
                    pixel.b = 0f;
                    pixel2.b = 0f;
                }
                break;
            case MarkerType.Roads:
                if (visible && typeIndex > -1)
                {
                    pixel.a = (float)(typeIndex + 38) / (float)TerrainMarkers.atlasSize;
                    pixel2.a = (float)rotationOffset / 6f;
                }
                else
                {
                    pixel.a = 0f;
                    pixel2.a = 0f;
                }
                break;
            case MarkerType.Roads2:
                if (visible && typeIndex > -1)
                {
                    pixel.a = (float)(typeIndex + 51) / (float)TerrainMarkers.atlasSize;
                    pixel2.a = (float)rotationOffset / 6f;
                }
                else
                {
                    pixel.a = 0f;
                    pixel2.a = 0f;
                }
                break;
            }
            if (color3 != pixel)
            {
                this.dataTexture.SetPixel(vector2i.x + 1, vector2i.y, pixel);
                this.dirty = true;
            }
            if (color4 != pixel2)
            {
                this.dataTexture.SetPixel(vector2i.x, vector2i.y + 1, pixel2);
                this.dirty = true;
            }
            if (color5 != color2)
            {
                this.dataTexture.SetPixel(vector2i.x + 1, vector2i.y + 1, color2);
                this.dirty = true;
            }
        }

        public void DEBUG_Data()
        {
            GameObject.Find("IMG").GetComponent<RawImage>().texture = this.dataTexture;
        }

        public void UpdateMarkers()
        {
            if (this.dataTexture != null && this.dirty)
            {
                this.dirty = false;
                this.dataTexture.Apply();
            }
        }

        public void ClearMarkersOfType(MarkerType t)
        {
            if (!this.typeToPosition.ContainsKey(t) || this.typeToPosition[t] == null)
            {
                return;
            }
            foreach (Vector3i item in new List<Vector3i>(this.typeToPosition[t]))
            {
                this.SetBasicMarker(item, t, visible: false);
            }
        }

        public void RequestUpdate(MarkerType t)
        {
            this.requiredUpdate.Add(t);
        }

        private void MarkersUpdate(object sender, object e)
        {
            if ((!(sender is IPlanePosition) || ((sender as IPlanePosition).GetPlane() != null && (sender as IPlanePosition).GetPlane() == this.plane)) && World.GetActivePlane() == this.plane)
            {
                this.MarkersUpdate(this.plane);
            }
        }

        public void AreaMarkerUpdate(object sender, object e)
        {
            if (e != null || World.GetActivePlane() != this.plane)
            {
                return;
            }
            IPlanePosition planePosition = null;
            if (this.plane.battlePlane)
            {
                planePosition = BattleHUD.GetSelectedUnit();
            }
            else if (FSMSelectionManager.Get() != null)
            {
                planePosition = FSMSelectionManager.Get().GetSelectedGroup();
            }
            if (planePosition == null || planePosition is Location)
            {
                this.HidePath();
                this.HideMovementArea(update: true);
                MHEventSystem.TriggerEvent<TerrainMarkers>(this, null);
                return;
            }
            new HashSet<Vector3i>();
            FInt fInt = FInt.ZERO;
            if (planePosition is IGroup && (planePosition as IGroup).GetOwnerID() == PlayerWizard.HumanID())
            {
                fInt = (planePosition as Group).CurentMP();
            }
            else if (planePosition is BattleUnit && (planePosition as BattleUnit).ownerID == PlayerWizard.HumanID())
            {
                fInt = (planePosition as BattleUnit).Mp;
            }
            if (fInt <= 0)
            {
                this.HidePath();
                this.HideMovementArea(update: true);
                MHEventSystem.TriggerEvent<TerrainMarkers>(this, null);
                return;
            }
            List<Vector3i> area;
            if (planePosition is BattleUnit && (planePosition as BattleUnit).teleporting)
            {
                (planePosition as BattleUnit).GetAttFinal(TAG.TELEPORTING).ToInt();
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(planePosition.GetPlane(), planePosition.GetPosition(), FInt.ONE * 50, planePosition);
                PathfinderV2.FindArea(requestDataV);
                area = requestDataV.GetArea();
            }
            else
            {
                RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(planePosition.GetPlane(), planePosition.GetPosition(), fInt, planePosition);
                PathfinderV2.FindArea(requestDataV2);
                area = requestDataV2.GetArea();
            }
            global::WorldCode.Plane.GetMarkers().ShowMovementArea(area);
        }

        public void MarkersUpdate(global::WorldCode.Plane plane)
        {
            if (this.plane != plane)
            {
                return;
            }
            this.ClearMarkersOfType(MarkerType.Friendly);
            List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(plane);
            if (groupsOfPlane != null)
            {
                foreach (Group item in groupsOfPlane)
                {
                    if (item.alive && item.GetOwnerID() == PlayerWizard.HumanID())
                    {
                        this.SetBasicMarker(item.GetPosition(), MarkerType.Friendly, visible: true);
                    }
                }
            }
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(plane);
            if (locationsOfThePlane == null)
            {
                return;
            }
            foreach (Location item2 in locationsOfThePlane)
            {
                if (item2.GetOwnerID() == PlayerWizard.HumanID())
                {
                    this.SetBasicMarker(item2.GetPosition(), MarkerType.Friendly, visible: true);
                }
            }
        }

        public void HighlightHexes(List<Vector3i> hexes)
        {
            foreach (Vector3i hex in hexes)
            {
                this.SetBasicMarker(hex, MarkerType.HexHighlight, visible: true);
            }
            this.UpdateMarkers();
        }

        public void ClearHighlightHexes()
        {
            this.ClearMarkersOfType(MarkerType.HexHighlight);
            this.UpdateMarkers();
        }

        public bool Update(bool recalculateMarkers = false)
        {
            IPlanePosition planePosition = null;
            if (recalculateMarkers)
            {
                this.GeneralUpdateMarkers(false, false);
            }
            if (this.plane.battlePlane)
            {
                planePosition = BattleHUD.GetSelectedUnit();
                if (FSMBattleTurn.IsCastingSpells())
                {
                    this.HidePath();
                    return false;
                }
            }
            else if (FSMSelectionManager.Get() != null)
            {
                planePosition = FSMSelectionManager.Get().GetSelectedGroup();
            }
            if (planePosition == null)
            {
                return false;
            }
            if ((planePosition is IGroup && (planePosition as IGroup).GetOwnerID() == PlayerWizard.HumanID()) || (planePosition is BattleUnit && (planePosition as BattleUnit).ownerID == PlayerWizard.HumanID()))
            {
                Group group = planePosition as Group;
                Vector3 clickWorldPosition = CameraController.GetClickWorldPosition(flat: true, mousePosition: true, checkForUILock: true);
                Vector3i vector3i = Vector3i.invalid;
                if (group != null && group.destination != Vector3i.invalid)
                {
                    vector3i = group.destination;
                }
                else if (clickWorldPosition != -Vector3.one)
                {
                    vector3i = HexCoordinates.GetHexCoordAt(clickWorldPosition);
                }
                if (this.plane.battlePlane && vector3i != Vector3i.invalid)
                {
                    BattleUnit unitAt = Battle.GetBattle().GetUnitAt(vector3i);
                    if (unitAt != null)
                    {
                        if (unitAt.ownerID == (planePosition as BattleUnit).ownerID)
                        {
                            vector3i = Vector3i.invalid;
                        }
                        if (planePosition is BattleUnit battleUnit && battleUnit.currentlyVisible && Battle.AttackFormPossible(battleUnit, unitAt) != 0)
                        {
                            vector3i = Vector3i.invalid;
                        }
                    }
                }
                if (vector3i != Vector3i.invalid)
                {
                    List<Vector3i> list = null;
                    RequestDataV2 requestDataV = null;
                    if (!(planePosition is Location) && planePosition.GetPosition() != vector3i && planePosition.GetPlane() != null && planePosition.GetPlane().pathfindingArea.IsInside(vector3i))
                    {
                        requestDataV = RequestDataV2.CreateRequest(planePosition.GetPlane(), planePosition.GetPosition(), vector3i, planePosition);
                        requestDataV.allowAllyPassMode = !FSMSelectionManager.Get().roadBuildingMode;
                        requestDataV.water = requestDataV.water && !FSMSelectionManager.Get().roadBuildingMode;
                        requestDataV.nonCorporeal = requestDataV.nonCorporeal && !FSMSelectionManager.Get().roadBuildingMode;
                        PathfinderV2.FindPath(requestDataV);
                        list = requestDataV.GetPath();
                    }
                    if (list == null)
                    {
                        this.HidePath();
                    }
                    else
                    {
                        this.ShowPath(list, requestDataV);
                    }
                }
                else
                {
                    this.HidePath();
                }
            }
            else
            {
                this.HidePath();
            }
            if (this.dirty)
            {
                this.dataTexture.Apply();
                this.dirty = false;
                return true;
            }
            return false;
        }
    }
}
