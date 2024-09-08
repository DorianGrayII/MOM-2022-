// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AIWarEffort
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class AIWarEffort
{
    [ProtoMember(1)]
    public Reference<PlayerWizard> owner;

    [ProtoMember(2)]
    public Reference<PlayerWizard> enemy;

    [ProtoMember(3)]
    public List<AIWarTarget> warTargets;

    [ProtoMember(4)]
    public List<AIWarArmy> armies;

    [ProtoMember(5)]
    public List<Reference<global::MOM.Location>> gates;

    [ProtoMember(6)]
    public List<AIArmyResupply> resupply;

    [ProtoMember(7)]
    public bool needToConquerGates;

    [ProtoMember(8)]
    public bool valid;

    [ProtoIgnore]
    public List<SingleRegion> regionsAlly;

    [ProtoIgnore]
    public List<SingleRegion> regionsEnemy;

    [ProtoIgnore]
    public SingleRegion closestRegion;

    [ProtoIgnore]
    public SingleRegion mostEngagedRegion;

    [ProtoIgnore]
    private bool ready;

    public AIWarEffort()
    {
    }

    public AIWarEffort(PlayerWizard owner, PlayerWizard enemy)
    {
        this.owner = owner;
        this.enemy = enemy;
    }

    public IEnumerator ValidateWarEffort()
    {
        this.ready = false;
        if (!this.enemy.Get().isAlive || !this.owner.Get().GetDiplomacy().IsAtWarWith(this.enemy.ID))
        {
            this.ready = true;
            this.valid = false;
            yield break;
        }
        if (this.armies == null)
        {
            this.armies = new List<AIWarArmy>();
        }
        for (int num = this.armies.Count - 1; num >= 0; num--)
        {
            AIWarArmy aIWarArmy = this.armies[num];
            if (aIWarArmy.target != null && (this.warTargets == null || !this.warTargets.Contains(aIWarArmy.target)))
            {
                aIWarArmy.target = null;
            }
            aIWarArmy.Validate();
        }
        this.armies = this.armies.FindAll((AIWarArmy o) => o.group != null);
        if (this.resupply == null)
        {
            this.resupply = new List<AIArmyResupply>();
        }
        for (int num2 = this.resupply.Count - 1; num2 >= 0; num2--)
        {
            if (!(this.resupply[num2]?.Valid() ?? false))
            {
                this.resupply.RemoveAt(num2);
            }
        }
        yield return this.IdentifyObjectives();
        while (!this.ready)
        {
            yield return null;
        }
        this.valid = true;
    }

    public IEnumerator ActivateWarEfforts()
    {
        yield return this.PrepareDangerMaps();
        if (this.regionsAlly != null && this.regionsEnemy != null)
        {
            this.ActToProvideArmies(this.regionsAlly, this.regionsEnemy);
        }
        foreach (AIWarArmy army in this.armies)
        {
            if (army.group != null && World.Get() != null)
            {
                _ = army.group.ID;
                _ = World.Get().debugIndex;
            }
            if ((army.target == null || global::UnityEngine.Random.Range(0, 100) < 15) && this.warTargets != null)
            {
                AIWarTarget aIWarTarget = null;
                int num = int.MaxValue;
                foreach (AIWarTarget warTarget in this.warTargets)
                {
                    if (warTarget.haveAssignee || warTarget.GetGroup() == null || army == null)
                    {
                        continue;
                    }
                    global::MOM.Location location = warTarget.GetGroup().locationHost?.Get();
                    if (location == null || location.GetUnits().Count >= 1 || location.locationType != ELocationType.Node)
                    {
                        int num2 = this.GatedDistance(warTarget.GetGroup(), army);
                        if (num > num2)
                        {
                            num = num2;
                            aIWarTarget = warTarget;
                        }
                    }
                }
                if (aIWarTarget != null)
                {
                    army.SetTarget(aIWarTarget);
                }
            }
            if (army.target == null || !army.target.Valid() || army.group?.Get() == null)
            {
                continue;
            }
            bool num3 = army.GetPlane() != army.target.GetGroup().GetPlane();
            global::MOM.Group target = army.target.GetGroup();
            target.GetPosition();
            if (num3)
            {
                global::MOM.Location travelGate = this.GetTravelGate(army, target);
                if (travelGate == null)
                {
                    this.needToConquerGates = true;
                    continue;
                }
                if (travelGate.GetOwnerID() != this.owner.Get().GetID())
                {
                    target = travelGate.GetLocalGroup();
                }
                travelGate.GetPosition();
            }
            AIAchievability ach = new AIAchievability();
            yield return this.IsTargetAchievable(army.group, target, ach);
            if (!ach.anyAdvantagePossible || (!ach.targetIsAchievable && army.group.Get().GetUnits().Count <= 4))
            {
                yield return this.MoveArmy(army, null);
            }
            else
            {
                yield return PlayerWizardAI.MoveGroup(army.group.Get(), target.GetPosition(), target.GetPlane());
            }
        }
    }

    public IEnumerator PrepareDangerMaps()
    {
        bool ready2 = DataHeatMaps.Get(World.GetArcanus()).IsMapReady(DataHeatMaps.HMType.DangerMap, this.owner.Get().GetID());
        while (!ready2)
        {
            ready2 = DataHeatMaps.Get(World.GetArcanus()).IsMapReady(DataHeatMaps.HMType.DangerMap, this.owner.Get().GetID());
            if (ready2)
            {
                break;
            }
            yield return null;
        }
        ready2 = DataHeatMaps.Get(World.GetMyrror()).IsMapReady(DataHeatMaps.HMType.DangerMap, this.owner.Get().GetID());
        while (!ready2)
        {
            ready2 = DataHeatMaps.Get(World.GetMyrror()).IsMapReady(DataHeatMaps.HMType.DangerMap, this.owner.Get().GetID());
            if (!ready2)
            {
                yield return null;
                continue;
            }
            break;
        }
    }

    public IEnumerator IdentifyObjectives()
    {
        this.regionsAlly = null;
        this.regionsEnemy = null;
        if (this.warTargets == null)
        {
            this.warTargets = new List<AIWarTarget>();
        }
        this.warTargets.Clear();
        this.gates = null;
        PlayerWizardAI ai = this.owner.Get() as PlayerWizardAI;
        if (this.regionsAlly == null)
        {
            this.regionsAlly = new List<SingleRegion>();
        }
        if (this.regionsEnemy == null)
        {
            this.regionsEnemy = new List<SingleRegion>();
        }
        yield return ai.controlRegion.GetRegionsAsync(this.regionsAlly);
        yield return this.enemy.Get().controlRegion.GetRegionsAsync(this.regionsEnemy);
        this.closestRegion = null;
        foreach (SingleRegion item in this.regionsAlly)
        {
            item.enemyRegionAssignment = null;
            foreach (SingleRegion item2 in this.regionsEnemy)
            {
                if (item.enemyRegionAssignment == null || item.plane.GetDistanceWrapping(item.enemyRegionAssignment.center, item.center) > item.plane.GetDistanceWrapping(item2.center, item.center))
                {
                    item.enemyRegionAssignment = item2;
                    if (this.closestRegion == null || item.plane.GetDistanceWrapping(this.closestRegion.center, item.center) > item.plane.GetDistanceWrapping(item2.center, item.center))
                    {
                        this.closestRegion = item2;
                        this.mostEngagedRegion = item;
                    }
                }
            }
        }
        List<AIRegionalWarAssignment> list = new List<AIRegionalWarAssignment>();
        foreach (global::MOM.Group v in ai.GetVisibleGroups())
        {
            if (!v.alive || v.GetOwnerID() != this.enemy.Get().GetID())
            {
                continue;
            }
            foreach (SingleRegion r in this.regionsAlly)
            {
                if (v.GetPlane() == r.plane && r.area.Contains(v.GetPosition()))
                {
                    AIWarTarget aIWarTarget = new AIWarTarget();
                    aIWarTarget.group = v;
                    aIWarTarget.owner = this.enemy.Get();
                    aIWarTarget.consideredAnInvader = true;
                    if (this.warTargets.Find((AIWarTarget o) => o.group?.Get() == v) == null)
                    {
                        this.warTargets.Add(aIWarTarget);
                    }
                    AIRegionalWarAssignment aIRegionalWarAssignment = list.Find((AIRegionalWarAssignment o) => o.owner == r);
                    if (aIRegionalWarAssignment == null)
                    {
                        aIRegionalWarAssignment = new AIRegionalWarAssignment(r);
                        list.Add(aIRegionalWarAssignment);
                    }
                    aIRegionalWarAssignment.objectives.Add(aIWarTarget);
                }
            }
        }
        if (this.armies == null)
        {
            this.armies = new List<AIWarArmy>();
        }
        foreach (AIWarArmy army in this.armies)
        {
            if (army.group?.Get() == null)
            {
                continue;
            }
            SingleRegion closestEnemyRegion = this.GetClosestEnemyRegion(army.group.Get(), this.regionsEnemy);
            if (closestEnemyRegion?.locations == null)
            {
                continue;
            }
            foreach (TownLocation tl in closestEnemyRegion.locations)
            {
                if (this.warTargets == null || this.warTargets.Find((AIWarTarget o) => o.group?.Get() == tl.GetLocalGroup()) == null)
                {
                    AIWarTarget aIWarTarget2 = new AIWarTarget();
                    aIWarTarget2.owner = GameManager.GetWizard(tl.owner);
                    aIWarTarget2.group = tl.GetLocalGroup();
                    if (this.warTargets == null)
                    {
                        this.warTargets = new List<AIWarTarget>();
                    }
                    this.warTargets.Add(aIWarTarget2);
                }
            }
        }
        this.ready = true;
    }

    public List<Reference<global::MOM.Location>> GetGates()
    {
        if (!GameManager.Get().allowPlaneSwitch)
        {
            return null;
        }
        if (this.gates != null)
        {
            foreach (global::MOM.Location item in GameManager.GetLocationsOfThePlane(arcanus: true))
            {
                if (item.GetOwnerID() == this.enemy.ID && item.GetOwnerID() == this.owner.ID)
                {
                    if (this.gates == null)
                    {
                        this.gates = new List<Reference<global::MOM.Location>>();
                    }
                    this.gates.Add(item);
                }
            }
        }
        return this.gates;
    }

    public int GatedDistance(IPlanePosition a, IPlanePosition b)
    {
        if (a == null || b == null)
        {
            return int.MaxValue;
        }
        if (a.GetPlane() == b.GetPlane())
        {
            return a.GetDistanceTo(b);
        }
        List<Reference<global::MOM.Location>> list = this.GetGates();
        if (list == null || list.Count < 1)
        {
            return 10000;
        }
        if (list.Find((Reference<global::MOM.Location> o) => o.Get().GetPosition() == b.GetPosition()) != null)
        {
            return a.GetDistanceTo(b.GetPosition());
        }
        int num = int.MaxValue;
        foreach (Reference<global::MOM.Location> item in list)
        {
            int num2 = a.GetDistanceTo(item.Get().GetPosition()) + b.GetDistanceTo(item.Get().GetPosition());
            if (num < num2)
            {
                num = num2;
            }
        }
        return num;
    }

    public int GatedDistance(IPlanePosition a, global::WorldCode.Plane p, Vector3i b)
    {
        if (a.GetPlane() == p)
        {
            return a.GetDistanceTo(b);
        }
        List<Reference<global::MOM.Location>> list = this.GetGates();
        if (list == null || list.Count < 1)
        {
            return 10000;
        }
        if (list.Find((Reference<global::MOM.Location> o) => o.Get().GetPosition() == b) != null)
        {
            return a.GetDistanceTo(b);
        }
        int num = int.MaxValue;
        foreach (Reference<global::MOM.Location> item in list)
        {
            int num2 = a.GetDistanceTo(item.Get().GetPosition()) + item.Get().GetDistanceTo(b);
            if (num < num2)
            {
                num = num2;
            }
        }
        return num;
    }

    public global::MOM.Location GetTravelGate(IPlanePosition a, IPlanePosition b)
    {
        if (a.GetPlane() == b.GetPlane())
        {
            return null;
        }
        List<Reference<global::MOM.Location>> list = this.GetGates();
        if (list == null || list.Count < 1)
        {
            return null;
        }
        if (list.Find((Reference<global::MOM.Location> o) => o.Get().GetPosition() == b.GetPosition()) != null)
        {
            if (b is global::MOM.Group group)
            {
                return group.GetLocationHost();
            }
            return b as global::MOM.Location;
        }
        int num = 10000;
        global::MOM.Location result = null;
        foreach (Reference<global::MOM.Location> item in list)
        {
            int num2 = a.GetDistanceTo(item.Get().GetPosition()) + b.GetDistanceTo(item.Get().GetPosition());
            if (num < num2)
            {
                num = num2;
                result = item.Get();
            }
        }
        return result;
    }

    public List<Vector3i> GetSameTerrainPath(Vector3i a, Vector3i b, global::WorldCode.Plane plane)
    {
        bool flag = plane.IsLand(a);
        bool flag2 = plane.IsLand(b);
        if (flag != flag2)
        {
            return null;
        }
        if (flag)
        {
            List<Vector3i> islandFor = plane.GetIslandFor(a);
            if (islandFor != null && islandFor.Contains(b))
            {
                goto IL_0048;
            }
        }
        if (!flag)
        {
            HashSet<Vector3i> waterBodyFor = plane.GetWaterBodyFor(a);
            if (waterBodyFor != null && waterBodyFor.Contains(b))
            {
                goto IL_0048;
            }
        }
        return null;
        IL_0048:
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, a, b, null, allTerrain: false, avoidFirewall: false, movementFormBasedOnStartLocation: true);
        PathfinderV2.FindPath(requestDataV);
        return requestDataV.GetPath();
    }

    public List<Vector3i> GetGroupAblePath(IPlanePosition a, Vector3i b)
    {
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(a.GetPlane(), a.GetPosition(), b, a);
        PathfinderV2.FindPath(requestDataV);
        return requestDataV.GetPath();
    }

    public List<Vector3i> GetGroupWaterAndLandPath(IPlanePosition a, Vector3i b)
    {
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(a.GetPlane(), a.GetPosition(), b, a, allTerrain: true);
        PathfinderV2.FindPath(requestDataV);
        return requestDataV.GetPath();
    }

    private global::MOM.Group FindAndFormTransport(Vector3i placeOfNeed, global::WorldCode.Plane plane)
    {
        AIPlaneVisibility planeVisibility = (this.owner.Get() as PlayerWizardAI).GetPlaneVisibility(plane);
        if (planeVisibility.ownGroups == null)
        {
            return null;
        }
        int num = 10000;
        global::MOM.Group group = null;
        foreach (global::MOM.Group ownGroup in planeVisibility.ownGroups)
        {
            int distanceTo = ownGroup.GetDistanceTo(placeOfNeed);
            if (distanceTo >= 25 || distanceTo >= num || ownGroup.transporter?.Get() == null || (ownGroup.CurentMP() == 0 && !ownGroup.IsHosted()))
            {
                continue;
            }
            global::MOM.Unit unit = ownGroup.transporter?.Get();
            bool flag = false;
            foreach (Reference<global::MOM.Unit> unit2 in ownGroup.GetUnits())
            {
                if (!(unit2 == unit))
                {
                    if (!unit2.Get().CanTravelOverWater())
                    {
                        flag = true;
                    }
                    else
                    {
                        _ = unit2.Get().GetAttFinal((Tag)TAG.TRANSPORTER) <= 0;
                    }
                }
            }
            if (flag)
            {
                continue;
            }
            if (ownGroup.positionOfOrigin == placeOfNeed)
            {
                num = 0;
                group = ownGroup;
                continue;
            }
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, ownGroup.positionOfOrigin, placeOfNeed, ownGroup);
            PathfinderV2.FindPath(requestDataV);
            int num2 = requestDataV.GetPath()?.Count ?? int.MaxValue;
            if (num2 >= 2 && num2 < num)
            {
                num = num2;
                group = ownGroup;
            }
        }
        if (group != null && (group.GetUnits().Count > 1 || group.IsHosted()))
        {
            global::MOM.Group group2 = new global::MOM.Group(group.GetPlane(), group.GetOwnerID());
            group2.Position = group.GetPosition();
            group2.AddUnit(group.transporter);
            group2.GetMapFormation(group.GetMapFormation(createIfMissing: false) != null);
            group2.RegainTo1MP();
            group = group2;
        }
        return group;
    }

    public void UpdatetargetIfCloser(AIWarArmy army, AIWarTarget target)
    {
        int num = this.GatedDistance(army.target.GetGroup(), army.GetPlane(), army.GetPosition());
        if (this.GatedDistance(target.GetGroup(), army.GetPlane(), army.GetPosition()) < num)
        {
            army.SetTarget(target);
        }
    }

    public SingleRegion GetClosestEnemyRegion(IPlanePosition a, List<SingleRegion> regions)
    {
        if (regions == null || regions.Count < 1)
        {
            return null;
        }
        int num = 10000;
        SingleRegion result = null;
        foreach (SingleRegion region in regions)
        {
            int num2 = this.GatedDistance(a, region);
            if (num2 < num)
            {
                num = num2;
                result = region;
            }
        }
        return result;
    }

    public void IdentifyEnemyExpeditions()
    {
    }

    public void IdentifyOwnExpeditions()
    {
    }

    public void ActToProvideTransport()
    {
    }

    public void ActToProvideArmies(List<SingleRegion> ownRegions, List<SingleRegion> enemyRegions)
    {
        PlayerWizardAI playerWizardAI = this.owner.Get() as PlayerWizardAI;
        int count = this.armies.FindAll((AIWarArmy o) => (o.group?.Get().GetUnits().Count ?? 0) > 0).Count;
        int wizardTownCount = GameManager.GetWizardTownCount(playerWizardAI.ID);
        int num = 2 + Mathf.CeilToInt((float)wizardTownCount * 0.15f) - count;
        if (num <= 0)
        {
            return;
        }
        List<global::MOM.Group> list = new List<global::MOM.Group>();
        foreach (global::MOM.Group ownGroup in playerWizardAI.GetOwnGroups())
        {
            if (ownGroup.doomStack && !ownGroup.IsHosted() && ownGroup.GetUnits().Count > 5 && playerWizardAI.GetWareffortForGroup(ownGroup) == null)
            {
                list.Add(ownGroup);
            }
        }
        if (list.Count > num)
        {
            Dictionary<global::MOM.Group, int> distances = new Dictionary<global::MOM.Group, int>();
            foreach (global::MOM.Group item in list)
            {
                int num2 = int.MaxValue;
                if (enemyRegions != null)
                {
                    foreach (SingleRegion enemyRegion in enemyRegions)
                    {
                        int num3 = this.GatedDistance(item, enemyRegion.plane, enemyRegion.center);
                        if (num2 > num3)
                        {
                            num2 = num3;
                        }
                    }
                }
                distances[item] = num2;
            }
            list.Sort((global::MOM.Group a, global::MOM.Group b) => distances[a].CompareTo(distances[b]));
        }
        for (int i = 0; i < list.Count; i++)
        {
            global::MOM.Group group = list[i];
            AIWarArmy aIWarArmy = new AIWarArmy(group.GetPosition(), group.GetPlane().arcanusType);
            aIWarArmy.group = group;
            this.armies.Add(aIWarArmy);
            aIWarArmy.owner = GameManager.GetWizard(group.GetOwnerID());
            group.GetDesignation().NewDesignation(AIGroupDesignation.Designation.AggressionLong, null);
            num--;
            if (num == 0)
            {
                break;
            }
        }
        if (this.armies == null)
        {
            return;
        }
        foreach (AIWarArmy army in this.armies)
        {
            army.armyMobilityDesignation = AIWarArmy.Mobility.UseWalk;
            army.target = null;
        }
    }

    public void ActToCreateWaterSuperiority()
    {
    }

    public IEnumerator MoveArmy(AIWarArmy army, List<Vector3i> desiredPath, bool seekDangersOrTargets = true)
    {
        if (desiredPath == null)
        {
            if (seekDangersOrTargets)
            {
                yield return this.MoveBasedOnDangersAndAchievability(army, 8);
            }
            yield break;
        }
        if (seekDangersOrTargets)
        {
            yield return this.MoveBasedOnDangersAndAchievability(army, 3, heavyEstimatorOnly: true);
        }
        if (!(army?.group?.Get().alive).GetValueOrDefault() || army.group.Get().CurentMP() == 0)
        {
            yield break;
        }
        global::MOM.Group g = army.group.Get();
        int num = desiredPath.FindIndex((Vector3i o) => o == g.GetPosition());
        if (num == -1 || num == desiredPath.Count - 1)
        {
            yield break;
        }
        desiredPath = desiredPath.GetRange(num, desiredPath.Count - num);
        bool goThroughPath = true;
        if (!g.landMovement || !g.waterMovement)
        {
            for (int i = 1; i < desiredPath.Count; i++)
            {
                Hex hexAt = g.GetPlane().GetHexAt(desiredPath[i]);
                if (hexAt.IsLand() && !g.landMovement)
                {
                    if (i > 1)
                    {
                        List<Vector3i> range = desiredPath.GetRange(0, i);
                        army.group = army.group.Get().MoveViaPath(range, mergeCollidedAlliedGroups: false, enterCollidedAlliedTowns: false) as global::MOM.Group;
                        yield return this.WaitForFocus();
                    }
                    if (army.GetPosition() != desiredPath[i - 1])
                    {
                        break;
                    }
                    List<Reference<global::MOM.Unit>> list = new List<Reference<global::MOM.Unit>>();
                    foreach (Reference<global::MOM.Unit> unit in army.group.Get().GetUnits())
                    {
                        if (!unit.Get().CanTravelOverLand())
                        {
                            list.Add(unit);
                        }
                    }
                    if (list.Count < army.group.Get().GetUnits().Count && list.Count > 0)
                    {
                        goThroughPath = false;
                        global::MOM.Group obj = new global::MOM.Group(army.GetPlane(), army.group.Get().GetOwnerID())
                        {
                            Position = army.GetPosition()
                        };
                        obj.AddUnitsIfPossible(list);
                        obj.GetMapFormation(army.group.Get().GetMapFormation(createIfMissing: false) != null);
                        army.group.Get().RegainTo1MP();
                        List<Vector3i> range2 = desiredPath.GetRange(i - 1, desiredPath.Count - i + 1);
                        army.group = army.group.Get().MoveViaPath(range2, mergeCollidedAlliedGroups: true, enterCollidedAlliedTowns: false) as global::MOM.Group;
                        yield return this.WaitForFocus();
                    }
                    break;
                }
                if (!hexAt.IsLand() && !g.waterMovement)
                {
                    if (i > 1)
                    {
                        goThroughPath = false;
                        List<Vector3i> range3 = desiredPath.GetRange(0, i);
                        army.group = army.group.Get().MoveViaPath(range3, mergeCollidedAlliedGroups: false, enterCollidedAlliedTowns: false) as global::MOM.Group;
                        yield return this.WaitForFocus();
                    }
                    break;
                }
            }
        }
        if (goThroughPath && g.CurentMP() > 0)
        {
            army.group = army.group.Get().MoveViaPath(desiredPath, mergeCollidedAlliedGroups: false, enterCollidedAlliedTowns: false) as global::MOM.Group;
            yield return this.WaitForFocus();
        }
    }

    public bool PrepareByWarCasting()
    {
        if (this.armies == null || this.armies.Count < 1)
        {
            return false;
        }
        foreach (AIWarArmy army in this.armies)
        {
            if (!(army.needOfSpell != null) || army.group?.Get() == null || !army.group.Get().alive)
            {
                continue;
            }
            if (!(bool)ScriptLibrary.Call("CounterMagicWorld", army.needOfSpell.Get(), this.owner.Get()))
            {
                if (army.needOfSpell.Get() == (Spell)SPELL.DJINN)
                {
                    if (army.group.Get().GetUnits().Count > 8)
                    {
                        army.group.Get().KickOutOneUnit();
                    }
                    global::MOM.Unit u = global::MOM.Unit.CreateFrom((global::DBDef.Unit)(Enum)UNIT.SOR_DJINN);
                    army.group.Get().AddUnit(u);
                }
                else if (army.needOfSpell.Get() == (Spell)SPELL.WIND_WALKING || army.needOfSpell.Get() == (Spell)SPELL.WRAITH_FORM || army.needOfSpell.Get() == (Spell)SPELL.WATER_WALKING || army.needOfSpell.Get() == (Spell)SPELL.FLIGHT)
                {
                    foreach (Reference<global::MOM.Unit> unit in army.group.Get().GetUnits())
                    {
                        if (!unit.Get().CanTravelOverWater())
                        {
                            Spell spell = army.needOfSpell.Get();
                            ScriptLibrary.Call(spell.worldScript, this.owner.Get(), unit, spell);
                            return true;
                        }
                    }
                }
                else if (army.needOfSpell.Get() == (Spell)SPELL.FLOATING_ISLAND)
                {
                    if (army.group.Get().GetUnits().Count > 8)
                    {
                        army.group.Get().KickOutOneUnit();
                    }
                    army.group.Get().AddUnit(global::MOM.Unit.CreateFrom((global::DBDef.Unit)(Enum)UNIT.SOR_FLOATING_ISLAND));
                }
                return true;
            }
            return true;
        }
        return false;
    }

    public IEnumerator WaitForFocus()
    {
        while (!GameManager.Get().IsFocusFree())
        {
            yield return null;
        }
    }

    public IEnumerator IsTargetAchievable(global::MOM.Group attacker, global::MOM.Group defender, AIAchievability aIAchievability)
    {
        if (attacker == null || defender == null)
        {
            yield break;
        }
        if (defender.GetUnits().Count == 0)
        {
            aIAchievability.targetIsAchievable = true;
            aIAchievability.targetPosesSeriousDanger = false;
            aIAchievability.anyAdvantagePossible = true;
            yield break;
        }
        if (attacker.GetUnits().Count == 0)
        {
            aIAchievability.targetIsAchievable = false;
            aIAchievability.targetPosesSeriousDanger = false;
            aIAchievability.anyAdvantagePossible = false;
            yield break;
        }
        bool disposable = attacker.GetUnits().Count > 6;
        Battle b = Battle.Create(attacker, defender);
        BattleResult battleResult = new BattleResult();
        yield return PowerEstimate.SimulatedBattle(b, 3, battleResult);
        if (disposable)
        {
            aIAchievability.targetIsAchievable = battleResult.dWinAveraged < 5;
        }
        else
        {
            aIAchievability.targetIsAchievable = battleResult.aWinAveraged >= 3;
        }
        aIAchievability.targetPosesSeriousDanger = battleResult.aWinAveraged <= 1;
        aIAchievability.anyAdvantagePossible = battleResult.dWinAveraged < 5;
        b.Destroy();
    }

    public IEnumerator MoveBasedOnDangersAndAchievability(AIWarArmy army, int actionRange, bool heavyEstimatorOnly = false)
    {
        PlayerWizardAI ai = this.owner.Get() as PlayerWizardAI;
        int armyValue = army.group.Get().GetValue();
        global::MOM.Group danger = null;
        int dangerDist = int.MaxValue;
        global::MOM.Group target = null;
        int targetDist = int.MaxValue;
        global::MOM.Group dangerGroup = null;
        foreach (global::MOM.Group v in ai.GetVisibleGroups())
        {
            if (((v.GetOwnerID() <= 0 || !ai.GetDiplomacy().IsAtWarWith(v.GetOwnerID())) && v.IsHosted()) || (v.IsHosted() && v.GetUnits().Count == 0 && !(v.GetLocationHost() is TownLocation)) || army.target?.GetGroup() == v)
            {
                continue;
            }
            int dist = this.GatedDistance(army, v);
            if (dist > actionRange)
            {
                continue;
            }
            int value = v.GetValue();
            if (!heavyEstimatorOnly && (float)value > (float)armyValue * 2f && dangerDist > dist)
            {
                danger = v;
                dangerGroup = v;
                dangerDist = dist;
            }
            else if ((float)value < (float)armyValue * 0.5f && targetDist > dist)
            {
                target = v;
                targetDist = dist;
            }
            else if ((float)value <= (float)armyValue * 2f && (targetDist > dist || dangerDist > dist))
            {
                AIAchievability a = new AIAchievability();
                yield return this.IsTargetAchievable(army.group.Get(), v, a);
                if (a.targetIsAchievable && targetDist > dist)
                {
                    target = v;
                    targetDist = dist;
                }
                else if (a.anyAdvantagePossible && targetDist > dist && (army?.group.Get().GetUnits().Count ?? 0) > 6)
                {
                    target = v;
                    targetDist = dist;
                }
                else if (a.targetPosesSeriousDanger && dangerDist > dist)
                {
                    danger = v;
                    dangerGroup = v;
                    dangerDist = dist;
                }
                else
                {
                    dangerGroup = v;
                }
            }
        }
        if (dangerGroup != null)
        {
            AIPlaneVisibility planeVisibility = ai.GetPlaneVisibility(dangerGroup.GetPlane());
            if (planeVisibility != null)
            {
                TownLocation townLocation = null;
                int num = int.MaxValue;
                foreach (global::MOM.Location knownLocation in planeVisibility.knownLocations)
                {
                    if (knownLocation.owner == ai.GetID() && knownLocation is TownLocation)
                    {
                        int distanceTo = knownLocation.GetDistanceTo(army.group.Get());
                        if (distanceTo <= 6 && knownLocation.locationTactic.dangerRank >= 2 && 9 >= knownLocation.GetUnits().Count + army.group.Get().GetUnits().Count && distanceTo < num)
                        {
                            num = distanceTo;
                            townLocation = knownLocation as TownLocation;
                        }
                    }
                }
                if (townLocation != null)
                {
                    army.group.Get().GetDesignation().NewDesignation(AIGroupDesignation.Designation.Transfer, new Destination(townLocation, aggressive: false));
                    yield break;
                }
            }
        }
        if (danger != null && target != null)
        {
            if (this.GatedDistance(danger, target) > dangerDist)
            {
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(army.GetPlane(), army.GetPosition(), target.GetPosition(), army.group.Get());
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                yield return this.MoveArmy(army, path, seekDangersOrTargets: false);
            }
        }
        else if (target != null)
        {
            RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(army.GetPlane(), army.GetPosition(), target.GetPosition(), army.group.Get());
            PathfinderV2.FindPath(requestDataV2);
            List<Vector3i> path2 = requestDataV2.GetPath();
            if (path2 != null && path2.Count > 1)
            {
                yield return this.MoveArmy(army, path2, seekDangersOrTargets: false);
            }
        }
        if (danger == null || !(army?.group?.Get().alive).GetValueOrDefault() || army.group.Get().CurentMP() == 0)
        {
            yield break;
        }
        RequestDataV2 requestDataV3 = RequestDataV2.CreateRequest(army.GetPlane(), army.GetPosition(), new FInt(army.group.Get().CurentMP().ToInt() + 4), army.group.Get());
        PathfinderV2.FindArea(requestDataV3);
        List<Vector3i> area = requestDataV3.GetArea();
        if (area == null || area.Count <= 1)
        {
            yield break;
        }
        Vector3i vector3i = Vector3i.invalid;
        int num2 = 0;
        foreach (Vector3i item in area)
        {
            int num3 = this.GatedDistance(danger, army.GetPlane(), item);
            if (num3 > num2)
            {
                num2 = num3;
                vector3i = item;
            }
        }
        if (vector3i != Vector3i.invalid)
        {
            List<Vector3i> pathTo = requestDataV3.GetPathTo(vector3i);
            yield return this.MoveArmy(army, pathTo, seekDangersOrTargets: false);
        }
    }

    public IEnumerator OptimizeArmy(AIWarArmy army)
    {
        PlayerWizardAI playerWizardAI = this.owner.Get() as PlayerWizardAI;
        if (playerWizardAI.GetOwnGroups() == null)
        {
            yield break;
        }
        global::MOM.Group g = army.group.Get();
        int count = g.GetUnits().Count;
        int num = this.armies.IndexOf(army);
        List<Reference<global::MOM.Unit>> offeredUnits = null;
        int minUnitValue = Mathf.Clamp(TurnManager.GetTurnNumber() * 3, 0, 600);
        if (count >= 8)
        {
            int num2 = int.MaxValue;
            foreach (Reference<global::MOM.Unit> unit in g.GetUnits())
            {
                int worldUnitValue = unit.Get().GetWorldUnitValue();
                if (worldUnitValue < num2)
                {
                    num2 = worldUnitValue;
                }
            }
            minUnitValue = Mathf.Max(num2, minUnitValue);
        }
        foreach (global::MOM.Group v in playerWizardAI.GetOwnGroups())
        {
            if (v == g || v.GetPlane() != g.GetPlane() || v.GetDistanceTo(g) >= 6 || (playerWizardAI.GetWareffortForGroup(v) != null && this.armies.FindIndex((AIWarArmy o) => o.group == v) <= num))
            {
                continue;
            }
            if (offeredUnits == null)
            {
                offeredUnits = new List<Reference<global::MOM.Unit>>();
            }
            if (army.armyMobilityDesignation == AIWarArmy.Mobility.UseWalk)
            {
                offeredUnits.AddRange(v.GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().GetWorldUnitValue() > minUnitValue));
                continue;
            }
            offeredUnits.AddRange(v.GetUnits().FindAll(delegate(Reference<global::MOM.Unit> o)
            {
                int worldUnitValue2 = o.Get().GetWorldUnitValue();
                return (o.Get().CanTravelOverLand() && o.Get().CanTravelOverWater() && worldUnitValue2 > minUnitValue) || (float)worldUnitValue2 > (float)minUnitValue * 1.5f;
            }));
        }
        if (offeredUnits == null || offeredUnits.Count <= 0)
        {
            yield break;
        }
        if (offeredUnits.Count > 1)
        {
            Dictionary<global::MOM.Unit, int> values2 = new Dictionary<global::MOM.Unit, int>();
            foreach (Reference<global::MOM.Unit> item in offeredUnits)
            {
                values2[item.Get()] = item.Get().GetWorldUnitValue();
            }
            offeredUnits.Sort((Reference<global::MOM.Unit> a, Reference<global::MOM.Unit> b) => -values2[a].CompareTo(values2[b]));
            offeredUnits = offeredUnits.FindAll((Reference<global::MOM.Unit> o) => o.Get().group == offeredUnits[0].Get().group);
        }
        Reference<global::MOM.Group> group = offeredUnits[0].Get().group;
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(army.GetPlane(), army.GetPosition(), group.Get().GetPosition(), g);
        PathfinderV2.FindPath(requestDataV);
        List<Vector3i> path = requestDataV.GetPath();
        path?.RemoveAt(path.Count - 1);
        if (path != null && path.Count > 2)
        {
            g.MoveViaPath(path, mergeCollidedAlliedGroups: false);
            yield return this.WaitForFocus();
        }
        if (g.CurentMP() == 0)
        {
            yield break;
        }
        global::MOM.Group group2 = offeredUnits[0].Get().group.Get();
        offeredUnits.AddRange(army.group.Get().GetUnits());
        if (offeredUnits.Count <= 8)
        {
            foreach (Reference<global::MOM.Unit> item2 in offeredUnits)
            {
                if (item2.Get().group != g)
                {
                    g.AddUnit(item2.Get(), updateMovementFlags: false);
                }
            }
            g.UpdateMovementFlags();
            yield break;
        }
        if (offeredUnits.Count > 1)
        {
            Dictionary<global::MOM.Unit, int> values = new Dictionary<global::MOM.Unit, int>();
            foreach (Reference<global::MOM.Unit> item3 in offeredUnits)
            {
                values[item3.Get()] = item3.Get().GetWorldUnitValue();
            }
            offeredUnits.Sort((Reference<global::MOM.Unit> a, Reference<global::MOM.Unit> b) => -values[a].CompareTo(values[b]));
        }
        int count2 = g.GetUnits().FindAll((Reference<global::MOM.Unit> o) => offeredUnits.IndexOf(o.Get()) == -1).Count;
        for (int i = 0; i < offeredUnits.Count; i++)
        {
            Reference<global::MOM.Unit> reference = offeredUnits[i];
            if (count2 + i <= 8)
            {
                if (reference.Get().group == group2)
                {
                    reference.Get().group.Get().RemoveUnit(reference, allowGroupDestruction: false, updateGroup: false);
                }
            }
            else if (reference.Get().group == g)
            {
                reference.Get().group.Get().RemoveUnit(reference, allowGroupDestruction: false, updateGroup: false);
            }
        }
        for (int j = 0; j < offeredUnits.Count; j++)
        {
            Reference<global::MOM.Unit> reference2 = offeredUnits[j];
            if (j + count2 <= 8)
            {
                if (reference2.Get().group?.Get() == null)
                {
                    g.AddUnit(reference2);
                }
            }
            else if (reference2.Get().group?.Get() == null)
            {
                group2.AddUnit(reference2);
            }
        }
        if (g.IsHosted())
        {
            for (int num3 = g.GetUnits().Count - 1; num3 >= 0; num3--)
            {
                if (!g.GetUnits()[num3].Get().CanTravelOverLand())
                {
                    g.GetUnits()[num3].Get().Destroy();
                }
            }
        }
        if (group2.IsHosted())
        {
            for (int num4 = group2.GetUnits().Count - 1; num4 >= 0; num4--)
            {
                if (!group2.GetUnits()[num4].Get().CanTravelOverLand())
                {
                    group2.GetUnits()[num4].Get().Destroy();
                }
            }
        }
        if (g.GetUnits().Count == 0 && !g.IsHosted())
        {
            g.Destroy();
        }
        if (group2.GetUnits().Count == 0 && !group2.IsHosted())
        {
            group2.Destroy();
        }
    }
}
