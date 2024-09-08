namespace WorldCode
{
    using DBDef;
    using MHUtils;
    using MOM;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SearcherDataV2
    {
        public const int neighbourDataStep = 12;
        public const int neighbourStep = 2;
        public const int neighbourIndexOffset = 0;
        public const int neighbourVRiverOffset = 1;
        public int width;
        public int height;
        public bool wrapping;
        public PathNode[] nodes;
        public int[] nodeNeighbours;
        public bool[] locations;
        public int[] units;
        public bool[] walls;
        public int gateIndex;
        public FInt maxTerrainCost;
        public int iteration;

        public static FInt ClampMPCost(SearcherDataV2 data, FInt cost)
        {
            return ((data != null) ? ((cost <= data.maxTerrainCost) ? cost : data.maxTerrainCost) : cost);
        }

        public void ClearUnitPosition(Vector3i pos)
        {
            if (pos != Vector3i.invalid)
            {
                int index = this.GetIndex(pos);
                this.units[index] = 0;
            }
        }

        public static int GetCDOwnerID(int comboData)
        {
            return ((comboData >= 0) ? (comboData & 15) : (((comboData == -1) || (comboData == -11)) ? -1 : (((comboData == -2) || (comboData == -12)) ? -2 : -1)));
        }

        public static int GetCDUnitCount(int comboData)
        {
            return ((comboData >= 0) ? ((comboData >> 0x10) & 15) : -1);
        }

        public int GetColumn(Vector3i pos)
        {
            int num = (-this.width / 2) + 1;
            return (pos.x - num);
        }

        public int GetGroupSizeOwnerCombo(MOM.Group g)
        {
            IPlanePosition selectedGroup;
            int ownerID = g.GetOwnerID();
            if (ownerID > 15)
            {
                Debug.LogError("Wizard ID larger than 0xF !!!");
            }
            int count = g.GetUnits().Count;
            FSMSelectionManager manager1 = FSMSelectionManager.Get();
            if (manager1 != null)
            {
                selectedGroup = manager1.GetSelectedGroup();
            }
            else
            {
                FSMSelectionManager local1 = manager1;
                selectedGroup = null;
            }
            if (ReferenceEquals(selectedGroup, g) && ((FSMSelectionManager.Get().selectedUnits != null) && (FSMSelectionManager.Get().selectedUnits.Count > 0)))
            {
                count = FSMSelectionManager.Get().selectedUnits.Count;
            }
            int num3 = g.IsGroupInvisible() ? 1 : 0;
            bool flag = (g.transporter != null) && g.waterMovement;
            return ((((((((g.transporter != null) && g.landMovement) ? 0x200000 : 0) | (flag ? 0x100000 : 0)) | (count << 0x10)) | (num3 << 15)) | 0x4000) | ownerID);
        }

        public int GetIndex(Vector3i p)
        {
            return ((this.width * this.GetRow(p)) + this.GetColumn(p));
        }

        public int GetRow(Vector3i pos)
        {
            int num = -((this.height / 2) - 1);
            int num3 = ((this.width / 4) + num) + (-(this.GetColumn(pos) + 1) / 2);
            return (pos.y - num3);
        }

        public static bool HasTransporter(int comboData, bool water)
        {
            return ((comboData >= 0) ? ((!water || ((comboData & 0x100000) <= 0)) ? (!water && ((comboData & 0x200000) > 0)) : true) : false);
        }

        public void InitializeData(WorldCode.Plane p)
        {
            V3iRect pathfindingArea = p.pathfindingArea;
            this.width = pathfindingArea.AreaWidth;
            this.height = pathfindingArea.AreaHeight;
            this.wrapping = p.pathfindingArea.horizontalWrap;
            this.nodes = new PathNode[this.width * this.height];
            this.nodeNeighbours = new int[(12 * this.width) * this.height];
            if (!p.battlePlane)
            {
                this.maxTerrainCost = new FInt(100);
            }
            else
            {
                string str = DifficultySettingsData.GetSetting("UI_BATTLE_MP_TERRAIN_COSTS").value;
                this.maxTerrainCost = (str == "1") ? FInt.ONE : ((str == "2") ? new FInt(2) : ((str == "3") ? new FInt(100) : new FInt(100)));
            }
            foreach (KeyValuePair<Vector3i, Hex> pair in p.GetHexes())
            {
                PathNode node = new PathNode();
                node.UpdateBaseData(pair.Key, this);
                node.UpdateTerrainData(pair.Value, pair.Value.MovementCost());
                this.nodes[node.index] = node;
                this.PrepareNeighbours(p.pathfindingArea, pair.Value, node.index);
            }
        }

        public void InitializeLocationsAndUnits(Battle b)
        {
            int num = b.plane.searcherIteration + 1;
            b.plane.searcherIteration = num;
            this.iteration = num;
            if (this.locations == null)
                this.locations = new bool[this.nodes.Length];
            if ((this.units != null) && (this.units.Length == this.nodes.Length))
            {
                Array.Clear(this.units, 0, this.units.Length);
            }
            else
            {
                this.units = new int[this.nodes.Length];
            }
            foreach (BattleUnit unit in b.GetUnits(true))
            {
                int index = this.GetIndex(unit.GetPosition());
                if (unit.IsAlive())
                {
                    this.units[index] = (unit.currentlyVisible || !unit.IsInvisibleUnit()) ? -1 : -11;
                }
            }
            foreach (BattleUnit unit2 in b.GetUnits(false))
            {
                int index = this.GetIndex(unit2.GetPosition());
                if (unit2.IsAlive())
                {
                    this.units[index] = (unit2.currentlyVisible || !unit2.IsInvisibleUnit()) ? -2 : -12;
                }
            }
            if ((b.battleWalls == null) || (b.battleWalls.Count <= 0))
            {
                this.walls = null;
            }
            else
            {
                if ((this.walls != null) && (this.walls.Length == this.nodes.Length))
                {
                    Array.Clear(this.walls, 0, this.walls.Length);
                }
                else
                {
                    this.walls = new bool[this.nodes.Length];
                }
                this.gateIndex = this.GetIndex(b.battleWalls[0].position);
                for (int i = 0; i < b.battleWalls.Count; i++)
                {
                    if (b.battleWalls[i].standing)
                    {
                        int index = this.GetIndex(b.battleWalls[i].position);
                        this.walls[index] = true;
                    }
                }
            }
        }

        public void InitializeLocationsAndUnits(WorldCode.Plane p)
        {
            int num = p.searcherIteration + 1;
            p.searcherIteration = num;
            this.iteration = num;
            if (this.locations == null)
            {
                this.locations = new bool[this.nodes.Length];
            }
            else
            {
                Array.Clear(this.locations, 0, this.locations.Length);
            }
            if (this.units == null)
            {
                this.units = new int[this.nodes.Length];
            }
            else
            {
                Array.Clear(this.units, 0, this.units.Length);
            }
            List<MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(p);
            if (locationsOfThePlane != null)
            {
                for (int i = 0; i < locationsOfThePlane.Count; i++)
                {
                    MOM.Location location = locationsOfThePlane[i];
                    int index = this.GetIndex(location.Position);
                    this.locations[index] = true;
                    if ((location.locationType == ELocationType.PlaneTower) && !ReferenceEquals(location.GetGroup().GetPlane(), p))
                    {
                        this.units[index] = this.GetGroupSizeOwnerCombo(location.GetGroup());
                    }
                }
            }
            List<MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(p);
            if (groupsOfPlane != null)
            {
                for (int i = 0; i < groupsOfPlane.Count; i++)
                {
                    MOM.Group g = groupsOfPlane[i];
                    int index = this.GetIndex(g.GetPosition());
                    this.units[index] = this.GetGroupSizeOwnerCombo(g);
                }
            }
        }

        public static bool IsCDVisible(int comboData, bool attackerSide)
        {
            return ((comboData >= 0) ? ((comboData & 0x8000) == 0) : ((comboData < -2) ? (!((comboData == -11) & attackerSide) ? ((comboData == -12) && !attackerSide) : true) : true));
        }

        public bool IsUnitAt(Vector3i pos)
        {
            int index = this.GetIndex(pos);
            return (((this.locations == null) || !this.locations[index]) ? ((this.units != null) && (this.units[index] != 0)) : true);
        }

        public void PrepareNeighbours(V3iRect area, Hex h, int hIndex)
        {
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i vectori = HexNeighbors.neighbours[i];
                int index = (hIndex * 12) + (i * 2);
                Vector3i pos = h.Position + vectori;
                if (this.wrapping)
                {
                    int num3 = this.width / 2;
                    if ((pos.x <= -num3) || (pos.x > num3))
                    {
                        pos = Vector3i.WrapByWidth(pos, this.width);
                    }
                }
                if (!area.IsInside(pos, false))
                {
                    this.nodeNeighbours[index] = -1;
                }
                else
                {
                    this.nodeNeighbours[index] = this.GetIndex(pos);
                    if ((h.viaRiver != null) && h.viaRiver[i])
                    {
                        this.nodeNeighbours[index + 1] = 1;
                    }
                }
            }
        }

        public void UpdateGroupToPosition(Vector3i pos, MOM.Group g)
        {
            if ((pos != Vector3i.invalid) && g.alive)
            {
                int index = this.GetIndex(pos);
                this.units[index] = this.GetGroupSizeOwnerCombo(g);
            }
        }

        internal void UpdateUnitPosition(BattleUnit battleUnit)
        {
            if ((battleUnit.GetPosition() != Vector3i.invalid) && battleUnit.IsAlive())
            {
                int index = this.GetIndex(battleUnit.GetPosition());
                if (battleUnit.currentlyVisible)
                {
                    this.units[index] = -1;
                }
                else
                {
                    this.units[index] = battleUnit.attackingSide ? -2 : -3;
                }
            }
        }

        public void UpdateUnitPosition(Vector3i atPos, MOM.Group g)
        {
            if ((atPos != Vector3i.invalid) && g.alive)
            {
                int groupSizeOwnerCombo = 0;
                if (g != null)
                {
                    groupSizeOwnerCombo = this.GetGroupSizeOwnerCombo(g);
                }
                int index = this.GetIndex(atPos);
                this.units[index] = groupSizeOwnerCombo;
            }
        }

        public void UpdateUnitPosition(Vector3i from, Vector3i to, IPlanePosition ipp)
        {
            int groupSizeOwnerCombo = 0;
            if (from != Vector3i.invalid)
            {
                MOM.Location local2;
                int index = this.GetIndex(from);
                groupSizeOwnerCombo = this.units[index];
                List<MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(ipp.GetPlane());
                if (locationsOfThePlane != null)
                {
                    local2 = locationsOfThePlane.Find(o => o.GetPosition() == ipp.GetPosition());
                }
                else
                {
                    List<MOM.Location> local1 = locationsOfThePlane;
                    local2 = null;
                }
                MOM.Location location = local2;
                this.units[index] = (location == null) ? 0 : this.GetGroupSizeOwnerCombo(location.GetGroup());
            }
            if (to != Vector3i.invalid)
            {
                if (ipp is MOM.Group)
                {
                    groupSizeOwnerCombo = this.GetGroupSizeOwnerCombo(ipp as MOM.Group);
                }
                if (!(ipp is MOM.Group) || (ipp as MOM.Group).alive)
                {
                    if (groupSizeOwnerCombo == 0)
                    {
                        Debug.LogError("value 0 not expected");
                    }
                    int index = this.GetIndex(to);
                    this.units[index] = groupSizeOwnerCombo;
                }
            }
        }
    }
}

