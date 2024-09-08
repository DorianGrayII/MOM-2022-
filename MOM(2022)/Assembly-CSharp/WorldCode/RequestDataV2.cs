namespace WorldCode
{
    using DBDef;
    using MHUtils;
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class RequestDataV2
    {
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

        public void AddToAreaList(int index)
        {
            if (!this.IsOnTheAreaList(index) && (!this.seekTargetsOnly || this.IsLocationOccupied(index, false, false)))
            {
                if (this.areaStart >= 0)
                {
                    this.SetPrevAreaListIndex(this.areaStart, index);
                    this.SetNextAreaListIndex(index, this.areaStart);
                }
                this.areaStart = index;
                this.areaSize++;
                this.SetOnTheAreaList(index, true);
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
                this.SetOnTheList(index, true);
            }
        }

        public void AllowEmbarkAtCost(int embarkCost)
        {
            this.water = true;
            this.land = true;
            this.movementPool = 0;
            this.embarkCost = embarkCost;
        }

        public void BreakMovementByMPPool(int mpPool, FInt actTurnMovementPool)
        {
            this.movementPool = mpPool;
            this.activeTurnMovementPool = actTurnMovementPool;
        }

        public static RequestDataV2 CreateRequest(WorldCode.Plane p, Vector3i from, FInt mpRange, IPlanePosition unit, bool avoidFirewall)
        {
            RequestDataV2 av = new RequestDataV2 {
                requestID = ++nextAvaliableID,
                from = from,
                to = Vector3i.invalid,
                mpRange = mpRange,
                pathFound = false,
                area = p.area,
                exclusionPoints = p.exclusionPoints,
                data = p.GetSearcherData(),
                roadManager = p.GetRoadManagers()
            };
            av.FillMPCost(unit);
            if ((p != null) && p.battlePlane)
            {
                av.battle = Battle.GetBattle();
                if (av.battle != null)
                {
                    av.avoidFirewall = avoidFirewall && av.battle.fireWall;
                }
            }
            if (unit is MOM.Group)
            {
                av.activeGroupComboInfo = av.data.GetGroupSizeOwnerCombo(unit as MOM.Group);
            }
            else
            {
                av.activeGroupComboInfo = -1;
                if (unit is BattleUnit)
                {
                    av.attacker = (unit as BattleUnit).attackingSide;
                    av.activeGroupComboInfo = av.attacker ? -1 : -2;
                }
            }
            return av;
        }

        public static RequestDataV2 CreateRequest(WorldCode.Plane p, Vector3i from, Vector3i to, IPlanePosition unit, bool allTerrain, bool avoidFirewall, bool movementFormBasedOnStartLocation)
        {
            RequestDataV2 av = new RequestDataV2 {
                requestID = ++nextAvaliableID,
                from = from,
                to = p.area.KeepHorizontalInside(to),
                pathFound = false,
                area = p.area,
                exclusionPoints = p.exclusionPoints,
                data = p.GetSearcherData(),
                roadManager = p.GetRoadManagers()
            };
            av.FillMPCost(unit);
            if ((p != null) && p.battlePlane)
            {
                av.battle = Battle.GetBattle();
                if (av.battle != null)
                {
                    av.avoidFirewall = avoidFirewall && av.battle.fireWall;
                }
            }
            if (unit is MOM.Group)
            {
                av.activeGroupComboInfo = av.data.GetGroupSizeOwnerCombo(unit as MOM.Group);
            }
            else if (!(unit is BattleUnit))
            {
                av.activeGroupComboInfo = 0;
            }
            else
            {
                av.attacker = (unit as BattleUnit).attackingSide;
                av.activeGroupComboInfo = av.attacker ? -1 : -2;
            }
            if (movementFormBasedOnStartLocation && ((unit == null) && !p.IsLand(from)))
            {
                av.water = true;
                av.land = false;
            }
            if (allTerrain)
            {
                av.FillMPMultiterrain();
            }
            return av;
        }

        public FInt DestinationCost()
        {
            return (this.pathFound ? this.GetGCost(this.toID) : ((FInt) (-1 * FInt.ONE)));
        }

        public void EnsureInitialized(int index)
        {
            if (index >= 0)
            {
                this.data.nodes[index].EnsureInitialized(this);
            }
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

        private void FillMovementCost(MOM.Group g)
        {
            if (!g.SubsetSelected())
            {
                this.water = g.waterMovement;
                this.land = g.landMovement;
                this.mountain = g.mountainMovement;
                this.forest = g.forestMovement;
                this.earthWalker = g.earthWalkerMovement;
                this.nonCorporeal = g.nonCorporealMovement;
                this.pathfinder = g.pathfinderMovement;
            }
            else
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
            this.BreakMovementByMPPool(g.GetMaxMP(), g.CurentMP());
            this.stoppedByGate = false;
        }

        private void FillMovementCost(MOM.Location g)
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

        private void FillMPCost(IPlanePosition unit)
        {
            AIArmyResupply resupply = unit as AIArmyResupply;
            if (resupply != null)
            {
                if (resupply.group != null)
                {
                    this.FillMovementCost(resupply.group.Get());
                }
            }
            else
            {
                AIWarArmy army = unit as AIWarArmy;
                if (army != null)
                {
                    if (army.group != null)
                    {
                        this.FillMovementCost(army.group.Get());
                    }
                }
                else if (unit is MOM.Group)
                {
                    this.FillMovementCost(unit as MOM.Group);
                }
                else if (unit is MOM.Location)
                {
                    this.FillMovementCost(unit as MOM.Location);
                }
                else if (unit is BattleUnit)
                {
                    this.FillMovementCost(unit as BattleUnit);
                }
                else
                {
                    this.water = false;
                    this.land = true;
                    this.mountain = true;
                    this.forest = true;
                    this.earthWalker = true;
                    this.nonCorporeal = true;
                    this.pathfinder = true;
                    this.stoppedByGate = false;
                }
            }
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

        public List<Vector3i> GetArea()
        {
            if (!this.pathFound)
            {
                return null;
            }
            List<Vector3i> list = new List<Vector3i>(this.areaSize);
            for (int i = this.areaStart; i != -1; i = this.GetNextAreaListIndex(i))
            {
                if (list.Count == this.areaSize)
                {
                    string text1;
                    if (this.debugLog != null)
                    {
                        text1 = this.debugLog.ToString();
                    }
                    else
                    {
                        StringBuilder debugLog = this.debugLog;
                        text1 = null;
                    }
                    Debug.LogError("invalid returned path " + text1);
                    return null;
                }
                list.Add(this.GetPosition(i));
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
            for (int i = this.areaStart; i != -1; i = this.GetNextAreaListIndex(i))
            {
                if (list.Count == this.areaSize)
                {
                    string text1;
                    if (this.debugLog != null)
                    {
                        text1 = this.debugLog.ToString();
                    }
                    else
                    {
                        StringBuilder debugLog = this.debugLog;
                        text1 = null;
                    }
                    Debug.LogError("invalid returned path " + text1);
                    return null;
                }
                list.Add(i);
            }
            return list;
        }

        public int GetCameFrom(int index)
        {
            return this.data.nodes[index].cameFrom;
        }

        public FInt GetCostTo(Vector3i pos)
        {
            if (!this.pathFound)
            {
                return (FInt) (-1 * FInt.ONE);
            }
            int index = this.data.GetIndex(pos);
            return ((index <= 0) ? ((FInt) (-1 * FInt.ONE)) : this.GetGCost(index));
        }

        public int GetDangerSum(int index)
        {
            return this.data.nodes[index].totalDanger;
        }

        public FInt GetEntryCost(Vector3i from, Vector3i to, bool debugdestination, WorldCode.Plane debugDataPlane)
        {
            int index = this.data.GetIndex(to);
            PathNode fromN = this.GetNode(this.data.GetIndex(from));
            PathNode node = this.GetNode(index);
            Vector3i vectori = from - to;
            int num2 = -1;
            int num3 = 0;
            if (vectori.SqMagnitude() == 2)
            {
                int num6 = 0;
                while (true)
                {
                    if (num6 < HexNeighbors.neighbours.Length)
                    {
                        if (!(HexNeighbors.neighbours[num6] == vectori))
                        {
                            num6++;
                            continue;
                        }
                        num2 = num6;
                    }
                    if (num2 >= 0)
                    {
                        int num7 = (index * 12) + (num2 * 2);
                        num3 = ((num7 < 0) || (this.data.nodeNeighbours.Length <= num7)) ? 0 : this.data.nodeNeighbours[num7 + 1];
                    }
                    break;
                }
            }
            FInt num4 = this.RoadMPCost(from, to);
            if (num4 > 0)
            {
                return num4;
            }
            FInt num5 = this.GetEntryCost(fromN, node, num3 > 0, false);
            if (this.avoidFirewall && ((this.battle != null) && (!this.nonCorporeal && this.battle.StepThroughFirewall(from, to))))
            {
                num5 += 10;
            }
            if (debugdestination && (debugDataPlane != null))
            {
                Hex hexAt = debugDataPlane.GetHexAt(to);
                StringBuilder message = new StringBuilder();
                message.AppendLine("[A*] step cost " + num5.ToString());
                string[] textArray1 = new string[] { "Log Step from: ", from.ToString(), " to ", to.ToString(), " on plane ", debugDataPlane.planeSource.dbName };
                message.AppendLine(string.Concat(textArray1));
                message.AppendLine("terrainMoveCost " + hexAt.MovementCost().ToString());
                message.AppendLine("water " + !hexAt.IsLand().ToString() + " waterMP " + this.water.ToString());
                message.AppendLine("mountain " + hexAt.HaveFlag(ETerrainType.Mountain).ToString() + " mountainMP " + this.mountain.ToString());
                message.AppendLine("hill " + hexAt.HaveFlag(ETerrainType.Hill).ToString() + " mountainMP " + this.mountain.ToString());
                message.AppendLine("forest " + hexAt.HaveFlag(ETerrainType.Forest).ToString() + " forestMP " + this.forest.ToString());
                message.AppendLine("earthWalker " + (hexAt.HaveFlag(ETerrainType.Desert) || hexAt.HaveFlag(ETerrainType.Tundra)).ToString() + " earthWalerMP " + this.earthWalker.ToString());
                message.AppendLine("swamp or earth walker" + hexAt.HaveFlag(ETerrainType.Swamp).ToString() + " waterMP " + this.water.ToString());
                message.AppendLine("river considered " + (hexAt.viaRiver != null).ToString() + " waterMP " + this.water.ToString());
                message.AppendLine("Non corporeal " + this.nonCorporeal.ToString() + " pathfinder " + this.pathfinder.ToString());
                Debug.Log(message);
            }
            return num5;
        }

        public FInt GetEntryCost(PathNode fromN, PathNode n, bool viaRiver, bool includeDangers)
        {
            int terrainMoveCost = n.terrainMoveCost;
            if (includeDangers && ((this.data != null) && (this.dangerPoints != null)))
            {
                for (int i = 0; i < this.dangerPoints.Length; i++)
                {
                    Multitype<Vector3i, int> multitype = this.dangerPoints[i];
                    if (this.area.HexDistance(n.pos, multitype.t0) <= multitype.t1)
                    {
                        terrainMoveCost += 2;
                    }
                }
            }
            if ((this.embarkCost >= 0) && (fromN.water != n.water))
            {
                terrainMoveCost += this.embarkCost;
            }
            else if (n.water)
            {
                return ((this.nonCorporeal || (this.pathfinder || (n.terrainMoveCost <= 0))) ? new FInt(this.water ? 1 : -1) : new FInt(this.water ? n.terrainMoveCost : -1));
            }
            if ((this.embarkCost == -1) && !this.land)
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
            return SearcherDataV2.ClampMPCost(this.data, new FInt(terrainMoveCost + num2));
        }

        public FInt GetGCost(int index)
        {
            return this.data.nodes[index].gCost;
        }

        public int GetLocalDanger(int index)
        {
            if (this.dangerMap == null)
            {
                return 0;
            }
            int num = this.dangerMap.GetValue(this.GetPosition(index));
            return ((num >= this.groupValue) ? num : 0);
        }

        public int GetNextAreaListIndex(int index)
        {
            return this.data.nodes[index].nextAreaItem;
        }

        public int GetNextListIndex(int index)
        {
            return this.data.nodes[index].nextItem;
        }

        public PathNode GetNode(int index)
        {
            if ((index >= 0) && (index < this.data.nodes.Length))
            {
                return this.data.nodes[index];
            }
            return new PathNode();
        }

        public List<Vector3i> GetNoObstackleArea()
        {
            if (!this.pathFound)
            {
                return null;
            }
            List<Vector3i> list = new List<Vector3i>(this.areaSize);
            for (int i = this.areaStart; i != -1; i = this.GetNextAreaListIndex(i))
            {
                if (list.Count == this.areaSize)
                {
                    string text1;
                    if (this.debugLog != null)
                    {
                        text1 = this.debugLog.ToString();
                    }
                    else
                    {
                        StringBuilder debugLog = this.debugLog;
                        text1 = null;
                    }
                    Debug.LogError("invalid returned path " + text1);
                    return null;
                }
                Vector3i position = this.GetPosition(i);
                if (!this.IsLocationOccupied(i, false, false) || (position == this.from))
                {
                    list.Add(position);
                }
            }
            return list;
        }

        public List<Vector3i> GetPath()
        {
            if (this.pathFound)
            {
                if (this.getPathResult == null)
                    this.getPathResult = this.GetPathTo(this.toID);
            }
            return this.getPathResult;
        }

        public List<Vector3i> GetPathTo(Vector3i v)
        {
            int index = this.data.GetIndex(v);
            return this.GetPathTo(index);
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
                Debug.LogWarning("path longer than 50: " + steps.ToString());
            }
            for (int i = target; i != -1; i = this.GetCameFrom(i))
            {
                if (list.Count == steps)
                {
                    string text1;
                    if (this.debugLog != null)
                    {
                        text1 = this.debugLog.ToString();
                    }
                    else
                    {
                        StringBuilder debugLog = this.debugLog;
                        text1 = null;
                    }
                    Debug.LogError("invalid returned path " + text1);
                    return null;
                }
                list.Add(this.GetPosition(i));
            }
            if ((list != null) && (list.Count > 1))
            {
                ListUtils.Invert<Vector3i>(list);
            }
            return list;
        }

        public Vector3i GetPosition(int index)
        {
            return this.data.nodes[index].pos;
        }

        public int GetPrevAreaListIndex(int index)
        {
            return this.data.nodes[index].prevAreaItem;
        }

        public int GetPrevListIndex(int index)
        {
            return this.data.nodes[index].prevItem;
        }

        public int GetSteps(int index)
        {
            return this.data.nodes[index].stepCount;
        }

        public FInt GetSumCost(int index)
        {
            return this.data.nodes[index].sumCost;
        }

        public bool IsLocationOccupied(int index, bool allowAllyPass, bool forceGatesStop)
        {
            return ((index >= 0) && ((index <= this.data.units.Length) && (((this.data.units == null) || ((this.data.units[index] == 0) || (!SearcherDataV2.IsCDVisible(this.data.units[index], this.attacker) || (allowAllyPass && (SearcherDataV2.GetCDOwnerID(this.data.units[index]) == SearcherDataV2.GetCDOwnerID(this.activeGroupComboInfo)))))) ? (!this.ignoreWalls && (!this.nonCorporeal && ((this.stoppedByGate || (index != this.data.gateIndex)) ? ((this.data.walls != null) && this.data.walls[index]) : false))) : true)));
        }

        public bool IsOnTheAreaList(int index)
        {
            return this.data.nodes[index].isOnTheAreaList;
        }

        public bool IsOnTheList(int index)
        {
            return this.data.nodes[index].isOnTheList;
        }

        public bool IsTransportReady(int index, PathNode node)
        {
            if ((index > 0) && (this.data.units.Length > index))
            {
                int comboData = this.data.units[index];
                if ((this.activeGroupComboInfo > 0) && ((SearcherDataV2.GetCDOwnerID(comboData) == SearcherDataV2.GetCDOwnerID(this.activeGroupComboInfo)) && SearcherDataV2.HasTransporter(comboData, node.water)))
                {
                    int cDUnitCount = SearcherDataV2.GetCDUnitCount(this.data.units[index]);
                    return ((cDUnitCount < 9) && (cDUnitCount > 0));
                }
            }
            return false;
        }

        public bool IsWithin(int index)
        {
            return ((index >= 0) && (index < this.data.nodes.Length));
        }

        public void MakeItAvoidanceSearch(MOM.Group p)
        {
            if (p == null)
            {
                Debug.LogError("Cannot add danger map for a search without group reference");
            }
            else if (!DataHeatMaps.Get(p.GetPlane()).IsMapReady(DataHeatMaps.HMType.DangerMap, p.GetOwnerID(), false))
            {
                Debug.LogError("danger heat map need to be prepared before use!");
            }
            else
            {
                this.groupValue = p.GetValue();
                this.dangerMap = DataHeatMaps.Get(p.GetPlane()).GetHeatMap(DataHeatMaps.HMType.DangerMap);
            }
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
                this.SetOnTheList(index, false);
            }
        }

        public void RestartSearch()
        {
            this.pathFound = false;
            this.getPathResult = null;
            foreach (PathNode node in this.data.nodes)
            {
                node.requestID = -1;
            }
        }

        public FInt RoadMPCost(Vector3i from, Vector3i to)
        {
            if (this.roadManager == null)
            {
                return FInt.ZERO;
            }
            FInt roadAt = this.roadManager.GetRoadAt(from);
            return (!(roadAt == FInt.ZERO) ? FInt.Min(roadAt, this.roadManager.GetRoadAt(to)) : roadAt);
        }

        public void SetCameFrom(int index, int value)
        {
            this.data.nodes[index].cameFrom = value;
        }

        public void SetCameFromStepCount(int index, int value)
        {
            this.data.nodes[index].stepCount = value;
        }

        public void SetDangerForNode(int index, int previousDanger)
        {
            if (this.dangerMap != null)
            {
                int num = this.dangerMap.GetValue(this.GetPosition(index));
                if (num < this.groupValue)
                {
                    this.data.nodes[index].totalDanger = previousDanger;
                }
                else
                {
                    this.data.nodes[index].totalDanger = previousDanger + (num - this.groupValue);
                }
            }
        }

        public void SetDangerPoints(Multitype<Vector3i, int>[] dangers)
        {
            this.dangerPoints = dangers;
        }

        public void SetGCost(int index, FInt value)
        {
            this.data.nodes[index].SetGCost(value);
        }

        public void SetNextAreaListIndex(int index, int value)
        {
            this.data.nodes[index].nextAreaItem = value;
        }

        public void SetNextListIndex(int index, int value)
        {
            this.data.nodes[index].nextItem = value;
        }

        public void SetNode(int index, PathNode n)
        {
            this.data.nodes[index] = n;
        }

        public void SetOnTheAreaList(int index, bool value)
        {
            this.data.nodes[index].isOnTheAreaList = value;
        }

        public void SetOnTheList(int index, bool value)
        {
            this.data.nodes[index].isOnTheList = value;
        }

        public void SetPrevAreaListIndex(int index, int value)
        {
            this.data.nodes[index].prevAreaItem = value;
        }

        public void SetPrevListIndex(int index, int value)
        {
            this.data.nodes[index].prevItem = value;
        }

        public enum MPCost
        {
            eImpossible,
            eNormal,
            eCheap,
            MAX
        }
    }
}

