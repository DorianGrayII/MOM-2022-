using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;

namespace WorldCode
{
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

        public void InitializeData(Plane p)
        {
            V3iRect pathfindingArea = p.pathfindingArea;
            this.width = pathfindingArea.AreaWidth;
            this.height = pathfindingArea.AreaHeight;
            this.wrapping = p.pathfindingArea.horizontalWrap;
            this.nodes = new PathNode[this.width * this.height];
            this.nodeNeighbours = new int[12 * this.width * this.height];
            if (p.battlePlane)
            {
                switch (DifficultySettingsData.GetSetting("UI_BATTLE_MP_TERRAIN_COSTS").value)
                {
                case "1":
                    this.maxTerrainCost = FInt.ONE;
                    break;
                case "2":
                    this.maxTerrainCost = new FInt(2);
                    break;
                case "3":
                    this.maxTerrainCost = new FInt(100);
                    break;
                default:
                    this.maxTerrainCost = new FInt(100);
                    break;
                }
            }
            else
            {
                this.maxTerrainCost = new FInt(100);
            }
            foreach (KeyValuePair<Vector3i, Hex> hex in p.GetHexes())
            {
                PathNode pathNode = default(PathNode);
                pathNode.UpdateBaseData(hex.Key, this);
                pathNode.UpdateTerrainData(hex.Value, hex.Value.MovementCost());
                this.nodes[pathNode.index] = pathNode;
                this.PrepareNeighbours(p.pathfindingArea, hex.Value, pathNode.index);
            }
        }

        public void PrepareNeighbours(V3iRect area, Hex h, int hIndex)
        {
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i vector3i = HexNeighbors.neighbours[i];
                int num = hIndex * 12;
                num += i * 2;
                Vector3i vector3i2 = h.Position + vector3i;
                if (this.wrapping)
                {
                    int num2 = this.width / 2;
                    if (vector3i2.x <= -num2 || vector3i2.x > num2)
                    {
                        vector3i2 = Vector3i.WrapByWidth(vector3i2, this.width);
                    }
                }
                if (area.IsInside(vector3i2))
                {
                    this.nodeNeighbours[num] = this.GetIndex(vector3i2);
                    if (h.viaRiver != null && h.viaRiver[i])
                    {
                        this.nodeNeighbours[num + 1] = 1;
                    }
                }
                else
                {
                    this.nodeNeighbours[num] = -1;
                }
            }
        }

        public int GetColumn(Vector3i pos)
        {
            int num = -this.width / 2 + 1;
            return pos.x - num;
        }

        public int GetRow(Vector3i pos)
        {
            int num = -(this.height / 2 - 1);
            int num2 = this.width / 4 + num;
            int num3 = -(this.GetColumn(pos) + 1) / 2;
            int num4 = num2 + num3;
            return pos.y - num4;
        }

        public int GetIndex(Vector3i p)
        {
            return this.width * this.GetRow(p) + this.GetColumn(p);
        }

        public void InitializeLocationsAndUnits(Plane p)
        {
            this.iteration = ++p.searcherIteration;
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
            List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(p);
            if (locationsOfThePlane != null)
            {
                for (int i = 0; i < locationsOfThePlane.Count; i++)
                {
                    global::MOM.Location location = locationsOfThePlane[i];
                    int index = this.GetIndex(location.Position);
                    this.locations[index] = true;
                    if (location.locationType == ELocationType.PlaneTower && location.GetGroup().GetPlane() != p)
                    {
                        this.units[index] = this.GetGroupSizeOwnerCombo(location.GetGroup());
                    }
                }
            }
            List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(p);
            if (groupsOfPlane != null)
            {
                for (int j = 0; j < groupsOfPlane.Count; j++)
                {
                    global::MOM.Group group = groupsOfPlane[j];
                    int index2 = this.GetIndex(group.GetPosition());
                    this.units[index2] = this.GetGroupSizeOwnerCombo(group);
                }
            }
        }

        public void InitializeLocationsAndUnits(Battle b)
        {
            this.iteration = ++b.plane.searcherIteration;
            if (this.locations == null)
            {
                this.locations = new bool[this.nodes.Length];
            }
            if (this.units == null || this.units.Length != this.nodes.Length)
            {
                this.units = new int[this.nodes.Length];
            }
            else
            {
                Array.Clear(this.units, 0, this.units.Length);
            }
            foreach (BattleUnit unit in b.GetUnits(attacker: true))
            {
                int index = this.GetIndex(unit.GetPosition());
                if (unit.IsAlive())
                {
                    this.units[index] = ((!unit.currentlyVisible && unit.IsInvisibleUnit()) ? (-11) : (-1));
                }
            }
            foreach (BattleUnit unit2 in b.GetUnits(attacker: false))
            {
                int index2 = this.GetIndex(unit2.GetPosition());
                if (unit2.IsAlive())
                {
                    this.units[index2] = ((!unit2.currentlyVisible && unit2.IsInvisibleUnit()) ? (-12) : (-2));
                }
            }
            if (b.battleWalls != null && b.battleWalls.Count > 0)
            {
                if (this.walls == null || this.walls.Length != this.nodes.Length)
                {
                    this.walls = new bool[this.nodes.Length];
                }
                else
                {
                    Array.Clear(this.walls, 0, this.walls.Length);
                }
                this.gateIndex = this.GetIndex(b.battleWalls[0].position);
                for (int i = 0; i < b.battleWalls.Count; i++)
                {
                    if (b.battleWalls[i].standing)
                    {
                        int index3 = this.GetIndex(b.battleWalls[i].position);
                        this.walls[index3] = true;
                    }
                }
            }
            else
            {
                this.walls = null;
            }
        }

        public int GetGroupSizeOwnerCombo(global::MOM.Group g)
        {
            int ownerID = g.GetOwnerID();
            if (ownerID > 15)
            {
                Debug.LogError("Wizard ID larger than 0xF !!!");
            }
            int count = g.GetUnits().Count;
            if (FSMSelectionManager.Get()?.GetSelectedGroup() == g && FSMSelectionManager.Get().selectedUnits != null && FSMSelectionManager.Get().selectedUnits.Count > 0)
            {
                count = FSMSelectionManager.Get().selectedUnits.Count;
            }
            int num = (g.IsGroupInvisible() ? 1 : 0);
            bool flag = g.transporter != null && g.waterMovement;
            return ((g.transporter != null && g.landMovement) ? 2097152 : 0) | (flag ? 1048576 : 0) | (count << 16) | (num << 15) | 0x4000 | ownerID;
        }

        public static bool IsCDVisible(int comboData, bool attackerSide)
        {
            if (comboData < 0)
            {
                if (comboData >= -2)
                {
                    return true;
                }
                if (comboData == -11 && attackerSide)
                {
                    return true;
                }
                if (comboData == -12 && !attackerSide)
                {
                    return true;
                }
                return false;
            }
            return (comboData & 0x8000) == 0;
        }

        public static int GetCDOwnerID(int comboData)
        {
            if (comboData < 0)
            {
                switch (comboData)
                {
                case -11:
                case -1:
                    return -1;
                case -12:
                case -2:
                    return -2;
                default:
                    return -1;
                }
            }
            return comboData & 0xF;
        }

        public static int GetCDUnitCount(int comboData)
        {
            if (comboData < 0)
            {
                return -1;
            }
            return (comboData >> 16) & 0xF;
        }

        public static bool HasTransporter(int comboData, bool water)
        {
            if (comboData < 0)
            {
                return false;
            }
            if (water && (comboData & 0x100000) > 0)
            {
                return true;
            }
            if (!water && (comboData & 0x200000) > 0)
            {
                return true;
            }
            return false;
        }

        public void UpdateUnitPosition(Vector3i from, Vector3i to, IPlanePosition ipp)
        {
            int num = 0;
            if (from != Vector3i.invalid)
            {
                int index = this.GetIndex(from);
                num = this.units[index];
                global::MOM.Location location = GameManager.GetLocationsOfThePlane(ipp.GetPlane())?.Find((global::MOM.Location o) => o.GetPosition() == ipp.GetPosition());
                if (location != null)
                {
                    this.units[index] = this.GetGroupSizeOwnerCombo(location.GetGroup());
                }
                else
                {
                    this.units[index] = 0;
                }
            }
            if (!(to != Vector3i.invalid))
            {
                return;
            }
            if (ipp is global::MOM.Group)
            {
                num = this.GetGroupSizeOwnerCombo(ipp as global::MOM.Group);
            }
            if (!(ipp is global::MOM.Group) || (ipp as global::MOM.Group).alive)
            {
                if (num == 0)
                {
                    Debug.LogError("value 0 not expected");
                }
                int index2 = this.GetIndex(to);
                this.units[index2] = num;
            }
        }

        public void UpdateUnitPosition(Vector3i atPos, global::MOM.Group g)
        {
            if (atPos != Vector3i.invalid && g.alive)
            {
                int num = 0;
                if (g != null)
                {
                    num = this.GetGroupSizeOwnerCombo(g);
                }
                int index = this.GetIndex(atPos);
                this.units[index] = num;
            }
        }

        internal void UpdateUnitPosition(BattleUnit battleUnit)
        {
            if (battleUnit.GetPosition() != Vector3i.invalid && battleUnit.IsAlive())
            {
                int index = this.GetIndex(battleUnit.GetPosition());
                if (battleUnit.currentlyVisible)
                {
                    this.units[index] = -1;
                }
                else
                {
                    this.units[index] = (battleUnit.attackingSide ? (-2) : (-3));
                }
            }
        }

        public void ClearUnitPosition(Vector3i pos)
        {
            if (pos != Vector3i.invalid)
            {
                int index = this.GetIndex(pos);
                this.units[index] = 0;
            }
        }

        public void UpdateGroupToPosition(Vector3i pos, global::MOM.Group g)
        {
            if (pos != Vector3i.invalid && g.alive)
            {
                int index = this.GetIndex(pos);
                this.units[index] = this.GetGroupSizeOwnerCombo(g);
            }
        }

        public bool IsUnitAt(Vector3i pos)
        {
            int index = this.GetIndex(pos);
            if ((this.locations != null && this.locations[index]) || (this.units != null && this.units[index] != 0))
            {
                return true;
            }
            return false;
        }

        public static FInt ClampMPCost(SearcherDataV2 data, FInt cost)
        {
            if (data == null)
            {
                return cost;
            }
            if (cost > data.maxTerrainCost)
            {
                return data.maxTerrainCost;
            }
            return cost;
        }
    }
}
