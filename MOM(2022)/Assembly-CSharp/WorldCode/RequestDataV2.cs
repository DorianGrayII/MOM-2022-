using System.Collections.Generic;
using System.Text;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;

namespace WorldCode
{
    public class RequestDataV2
    {
        public enum MPCost
        {
            eImpossible = 0,
            eNormal = 1,
            eCheap = 2,
            MAX = 3
        }

        public static int nextAvaliableID = 1;

        public int requestID;

        public Vector3i from;

        public Vector3i to;

        public int fromID;

        public int toID;

        public FInt mpRange;

        public bool pathFound;

        public bool water;

        public bool land;

        public bool mountain;

        public bool forest;

        public bool earthWalker;

        public bool nonCorporeal;

        public bool pathfinder;

        public bool attacker;

        public bool stoppedByGate;

        public bool ignoreWalls;

        public int movementPool = -1;

        public FInt activeTurnMovementPool;

        public int embarkCost = -1;

        public int activeGroupComboInfo;

        public SearcherDataV2 data;

        public RoadManager roadManager;

        public int optionsStart = -1;

        public int areaStart = -1;

        public int areaSize;

        public bool seekTargetsOnly;

        public StringBuilder debugLog;

        public V3iRect area;

        public Multitype<Vector3i, int>[] dangerPoints;

        public HashSet<Vector3i> exclusionPoints;

        public Battle battle;

        public bool avoidFirewall;

        public int groupValue;

        public HeatMap dangerMap;

        public List<Vector3i> getPathResult;

        public bool allowAllyPassMode = true;

        public bool avoidLandingOnTarget;

        public static RequestDataV2 CreateRequest(Plane p, Vector3i from, Vector3i to, IPlanePosition unit, bool allTerrain = false, bool avoidFirewall = false, bool movementFormBasedOnStartLocation = false)
        {
            RequestDataV2 requestDataV = new RequestDataV2();
            requestDataV.requestID = ++RequestDataV2.nextAvaliableID;
            requestDataV.from = from;
            requestDataV.to = p.area.KeepHorizontalInside(to);
            requestDataV.pathFound = false;
            requestDataV.area = p.area;
            requestDataV.exclusionPoints = p.exclusionPoints;
            requestDataV.data = p.GetSearcherData();
            requestDataV.roadManager = p.GetRoadManagers();
            requestDataV.FillMPCost(unit);
            if (p != null && p.battlePlane)
            {
                requestDataV.battle = Battle.GetBattle();
                if (requestDataV.battle != null)
                {
                    requestDataV.avoidFirewall = avoidFirewall && requestDataV.battle.fireWall;
                }
            }
            if (unit is global::MOM.Group)
            {
                requestDataV.activeGroupComboInfo = requestDataV.data.GetGroupSizeOwnerCombo(unit as global::MOM.Group);
            }
            else if (unit is BattleUnit)
            {
                BattleUnit battleUnit = unit as BattleUnit;
                requestDataV.attacker = battleUnit.attackingSide;
                requestDataV.activeGroupComboInfo = (requestDataV.attacker ? (-1) : (-2));
            }
            else
            {
                requestDataV.activeGroupComboInfo = 0;
            }
            if (movementFormBasedOnStartLocation && unit == null && !p.IsLand(from))
            {
                requestDataV.water = true;
                requestDataV.land = false;
            }
            if (allTerrain)
            {
                requestDataV.FillMPMultiterrain();
            }
            return requestDataV;
        }

        public static RequestDataV2 CreateRequest(Plane p, Vector3i from, FInt mpRange, IPlanePosition unit, bool avoidFirewall = false)
        {
            RequestDataV2 requestDataV = new RequestDataV2();
            requestDataV.requestID = ++RequestDataV2.nextAvaliableID;
            requestDataV.from = from;
            requestDataV.to = Vector3i.invalid;
            requestDataV.mpRange = mpRange;
            requestDataV.pathFound = false;
            requestDataV.area = p.area;
            requestDataV.exclusionPoints = p.exclusionPoints;
            requestDataV.data = p.GetSearcherData();
            requestDataV.roadManager = p.GetRoadManagers();
            requestDataV.FillMPCost(unit);
            if (p != null && p.battlePlane)
            {
                requestDataV.battle = Battle.GetBattle();
                if (requestDataV.battle != null)
                {
                    requestDataV.avoidFirewall = avoidFirewall && requestDataV.battle.fireWall;
                }
            }
            if (unit is global::MOM.Group)
            {
                requestDataV.activeGroupComboInfo = requestDataV.data.GetGroupSizeOwnerCombo(unit as global::MOM.Group);
            }
            else
            {
                requestDataV.activeGroupComboInfo = -1;
                if (unit is BattleUnit)
                {
                    BattleUnit battleUnit = unit as BattleUnit;
                    requestDataV.attacker = battleUnit.attackingSide;
                    requestDataV.activeGroupComboInfo = (requestDataV.attacker ? (-1) : (-2));
                }
            }
            return requestDataV;
        }

        public void MakeItAvoidanceSearch(global::MOM.Group p)
        {
            if (p == null)
            {
                Debug.LogError("Cannot add danger map for a search without group reference");
                return;
            }
            if (!DataHeatMaps.Get(p.GetPlane()).IsMapReady(DataHeatMaps.HMType.DangerMap, p.GetOwnerID(), createIfNotReady: false))
            {
                Debug.LogError("danger heat map need to be prepared before use!");
                return;
            }
            this.groupValue = p.GetValue();
            this.dangerMap = DataHeatMaps.Get(p.GetPlane()).GetHeatMap(DataHeatMaps.HMType.DangerMap);
        }

        private void FillMPMultiterrain()
        {
            this.water = true;
            this.land = true;
            this.mountain = true;
            this.forest = true;
            this.earthWalker = true;
            this.nonCorporeal = true;
            this.pathfinder = true;
            this.stoppedByGate = true;
        }

        public void BreakMovementByMPPool(int mpPool, FInt actTurnMovementPool)
        {
            this.movementPool = mpPool;
            this.activeTurnMovementPool = actTurnMovementPool;
        }

        public void AllowEmbarkAtCost(int embarkCost)
        {
            this.water = true;
            this.land = true;
            this.movementPool = 0;
            this.embarkCost = embarkCost;
        }

        public void MakePathOverWater()
        {
            this.water = true;
            this.land = false;
            this.mountain = false;
            this.forest = false;
            this.earthWalker = false;
            this.nonCorporeal = false;
            this.pathfinder = false;
            this.stoppedByGate = false;
        }

        private void FillMPCost(IPlanePosition unit)
        {
            if (unit is AIArmyResupply aIArmyResupply)
            {
                if (aIArmyResupply.group != null)
                {
                    this.FillMovementCost(aIArmyResupply.group.Get());
                }
                return;
            }
            if (unit is AIWarArmy aIWarArmy)
            {
                if (aIWarArmy.group != null)
                {
                    this.FillMovementCost(aIWarArmy.group.Get());
                }
                return;
            }
            if (unit is global::MOM.Group)
            {
                this.FillMovementCost(unit as global::MOM.Group);
                return;
            }
            if (unit is global::MOM.Location)
            {
                this.FillMovementCost(unit as global::MOM.Location);
                return;
            }
            if (unit is BattleUnit)
            {
                this.FillMovementCost(unit as BattleUnit);
                return;
            }
            this.water = false;
            this.land = true;
            this.mountain = true;
            this.forest = true;
            this.earthWalker = true;
            this.nonCorporeal = true;
            this.pathfinder = true;
            this.stoppedByGate = false;
        }

        private void FillMovementCost(global::MOM.Group g)
        {
            if (g.SubsetSelected())
            {
                g.UpdateMovementFlagsBySelection();
                this.water = g.waterMovement;
                this.land = g.landMovement;
                this.mountain = g.mountainMovement;
                this.forest = g.forestMovement;
                this.earthWalker = g.earthWalkerMovement;
                this.nonCorporeal = g.nonCorporealMovement;
                this.pathfinder = g.pathfinderMovement;
                g.UpdateMovementFlags();
            }
            else
            {
                this.water = g.waterMovement;
                this.land = g.landMovement;
                this.mountain = g.mountainMovement;
                this.forest = g.forestMovement;
                this.earthWalker = g.earthWalkerMovement;
                this.nonCorporeal = g.nonCorporealMovement;
                this.pathfinder = g.pathfinderMovement;
            }
            this.BreakMovementByMPPool(g.GetMaxMP(), g.CurentMP());
            this.stoppedByGate = false;
        }

        private void FillMovementCost(global::MOM.Location g)
        {
            this.water = false;
            this.land = false;
            this.mountain = false;
            this.forest = false;
            this.earthWalker = false;
            this.nonCorporeal = false;
            this.pathfinder = false;
            this.stoppedByGate = false;
        }

        private void FillMovementCost(BattleUnit g)
        {
            this.water = g.waterMovement;
            this.land = g.landMovement;
            this.mountain = g.mountainMovement;
            this.forest = g.forestMovement;
            this.earthWalker = g.earthWalkerMovement;
            this.nonCorporeal = g.nonCorporealMovement;
            this.stoppedByGate = g.attackingSide;
            this.activeTurnMovementPool = g.Mp;
        }

        public void SetDangerPoints(Multitype<Vector3i, int>[] dangers)
        {
            this.dangerPoints = dangers;
        }

        public void SetNode(int index, PathNode n)
        {
            this.data.nodes[index] = n;
        }

        public void SetGCost(int index, FInt value)
        {
            this.data.nodes[index].SetGCost(value);
        }

        public void SetDangerForNode(int index, int previousDanger)
        {
            if (this.dangerMap != null)
            {
                int value = this.dangerMap.GetValue(this.GetPosition(index));
                if (value < this.groupValue)
                {
                    this.data.nodes[index].totalDanger = previousDanger;
                }
                else
                {
                    this.data.nodes[index].totalDanger = previousDanger + (value - this.groupValue);
                }
            }
        }

        public void SetNextListIndex(int index, int value)
        {
            this.data.nodes[index].nextItem = value;
        }

        public void SetPrevListIndex(int index, int value)
        {
            this.data.nodes[index].prevItem = value;
        }

        public void SetOnTheList(int index, bool value)
        {
            this.data.nodes[index].isOnTheList = value;
        }

        public void SetNextAreaListIndex(int index, int value)
        {
            this.data.nodes[index].nextAreaItem = value;
        }

        public void SetPrevAreaListIndex(int index, int value)
        {
            this.data.nodes[index].prevAreaItem = value;
        }

        public void SetOnTheAreaList(int index, bool value)
        {
            this.data.nodes[index].isOnTheAreaList = value;
        }

        public void SetCameFrom(int index, int value)
        {
            this.data.nodes[index].cameFrom = value;
        }

        public void SetCameFromStepCount(int index, int value)
        {
            this.data.nodes[index].stepCount = value;
        }

        public PathNode GetNode(int index)
        {
            if (index < 0 || index >= this.data.nodes.Length)
            {
                return default(PathNode);
            }
            return this.data.nodes[index];
        }

        public FInt GetSumCost(int index)
        {
            return this.data.nodes[index].sumCost;
        }

        public FInt GetGCost(int index)
        {
            return this.data.nodes[index].gCost;
        }

        public int GetDangerSum(int index)
        {
            return this.data.nodes[index].totalDanger;
        }

        public int GetLocalDanger(int index)
        {
            if (this.dangerMap == null)
            {
                return 0;
            }
            int value = this.dangerMap.GetValue(this.GetPosition(index));
            if (value < this.groupValue)
            {
                return 0;
            }
            return value;
        }

        public int GetNextListIndex(int index)
        {
            return this.data.nodes[index].nextItem;
        }

        public int GetPrevListIndex(int index)
        {
            return this.data.nodes[index].prevItem;
        }

        public bool IsOnTheList(int index)
        {
            return this.data.nodes[index].isOnTheList;
        }

        public int GetNextAreaListIndex(int index)
        {
            return this.data.nodes[index].nextAreaItem;
        }

        public int GetPrevAreaListIndex(int index)
        {
            return this.data.nodes[index].prevAreaItem;
        }

        public bool IsOnTheAreaList(int index)
        {
            return this.data.nodes[index].isOnTheAreaList;
        }

        public int GetCameFrom(int index)
        {
            return this.data.nodes[index].cameFrom;
        }

        public int GetSteps(int index)
        {
            return this.data.nodes[index].stepCount;
        }

        public Vector3i GetPosition(int index)
        {
            return this.data.nodes[index].pos;
        }

        public bool IsWithin(int index)
        {
            if (index < 0 || index >= this.data.nodes.Length)
            {
                return false;
            }
            return true;
        }

        public bool IsLocationOccupied(int index, bool allowAllyPass = false, bool forceGatesStop = false)
        {
            if (index < 0 || index > this.data.units.Length)
            {
                return false;
            }
            if (this.data.units != null && this.data.units[index] != 0 && SearcherDataV2.IsCDVisible(this.data.units[index], this.attacker) && (!allowAllyPass || SearcherDataV2.GetCDOwnerID(this.data.units[index]) != SearcherDataV2.GetCDOwnerID(this.activeGroupComboInfo)))
            {
                return true;
            }
            if (this.ignoreWalls || this.nonCorporeal)
            {
                return false;
            }
            if (!this.stoppedByGate && index == this.data.gateIndex)
            {
                return false;
            }
            if (this.data.walls != null)
            {
                return this.data.walls[index];
            }
            return false;
        }

        public void RemoveFromOptionList(int index)
        {
            if (this.IsOnTheList(index))
            {
                int nextListIndex = this.GetNextListIndex(index);
                int prevListIndex = this.GetPrevListIndex(index);
                if (prevListIndex >= 0)
                {
                    this.SetNextListIndex(prevListIndex, nextListIndex);
                }
                else
                {
                    this.optionsStart = nextListIndex;
                }
                if (nextListIndex >= 0)
                {
                    this.SetPrevListIndex(nextListIndex, prevListIndex);
                }
                this.SetNextListIndex(index, -1);
                this.SetPrevListIndex(index, -1);
                this.SetOnTheList(index, value: false);
            }
        }

        public void AddToOptionList(int index)
        {
            if (!this.IsOnTheList(index))
            {
                if (this.optionsStart >= 0)
                {
                    this.SetPrevListIndex(this.optionsStart, index);
                    this.SetNextListIndex(index, this.optionsStart);
                }
                this.optionsStart = index;
                this.SetOnTheList(index, value: true);
            }
        }

        public void AddToAreaList(int index)
        {
            if (!this.IsOnTheAreaList(index) && (!this.seekTargetsOnly || this.IsLocationOccupied(index)))
            {
                if (this.areaStart >= 0)
                {
                    this.SetPrevAreaListIndex(this.areaStart, index);
                    this.SetNextAreaListIndex(index, this.areaStart);
                }
                this.areaStart = index;
                this.areaSize++;
                this.SetOnTheAreaList(index, value: true);
            }
        }

        public List<Vector3i> GetPath()
        {
            if (this.pathFound && this.getPathResult == null)
            {
                this.getPathResult = this.GetPathTo(this.toID);
            }
            return this.getPathResult;
        }

        public List<Vector3i> GetPathTo(int target)
        {
            if (!this.pathFound)
            {
                return null;
            }
            int steps = this.GetSteps(target);
            if (steps <= 0)
            {
                return null;
            }
            steps++;
            List<Vector3i> list = new List<Vector3i>(steps);
            if (steps > 50)
            {
                Debug.LogWarning("path longer than 50: " + steps);
            }
            for (int num = target; num != -1; num = this.GetCameFrom(num))
            {
                if (list.Count == steps)
                {
                    Debug.LogError("invalid returned path " + this.debugLog);
                    return null;
                }
                list.Add(this.GetPosition(num));
            }
            if (list != null && list.Count > 1)
            {
                list.Invert();
            }
            return list;
        }

        public List<Vector3i> GetPathTo(Vector3i v)
        {
            int index = this.data.GetIndex(v);
            return this.GetPathTo(index);
        }

        public List<Vector3i> GetArea()
        {
            if (!this.pathFound)
            {
                return null;
            }
            List<Vector3i> list = new List<Vector3i>(this.areaSize);
            for (int nextAreaListIndex = this.areaStart; nextAreaListIndex != -1; nextAreaListIndex = this.GetNextAreaListIndex(nextAreaListIndex))
            {
                if (list.Count == this.areaSize)
                {
                    Debug.LogError("invalid returned path " + this.debugLog);
                    return null;
                }
                list.Add(this.GetPosition(nextAreaListIndex));
            }
            return list;
        }

        public List<Vector3i> GetNoObstackleArea()
        {
            if (!this.pathFound)
            {
                return null;
            }
            List<Vector3i> list = new List<Vector3i>(this.areaSize);
            for (int nextAreaListIndex = this.areaStart; nextAreaListIndex != -1; nextAreaListIndex = this.GetNextAreaListIndex(nextAreaListIndex))
            {
                if (list.Count == this.areaSize)
                {
                    Debug.LogError("invalid returned path " + this.debugLog);
                    return null;
                }
                Vector3i position = this.GetPosition(nextAreaListIndex);
                if (!this.IsLocationOccupied(nextAreaListIndex) || position == this.from)
                {
                    list.Add(position);
                }
            }
            return list;
        }

        public List<int> GetAreaRawIndices()
        {
            if (!this.pathFound)
            {
                return null;
            }
            List<int> list = new List<int>(this.areaSize);
            for (int nextAreaListIndex = this.areaStart; nextAreaListIndex != -1; nextAreaListIndex = this.GetNextAreaListIndex(nextAreaListIndex))
            {
                if (list.Count == this.areaSize)
                {
                    Debug.LogError("invalid returned path " + this.debugLog);
                    return null;
                }
                list.Add(nextAreaListIndex);
            }
            return list;
        }

        public FInt DestinationCost()
        {
            if (!this.pathFound)
            {
                return -1 * FInt.ONE;
            }
            return this.GetGCost(this.toID);
        }

        public FInt GetCostTo(Vector3i pos)
        {
            if (!this.pathFound)
            {
                return -1 * FInt.ONE;
            }
            int index = this.data.GetIndex(pos);
            if (index > 0)
            {
                return this.GetGCost(index);
            }
            return -1 * FInt.ONE;
        }

        public FInt GetEntryCost(Vector3i from, Vector3i to, bool debugdestination = false, Plane debugDataPlane = null)
        {
            int index = this.data.GetIndex(to);
            PathNode node = this.GetNode(this.data.GetIndex(from));
            PathNode node2 = this.GetNode(index);
            Vector3i vector3i = from - to;
            int num = -1;
            int num2 = 0;
            if (vector3i.SqMagnitude() == 2)
            {
                for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
                {
                    if (HexNeighbors.neighbours[i] == vector3i)
                    {
                        num = i;
                        break;
                    }
                }
                if (num >= 0)
                {
                    int num3 = index * 12 + num * 2;
                    num2 = ((num3 >= 0 && this.data.nodeNeighbours.Length > num3) ? this.data.nodeNeighbours[num3 + 1] : 0);
                }
            }
            FInt fInt = this.RoadMPCost(from, to);
            if (fInt > 0)
            {
                return fInt;
            }
            FInt entryCost = this.GetEntryCost(node, node2, num2 > 0);
            if (this.avoidFirewall && this.battle != null && !this.nonCorporeal && this.battle.StepThroughFirewall(from, to))
            {
                entryCost += 10;
            }
            if (debugdestination && debugDataPlane != null)
            {
                Hex hexAt = debugDataPlane.GetHexAt(to);
                StringBuilder stringBuilder = new StringBuilder();
                FInt fInt2 = entryCost;
                stringBuilder.AppendLine("[A*] step cost " + fInt2.ToString());
                string[] obj = new string[6] { "Log Step from: ", null, null, null, null, null };
                Vector3i vector3i2 = from;
                obj[1] = vector3i2.ToString();
                obj[2] = " to ";
                vector3i2 = to;
                obj[3] = vector3i2.ToString();
                obj[4] = " on plane ";
                obj[5] = debugDataPlane.planeSource.dbName;
                stringBuilder.AppendLine(string.Concat(obj));
                stringBuilder.AppendLine("terrainMoveCost " + hexAt.MovementCost());
                stringBuilder.AppendLine("water " + !hexAt.IsLand() + " waterMP " + this.water);
                stringBuilder.AppendLine("mountain " + hexAt.HaveFlag(ETerrainType.Mountain) + " mountainMP " + this.mountain);
                stringBuilder.AppendLine("hill " + hexAt.HaveFlag(ETerrainType.Hill) + " mountainMP " + this.mountain);
                stringBuilder.AppendLine("forest " + hexAt.HaveFlag(ETerrainType.Forest) + " forestMP " + this.forest);
                stringBuilder.AppendLine("earthWalker " + (hexAt.HaveFlag(ETerrainType.Desert) || hexAt.HaveFlag(ETerrainType.Tundra)) + " earthWalerMP " + this.earthWalker);
                stringBuilder.AppendLine("swamp or earth walker" + hexAt.HaveFlag(ETerrainType.Swamp) + " waterMP " + this.water);
                stringBuilder.AppendLine("river considered " + (hexAt.viaRiver != null) + " waterMP " + this.water);
                stringBuilder.AppendLine("Non corporeal " + this.nonCorporeal + " pathfinder " + this.pathfinder);
                Debug.Log(stringBuilder);
            }
            return entryCost;
        }

        public FInt RoadMPCost(Vector3i from, Vector3i to)
        {
            if (this.roadManager == null)
            {
                return FInt.ZERO;
            }
            FInt roadAt = this.roadManager.GetRoadAt(from);
            if (roadAt == FInt.ZERO)
            {
                return roadAt;
            }
            FInt roadAt2 = this.roadManager.GetRoadAt(to);
            return FInt.Min(roadAt, roadAt2);
        }

        public FInt GetEntryCost(PathNode fromN, PathNode n, bool viaRiver, bool includeDangers = false)
        {
            int num = n.terrainMoveCost;
            if (includeDangers && this.data != null && this.dangerPoints != null)
            {
                for (int i = 0; i < this.dangerPoints.Length; i++)
                {
                    Multitype<Vector3i, int> multitype = this.dangerPoints[i];
                    if (this.area.HexDistance(n.pos, multitype.t0) <= multitype.t1)
                    {
                        num += 2;
                    }
                }
            }
            if (this.embarkCost >= 0 && fromN.water != n.water)
            {
                num += this.embarkCost;
            }
            else if (n.water)
            {
                if (!this.nonCorporeal && !this.pathfinder && n.terrainMoveCost > 0)
                {
                    return new FInt(this.water ? n.terrainMoveCost : (-1));
                }
                return new FInt(this.water ? 1 : (-1));
            }
            if (this.embarkCost == -1 && !this.land)
            {
                return new FInt(-1);
            }
            if (this.nonCorporeal || this.pathfinder)
            {
                return FInt.ONE;
            }
            int num2 = 0;
            if (viaRiver && !this.water)
            {
                num2 = 1;
            }
            if (n.hill || n.mountain)
            {
                if (this.mountain)
                {
                    return SearcherDataV2.ClampMPCost(this.data, new FInt(1 + num2));
                }
            }
            else if (n.forest)
            {
                if (this.forest)
                {
                    return SearcherDataV2.ClampMPCost(this.data, new FInt(1 + num2));
                }
            }
            else if (n.tundra)
            {
                if (this.earthWalker)
                {
                    return SearcherDataV2.ClampMPCost(this.data, new FInt(1 + num2));
                }
            }
            else if (n.desert)
            {
                if (this.earthWalker)
                {
                    return SearcherDataV2.ClampMPCost(this.data, new FInt(1 + num2));
                }
            }
            else if (n.swamp && (this.water || this.earthWalker))
            {
                return FInt.ONE;
            }
            return SearcherDataV2.ClampMPCost(this.data, new FInt(num + num2));
        }

        public void EnsureInitialized(int index)
        {
            if (index >= 0)
            {
                this.data.nodes[index].EnsureInitialized(this);
            }
        }

        public bool IsTransportReady(int index, PathNode node)
        {
            if (index > 0 && this.data.units.Length > index)
            {
                int comboData = this.data.units[index];
                if (this.activeGroupComboInfo > 0 && SearcherDataV2.GetCDOwnerID(comboData) == SearcherDataV2.GetCDOwnerID(this.activeGroupComboInfo) && SearcherDataV2.HasTransporter(comboData, node.water))
                {
                    int cDUnitCount = SearcherDataV2.GetCDUnitCount(this.data.units[index]);
                    if (cDUnitCount < 9)
                    {
                        return cDUnitCount > 0;
                    }
                    return false;
                }
            }
            return false;
        }

        public void RestartSearch()
        {
            this.pathFound = false;
            this.getPathResult = null;
            PathNode[] nodes = this.data.nodes;
            for (int i = 0; i < nodes.Length; i++)
            {
                PathNode pathNode = nodes[i];
                pathNode.requestID = -1;
            }
        }
    }
}
