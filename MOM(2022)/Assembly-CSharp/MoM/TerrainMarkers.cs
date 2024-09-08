namespace MOM
{
    using DBEnum;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using WorldCode;

    public class TerrainMarkers
    {
        public static bool TRUE = true;
        private static int atlasSizeX = 8;
        private static int atlasSizeY = 8;
        private static int atlasSize = (atlasSizeX * atlasSizeY);
        private Dictionary<MarkerType, List<Vector3i>> typeToPosition = new Dictionary<MarkerType, List<Vector3i>>();
        private Vector2i size;
        public Texture2D dataTexture;
        public Vector4 merkerResolutionTexture;
        public int turns;
        public Vector3i destination;
        private bool dirty;
        private Vector2i dataSize;
        private Dictionary<int, bool[][]> layouts;
        private Dictionary<int, Multitype<int, int>> layoutsHashed;
        public List<Vector3i> pathLocations;
        private Vector3i highlightPosition;
        private List<Vector3i> movementAreaLocations;
        private WorldCode.Plane plane;
        private HashSet<MarkerType> requiredUpdate;

        public TerrainMarkers()
        {
            Dictionary<int, bool[][]> dictionary = new Dictionary<int, bool[][]>();
            bool[] flagArray1 = new bool[6];
            flagArray1[0] = TRUE;
            bool[][] flagArrayArray1 = new bool[][] { flagArray1 };
            dictionary.Add(1, flagArrayArray1);
            bool[] flagArray2 = new bool[6];
            flagArray2[0] = TRUE;
            flagArray2[1] = TRUE;
            bool[][] flagArrayArray2 = new bool[3][];
            flagArrayArray2[0] = flagArray2;
            bool[] flagArray3 = new bool[6];
            flagArray3[0] = TRUE;
            flagArray3[2] = TRUE;
            flagArrayArray2[1] = flagArray3;
            bool[] flagArray4 = new bool[6];
            flagArray4[0] = TRUE;
            flagArray4[3] = TRUE;
            flagArrayArray2[2] = flagArray4;
            dictionary.Add(2, flagArrayArray2);
            bool[] flagArray5 = new bool[6];
            flagArray5[0] = TRUE;
            flagArray5[1] = TRUE;
            flagArray5[2] = TRUE;
            bool[][] flagArrayArray3 = new bool[4][];
            flagArrayArray3[0] = flagArray5;
            bool[] flagArray6 = new bool[6];
            flagArray6[0] = TRUE;
            flagArray6[1] = TRUE;
            flagArray6[3] = TRUE;
            flagArrayArray3[1] = flagArray6;
            bool[] flagArray7 = new bool[6];
            flagArray7[0] = TRUE;
            flagArray7[1] = TRUE;
            flagArray7[4] = TRUE;
            flagArrayArray3[2] = flagArray7;
            bool[] flagArray8 = new bool[6];
            flagArray8[0] = TRUE;
            flagArray8[2] = TRUE;
            flagArray8[4] = TRUE;
            flagArrayArray3[3] = flagArray8;
            dictionary.Add(3, flagArrayArray3);
            bool[] flagArray9 = new bool[6];
            flagArray9[0] = TRUE;
            flagArray9[1] = TRUE;
            flagArray9[2] = TRUE;
            flagArray9[3] = TRUE;
            bool[][] flagArrayArray4 = new bool[3][];
            flagArrayArray4[0] = flagArray9;
            bool[] flagArray10 = new bool[6];
            flagArray10[0] = TRUE;
            flagArray10[1] = TRUE;
            flagArray10[2] = TRUE;
            flagArray10[4] = TRUE;
            flagArrayArray4[1] = flagArray10;
            bool[] flagArray11 = new bool[6];
            flagArray11[0] = TRUE;
            flagArray11[1] = TRUE;
            flagArray11[3] = TRUE;
            flagArray11[4] = TRUE;
            flagArrayArray4[2] = flagArray11;
            dictionary.Add(4, flagArrayArray4);
            bool[] flagArray12 = new bool[6];
            flagArray12[0] = TRUE;
            flagArray12[1] = TRUE;
            flagArray12[2] = TRUE;
            flagArray12[3] = TRUE;
            flagArray12[4] = TRUE;
            bool[][] flagArrayArray5 = new bool[][] { flagArray12 };
            dictionary.Add(5, flagArrayArray5);
            bool[] flagArray13 = new bool[] { TRUE, TRUE, TRUE, TRUE, TRUE, TRUE };
            bool[][] flagArrayArray6 = new bool[][] { flagArray13 };
            dictionary.Add(6, flagArrayArray6);
            this.layouts = dictionary;
            this.requiredUpdate = new HashSet<MarkerType>();
            MHEventSystem.RegisterListener<FSMSelectionManager>(new EventFunction(this.AreaMarkerUpdate), this);
            MHEventSystem.RegisterListener<FSMMapGame>(new EventFunction(this.AreaMarkerUpdate), this);
            MHEventSystem.RegisterListener<FSMBattleTurn>(new EventFunction(this.AreaMarkerUpdate), this);
            MHEventSystem.RegisterListener<TurnManager>(new EventFunction(this.AreaMarkerUpdate), this);
            MHEventSystem.RegisterListener<BattleHUD>(new EventFunction(this.AreaMarkerUpdate), this);
            MHEventSystem.RegisterListener<HUD>(new EventFunction(this.AreaMarkerUpdate), this);
            MHEventSystem.RegisterListener<World>(new EventFunction(this.GeneralUpdateMarkers), this);
            MHEventSystem.RegisterListener<BattleUnit>(new EventFunction(this.GeneralUpdateMarkers), this);
            MHEventSystem.RegisterListener<Group>(new EventFunction(this.SelectionUpdated), this);
        }

        public void AreaMarkerUpdate(object sender, object e)
        {
            if ((e == null) && ReferenceEquals(World.GetActivePlane(), this.plane))
            {
                IPlanePosition unit = null;
                if (this.plane.battlePlane)
                {
                    unit = BattleHUD.GetSelectedUnit();
                }
                else if (FSMSelectionManager.Get() != null)
                {
                    unit = FSMSelectionManager.Get().GetSelectedGroup();
                }
                if ((unit == null) || (unit is Location))
                {
                    this.HidePath(true);
                    this.HideMovementArea(true);
                    MHEventSystem.TriggerEvent<TerrainMarkers>(this, null);
                }
                else
                {
                    HashSet<Vector3i> set1 = new HashSet<Vector3i>();
                    FInt zERO = FInt.ZERO;
                    if ((unit is IGroup) && ((unit as IGroup).GetOwnerID() == PlayerWizard.HumanID()))
                    {
                        zERO = (unit as Group).CurentMP();
                    }
                    else if ((unit is BattleUnit) && ((unit as BattleUnit).ownerID == PlayerWizard.HumanID()))
                    {
                        zERO = (unit as BattleUnit).Mp;
                    }
                    if (zERO <= 0)
                    {
                        this.HidePath(true);
                        this.HideMovementArea(true);
                        MHEventSystem.TriggerEvent<TerrainMarkers>(this, null);
                    }
                    else
                    {
                        List<Vector3i> area;
                        if (!(unit is BattleUnit) || !(unit as BattleUnit).teleporting)
                        {
                            RequestDataV2 rd = RequestDataV2.CreateRequest(unit.GetPlane(), unit.GetPosition(), zERO, unit, false);
                            PathfinderV2.FindArea(rd, false);
                            area = rd.GetArea();
                        }
                        else
                        {
                            IAttributeableExtension.GetAttFinal(unit as BattleUnit, TAG.TELEPORTING).ToInt();
                            RequestDataV2 rd = RequestDataV2.CreateRequest(unit.GetPlane(), unit.GetPosition(), FInt.ONE * 50, unit, false);
                            PathfinderV2.FindArea(rd, false);
                            area = rd.GetArea();
                        }
                        WorldCode.Plane.GetMarkers().ShowMovementArea(area);
                    }
                }
            }
        }

        public void ChangeTownArea(List<Vector3i> area, UnityEngine.Color c, bool show)
        {
            if (area.Count > 0)
            {
                bool[] directionalData = new bool[6];
                WorldCode.Plane activePlane = World.GetActivePlane();
                int num = 0;
                while (num < area.Count)
                {
                    int index = 0;
                    while (true)
                    {
                        if (index >= 6)
                        {
                            this.SetAdvancedMarker(area[num], MarkerType.Borders, show, directionalData, c);
                            num++;
                            break;
                        }
                        Vector3i pos = area[num] + HexNeighbors.neighbours[index];
                        if (activePlane != null)
                        {
                            pos = activePlane.area.KeepHorizontalInside(pos);
                        }
                        directionalData[index] = !area.Contains(pos);
                        index++;
                    }
                }
            }
        }

        public void ClearHighlightHexes()
        {
            this.ClearMarkersOfType(MarkerType.HexHighlight);
            this.UpdateMarkers();
        }

        public void ClearMarkersOfType(MarkerType t)
        {
            if (this.typeToPosition.ContainsKey(t) && (this.typeToPosition[t] != null))
            {
                foreach (Vector3i vectori in new List<Vector3i>(this.typeToPosition[t]))
                {
                    this.SetBasicMarker(vectori, t, false);
                }
            }
        }

        public void ClearOwnershipMarker(Vector3i pos)
        {
            this.SetBasicMarker(pos, MarkerType.Friendly, false);
        }

        private int ConvertToHashLayout(bool[] c)
        {
            int num = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i])
                {
                    num |= 1 << (i & 0x1f);
                }
            }
            return num;
        }

        public void DEBUG_Data()
        {
            GameObject.Find("IMG").GetComponent<RawImage>().texture = this.dataTexture;
        }

        public void Destroy()
        {
            if (this.dataTexture != null)
            {
                UnityEngine.Object.Destroy(this.dataTexture);
            }
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public void GeneralUpdateMarkers(object sender, object e)
        {
            if ((!(sender is IPlanePosition) || (((sender as IPlanePosition).GetPlane() != null) && ReferenceEquals((sender as IPlanePosition).GetPlane(), this.plane))) && ReferenceEquals(World.GetActivePlane(), this.plane))
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
                foreach (KeyValuePair<int, bool[][]> pair in this.layouts)
                {
                    int index = 0;
                    while (index < pair.Value.Length)
                    {
                        num++;
                        int num3 = this.ConvertToHashLayout(pair.Value[index]);
                        int num4 = 0;
                        while (true)
                        {
                            if (num4 >= 6)
                            {
                                index++;
                                break;
                            }
                            int key = 0x3f & ((num3 << (num4 & 0x1f)) | (num3 >> ((6 - num4) & 0x1f)));
                            if (!this.layoutsHashed.ContainsKey(key))
                            {
                                this.layoutsHashed[key] = new Multitype<int, int>(num, num4);
                            }
                            num4++;
                        }
                    }
                }
            }
            return this.layoutsHashed;
        }

        private void GetlayoutType(out int rotationOffset, out int typeIndex, bool[] directionSource, int neighbourCount)
        {
            if (directionSource == null)
            {
                rotationOffset = 0;
                typeIndex = -1;
            }
            else
            {
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
                }
                else
                {
                    int num = this.ConvertToHashLayout(directionSource);
                    Multitype<int, int> multitype = this.GetHashedLayouts()[num];
                    typeIndex = multitype.t0;
                    rotationOffset = multitype.t1;
                }
            }
        }

        public Vector2i HexToPixel(Vector3i pos)
        {
            Vector2i vectori1 = HexCoordinates.HexToPixelSpace(pos);
            int x = (vectori1.x + this.size.x) % this.size.x;
            int y = (vectori1.y + this.size.y) % this.size.y;
            if ((x < 0) || (y < 0))
            {
                Debug.LogWarning("[Warning] SetBasicMarker at position: " + x.ToString() + ", " + y.ToString());
            }
            return new Vector2i(x, y);
        }

        public void HideHighlight()
        {
            this.SetBasicMarker(this.highlightPosition, MarkerType.Highlight, false);
        }

        public void HideMovementArea(bool update)
        {
            if ((this.movementAreaLocations != null) && (this.movementAreaLocations.Count > 0))
            {
                foreach (Vector3i vectori in this.movementAreaLocations)
                {
                    UnityEngine.Color color = new UnityEngine.Color();
                    this.SetAdvancedMarker(vectori, MarkerType.MovementBorders, false, null, color);
                }
                this.movementAreaLocations.Clear();
            }
            if (update)
            {
                this.UpdateMarkers();
            }
        }

        public void HidePath(bool hideMarker)
        {
            VerticalMarkerManager.Get().DestroyMarker(this, true);
            if ((this.pathLocations != null) && (this.pathLocations.Count > 0))
            {
                foreach (Vector3i vectori in this.pathLocations)
                {
                    UnityEngine.Color color = new UnityEngine.Color();
                    this.SetAdvancedMarker(vectori, MarkerType.Path, false, null, color);
                    color = new UnityEngine.Color();
                    this.SetAdvancedMarker(vectori, MarkerType.PathWithMP, false, null, color);
                }
                this.pathLocations.Clear();
            }
        }

        public void Highlight(Vector3i pos)
        {
            this.HideHighlight();
            this.SetBasicMarker(pos, MarkerType.Highlight, true);
            this.highlightPosition = pos;
        }

        public void HighlightHexes(List<Vector3i> hexes)
        {
            foreach (Vector3i vectori in hexes)
            {
                this.SetBasicMarker(vectori, MarkerType.HexHighlight, true);
            }
            this.UpdateMarkers();
        }

        public void Initialize(int sizeW, int sizeH, WorldCode.Plane p)
        {
            this.size.x = sizeW;
            this.size.y = sizeH;
            this.plane = p;
            Color32[] colors = new Color32[((this.size.x * 2) * this.size.y) * 2];
            this.dataTexture = new Texture2D(this.size.x * 2, this.size.y * 2, TextureFormat.ARGB32, false, true);
            this.dataTexture.filterMode = FilterMode.Point;
            this.dataTexture.SetPixels32(colors);
            this.dataTexture.Apply();
            foreach (MarkerType type in Enum.GetValues(typeof(MarkerType)))
            {
                this.typeToPosition[type] = new List<Vector3i>();
            }
            this.merkerResolutionTexture.x = this.size.x * 2;
            this.merkerResolutionTexture.y = this.size.y * 2;
            this.merkerResolutionTexture.z = atlasSizeX;
            this.merkerResolutionTexture.w = atlasSizeY;
            this.pathLocations = new List<Vector3i>();
        }

        public void MarkersUpdate(WorldCode.Plane plane)
        {
            if (ReferenceEquals(this.plane, plane))
            {
                this.ClearMarkersOfType(MarkerType.Friendly);
                List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(plane);
                if (groupsOfPlane != null)
                {
                    foreach (Group group in groupsOfPlane)
                    {
                        if (group.alive && (group.GetOwnerID() == PlayerWizard.HumanID()))
                        {
                            this.SetBasicMarker(group.GetPosition(), MarkerType.Friendly, true);
                        }
                    }
                }
                List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(plane);
                if (locationsOfThePlane != null)
                {
                    foreach (Location location in locationsOfThePlane)
                    {
                        if (location.GetOwnerID() == PlayerWizard.HumanID())
                        {
                            this.SetBasicMarker(location.GetPosition(), MarkerType.Friendly, true);
                        }
                    }
                }
            }
        }

        private void MarkersUpdate(object sender, object e)
        {
            if ((!(sender is IPlanePosition) || (((sender as IPlanePosition).GetPlane() != null) && ReferenceEquals((sender as IPlanePosition).GetPlane(), this.plane))) && ReferenceEquals(World.GetActivePlane(), this.plane))
            {
                this.MarkersUpdate(this.plane);
            }
        }

        public void RequestUpdate(MarkerType t)
        {
            this.requiredUpdate.Add(t);
        }

        private void SelectionUpdated(object sender, object e)
        {
            if (this.movementAreaLocations != null)
            {
                this.GeneralUpdateMarkers(sender, e);
            }
        }

        public void SetAdvancedMarker(Vector3i pos, MarkerType type, bool visible, bool[] directionalData, UnityEngine.Color color)
        {
            int num;
            int num2;
            Vector2i vectori = this.HexToPixel(pos) * 2;
            if ((vectori.x < 0) || (vectori.y < 0))
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
            UnityEngine.Color pixel = this.dataTexture.GetPixel(vectori.x + 1, vectori.y);
            UnityEngine.Color color3 = this.dataTexture.GetPixel(vectori.x, vectori.y + 1);
            UnityEngine.Color color4 = this.dataTexture.GetPixel(vectori.x + 1, vectori.y + 1);
            UnityEngine.Color color5 = pixel;
            UnityEngine.Color color6 = color3;
            UnityEngine.Color color7 = color4;
            this.GetlayoutType(out num, out num2, directionalData, -1);
            if (type > MarkerType.PathWithMP)
            {
                if (type == MarkerType.MovementBorders)
                {
                    if (visible && (num2 > -1))
                    {
                        pixel.b = ((float) (num2 + 0x19)) / ((float) atlasSize);
                        color3.b = ((float) num) / 6f;
                    }
                    else
                    {
                        pixel.b = 0f;
                        color3.b = 0f;
                    }
                }
                else if (type == MarkerType.Roads)
                {
                    if (visible && (num2 > -1))
                    {
                        pixel.a = ((float) (num2 + 0x26)) / ((float) atlasSize);
                        color3.a = ((float) num) / 6f;
                    }
                    else
                    {
                        pixel.a = 0f;
                        color3.a = 0f;
                    }
                }
                else if (type == MarkerType.Roads2)
                {
                    if (visible && (num2 > -1))
                    {
                        pixel.a = ((float) (num2 + 0x33)) / ((float) atlasSize);
                        color3.a = ((float) num) / 6f;
                    }
                    else
                    {
                        pixel.a = 0f;
                        color3.a = 0f;
                    }
                }
            }
            else
            {
                if (type != MarkerType.Path)
                {
                    if (type == MarkerType.Borders)
                    {
                        if (!visible || (num2 <= -1))
                        {
                            pixel.g = 0f;
                            color3.g = 0f;
                        }
                        else
                        {
                            pixel.g = ((float) (num2 + 12)) / ((float) atlasSize);
                            color3.g = ((float) num) / 6f;
                            color4 = color;
                        }
                        goto TR_0006;
                    }
                    else if (type != MarkerType.PathWithMP)
                    {
                        goto TR_0006;
                    }
                }
                if (visible && (num2 > -1))
                {
                    pixel.r = ((float) (num2 + type)) / ((float) atlasSize);
                    color3.r = ((float) num) / 6f;
                }
                else
                {
                    pixel.r = 0f;
                    color3.r = 0f;
                }
            }
        TR_0006:
            if (color5 != pixel)
            {
                this.dataTexture.SetPixel(vectori.x + 1, vectori.y, pixel);
                this.dirty = true;
            }
            if (color6 != color3)
            {
                this.dataTexture.SetPixel(vectori.x, vectori.y + 1, color3);
                this.dirty = true;
            }
            if (color7 != color4)
            {
                this.dataTexture.SetPixel(vectori.x + 1, vectori.y + 1, color4);
                this.dirty = true;
            }
        }

        public void SetBasicMarker(Vector3i pos, MarkerType type, bool visible)
        {
            Vector2i vectori = this.HexToPixel(pos) * 2;
            if ((vectori.x >= 0) && (vectori.y >= 0))
            {
                if (!visible && this.typeToPosition[type].Contains(pos))
                {
                    this.typeToPosition[type].Remove(pos);
                }
                else if (visible && !this.typeToPosition[type].Contains(pos))
                {
                    this.typeToPosition[type].Add(pos);
                }
                UnityEngine.Color pixel = this.dataTexture.GetPixel(vectori.x, vectori.y);
                switch (type)
                {
                    case MarkerType.Highlight:
                        pixel.r = !visible ? 0f : (5f / ((float) atlasSize));
                        break;

                    case MarkerType.HexHighlight:
                        pixel.g = !visible ? 0f : (6f / ((float) atlasSize));
                        break;

                    case MarkerType.Friendly:
                        pixel.b = !visible ? 0f : (7f / ((float) atlasSize));
                        break;

                    case MarkerType.Embark:
                        pixel.a = !visible ? 0f : (8f / ((float) atlasSize));
                        break;

                    default:
                        break;
                }
                this.dataTexture.SetPixel(vectori.x, vectori.y, pixel);
                this.dirty = true;
            }
        }

        public void ShowMovementArea(List<Vector3i> area)
        {
            bool flag = true;
            if (this.movementAreaLocations != null)
            {
                flag = false;
                if (area.Count != this.movementAreaLocations.Count)
                {
                    flag = true;
                }
                else
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
                if (flag)
                {
                    this.HideMovementArea(false);
                }
            }
            if (area.Count > 0)
            {
                if (flag)
                {
                    bool[] directionalData = new bool[6];
                    WorldCode.Plane activePlane = World.GetActivePlane();
                    for (int i = 0; i < area.Count; i++)
                    {
                        if ((activePlane == null) || (activePlane.area.horizontalWrap || activePlane.area.IsInside(area[i], true)))
                        {
                            int index = 0;
                            while (true)
                            {
                                if (index >= 6)
                                {
                                    this.SetAdvancedMarker(area[i], MarkerType.MovementBorders, true, directionalData, UnityEngine.Color.white);
                                    break;
                                }
                                Vector3i pos = area[i] + HexNeighbors.neighbours[index];
                                if (activePlane != null)
                                {
                                    pos = activePlane.area.KeepHorizontalInside(pos);
                                }
                                directionalData[index] = !area.Contains(pos);
                                index++;
                            }
                        }
                    }
                }
                this.movementAreaLocations = area;
            }
        }

        public unsafe void ShowPath(List<Vector3i> path, RequestDataV2 rd)
        {
            IPlanePosition selectedUnit = null;
            FInt mp = FInt.ONE * 0x3e8;
            FInt num2 = FInt.ONE * 0x3e8;
            if (this.plane.battlePlane)
            {
                selectedUnit = BattleHUD.GetSelectedUnit();
                mp = (selectedUnit as BattleUnit).Mp;
            }
            else if (FSMSelectionManager.Get() != null)
            {
                Group selectedGroup = FSMSelectionManager.Get().GetSelectedGroup() as Group;
                mp = selectedGroup.CurentMP();
                num2 = new FInt(selectedGroup.GetMaxMP());
            }
            bool flag = true;
            if (this.pathLocations != null)
            {
                flag = false;
                if (path.Count != this.pathLocations.Count)
                {
                    flag = true;
                }
                else
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
                if (flag)
                {
                    this.HidePath(false);
                }
            }
            if ((path.Count > 1) && (num2 > 0))
            {
                if (flag)
                {
                    FInt num5;
                    int num4 = 1;
                    if (mp == 0)
                    {
                        num5 = num2;
                    }
                    bool[] directionalData = new bool[6];
                    Vector3i zero = Vector3i.zero;
                    Vector3i vectori2 = Vector3i.zero;
                    bool flag2 = false;
                    bool roadBuildingMode = FSMSelectionManager.Get().roadBuildingMode;
                    int num6 = 0;
                    while (true)
                    {
                        if (num6 >= path.Count)
                        {
                            if (!this.plane.battlePlane && ((num4 > 1) || (mp == 0)))
                            {
                                this.destination = path[path.Count - 1];
                                this.turns = num4;
                                VerticalMarkerManager.Get().DestroyMarker(this, true);
                                if (!roadBuildingMode)
                                {
                                    VerticalMarkerManager.Get().Addmarker(this);
                                }
                            }
                            break;
                        }
                        if (flag2)
                        {
                            num4++;
                        }
                        if (num6 > 0)
                        {
                            FInt oNE = rd.GetEntryCost(path[num6 - 1], path[num6], false, null);
                            if (oNE < 0)
                            {
                                oNE = FInt.ONE;
                            }
                            if ((num5 - oNE) <= 0)
                            {
                                if (num6 == (path.Count - 1))
                                {
                                    flag2 = false;
                                }
                                num5 = num2;
                            }
                        }
                        if (num6 > 0)
                        {
                            zero = selectedUnit.GetPlane().pathfindingArea.KeepHorizontalInside(path[num6 - 1] - path[num6]);
                            if (zero.x > 1)
                            {
                                short* numPtr1 = &zero.x;
                                numPtr1[0] = (short) (numPtr1[0] - 80);
                                short* numPtr2 = &zero.y;
                                numPtr2[0] = (short) (numPtr2[0] + 40);
                                short* numPtr3 = &zero.z;
                                numPtr3[0] = (short) (numPtr3[0] + 40);
                            }
                            else if (zero.x < -1)
                            {
                                short* numPtr4 = &zero.x;
                                numPtr4[0] = (short) (numPtr4[0] + 80);
                                short* numPtr5 = &zero.y;
                                numPtr5[0] = (short) (numPtr5[0] - 40);
                                short* numPtr6 = &zero.z;
                                numPtr6[0] = (short) (numPtr6[0] - 40);
                            }
                        }
                        if (num6 < (path.Count - 1))
                        {
                            vectori2 = selectedUnit.GetPlane().pathfindingArea.KeepHorizontalInside(path[num6 + 1] - path[num6]);
                            if (vectori2.x > 1)
                            {
                                short* numPtr7 = &vectori2.x;
                                numPtr7[0] = (short) (numPtr7[0] - 80);
                                short* numPtr8 = &vectori2.y;
                                numPtr8[0] = (short) (numPtr8[0] + 40);
                                short* numPtr9 = &vectori2.z;
                                numPtr9[0] = (short) (numPtr9[0] + 40);
                            }
                            else if (vectori2.x < -1)
                            {
                                short* numPtr10 = &vectori2.x;
                                numPtr10[0] = (short) (numPtr10[0] + 80);
                                short* numPtr11 = &vectori2.y;
                                numPtr11[0] = (short) (numPtr11[0] - 40);
                                short* numPtr12 = &vectori2.z;
                                numPtr12[0] = (short) (numPtr12[0] - 40);
                            }
                        }
                        int index = 0;
                        while (true)
                        {
                            if (index >= 6)
                            {
                                MarkerType type = (!flag2 || roadBuildingMode) ? MarkerType.Path : MarkerType.PathWithMP;
                                UnityEngine.Color color = new UnityEngine.Color();
                                this.SetAdvancedMarker(path[num6], type, true, directionalData, color);
                                num6++;
                                break;
                            }
                            directionalData[index] = ((num6 > 0) && (HexNeighbors.neighbours[index] == zero)) || ((num6 < (path.Count - 1)) && (HexNeighbors.neighbours[index] == vectori2));
                            index++;
                        }
                    }
                }
                this.pathLocations = path;
            }
        }

        public bool Update(bool recalculateMarkers)
        {
            IPlanePosition selectedGroup = null;
            if (recalculateMarkers)
            {
                this.GeneralUpdateMarkers(false, false);
            }
            if (!this.plane.battlePlane)
            {
                if (FSMSelectionManager.Get() != null)
                {
                    selectedGroup = FSMSelectionManager.Get().GetSelectedGroup();
                }
            }
            else
            {
                selectedGroup = BattleHUD.GetSelectedUnit();
                if (FSMBattleTurn.IsCastingSpells())
                {
                    this.HidePath(true);
                    return false;
                }
            }
            if (selectedGroup == null)
            {
                return false;
            }
            if ((!(selectedGroup is IGroup) || ((selectedGroup as IGroup).GetOwnerID() != PlayerWizard.HumanID())) && (!(selectedGroup is BattleUnit) || ((selectedGroup as BattleUnit).ownerID != PlayerWizard.HumanID())))
            {
                this.HidePath(true);
            }
            else
            {
                Group group = selectedGroup as Group;
                Vector3 worldPos = CameraController.GetClickWorldPosition(true, true, true);
                Vector3i invalid = Vector3i.invalid;
                if ((group != null) && (group.destination != Vector3i.invalid))
                {
                    invalid = group.destination;
                }
                else if (worldPos != -Vector3.one)
                {
                    invalid = HexCoordinates.GetHexCoordAt(worldPos);
                }
                if (this.plane.battlePlane && (invalid != Vector3i.invalid))
                {
                    BattleUnit unitAt = Battle.GetBattle().GetUnitAt(invalid);
                    if (unitAt != null)
                    {
                        if (unitAt.ownerID == (selectedGroup as BattleUnit).ownerID)
                        {
                            invalid = Vector3i.invalid;
                        }
                        BattleUnit attacker = selectedGroup as BattleUnit;
                        if ((attacker != null) && (attacker.currentlyVisible && (Battle.AttackFormPossible(attacker, unitAt, -1) != Battle.AttackForm.eNone)))
                        {
                            invalid = Vector3i.invalid;
                        }
                    }
                }
                if (!(invalid != Vector3i.invalid))
                {
                    this.HidePath(true);
                }
                else
                {
                    List<Vector3i> path = null;
                    RequestDataV2 rd = null;
                    if (!(selectedGroup is Location) && ((selectedGroup.GetPosition() != invalid) && ((selectedGroup.GetPlane() != null) && selectedGroup.GetPlane().pathfindingArea.IsInside(invalid, false))))
                    {
                        rd = RequestDataV2.CreateRequest(selectedGroup.GetPlane(), selectedGroup.GetPosition(), invalid, selectedGroup, false, false, false);
                        rd.allowAllyPassMode = !FSMSelectionManager.Get().roadBuildingMode;
                        rd.water = rd.water && !FSMSelectionManager.Get().roadBuildingMode;
                        rd.nonCorporeal = rd.nonCorporeal && !FSMSelectionManager.Get().roadBuildingMode;
                        PathfinderV2.FindPath(rd);
                        path = rd.GetPath();
                    }
                    if (path == null)
                    {
                        this.HidePath(true);
                    }
                    else
                    {
                        this.ShowPath(path, rd);
                    }
                }
            }
            if (!this.dirty)
            {
                return false;
            }
            this.dataTexture.Apply();
            this.dirty = false;
            return true;
        }

        public void UpdateMarkers()
        {
            if ((this.dataTexture != null) && this.dirty)
            {
                this.dirty = false;
                this.dataTexture.Apply();
            }
        }

        public enum MarkerType
        {
            Highlight = 5,
            Friendly = 7,
            HexHighlight = 6,
            Embark = 8,
            Path = 1,
            Borders = 12,
            MovementBorders = 0x19,
            Roads = 0x26,
            Roads2 = 0x33,
            PathWithMP = 0x10
        }
    }
}

