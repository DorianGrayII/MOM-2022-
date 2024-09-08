// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AIGroupDesignation
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
public class AIGroupDesignation
{
    public enum Designation
    {
        None = 0,
        Settler = 1,
        Melder = 2,
        Engineer = 3,
        Retreat = 4,
        Defend = 5,
        Transfer = 6,
        AggressionShort = 7,
        AggressionMedium = 8,
        AggressionLong = 9,
        MAX = 10
    }

    [ProtoMember(1)]
    public Designation designation;

    [ProtoMember(2)]
    public Destination destinationPosition;

    [ProtoMember(3)]
    public Vector3i dataPosition;

    [ProtoMember(4)]
    public Reference<global::MOM.Group> owner;

    [ProtoMember(5)]
    public bool inNeedOfWaterTransport;

    [ProtoMember(6)]
    public AIGroupTransition groupTransition;

    [ProtoMember(7)]
    public int transportSeekingDelay;

    [ProtoMember(8)]
    public int waitBehaviour;

    [ProtoMember(9)]
    public bool actOrResuplyOthers;

    [ProtoMember(10)]
    public int lastDoomstackAttempt;

    [ProtoIgnore]
    public List<Multitype<int, int>> ignoredLocations;

    [ProtoIgnore]
    public int nextMultistageRequest;

    public AIGroupDesignation()
    {
    }

    public AIGroupDesignation(global::MOM.Group g)
    {
        if (g.GetLocationHostSmart() != null)
        {
            Debug.LogError("Group Designation system is for roaming groups only, not for stationary groups belonging to the location");
        }
        this.owner = g;
        if (g.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsSettler()) != null)
        {
            this.designation = Designation.Settler;
            this.destinationPosition = null;
        }
        else
        {
            this.designation = Designation.AggressionShort;
        }
    }

    public void NewOwner(global::MOM.Group g)
    {
        this.owner.Get().aiDesignation = null;
        this.owner = g;
        g.aiDesignation = this;
    }

    public void NewDesignation(Designation d, Destination destination)
    {
        this.designation = d;
        this.destinationPosition = destination;
        this.ignoredLocations = null;
        this.waitBehaviour = 0;
        if (this.groupTransition != null)
        {
            this.groupTransition.ClearTransition();
        }
    }

    public void NewDesignation(Designation d, Vector3i designationPosition)
    {
        this.designation = d;
        this.dataPosition = Vector3i.invalid;
        if (designationPosition != Vector3i.invalid)
        {
            bool aggressive = false;
            if (d == Designation.Defend || d == Designation.AggressionShort || d == Designation.AggressionMedium || d == Designation.AggressionLong)
            {
                aggressive = true;
            }
            this.destinationPosition = new Destination(designationPosition, this.GetPlane().arcanusType, aggressive);
        }
        else
        {
            this.destinationPosition = null;
        }
        this.ignoredLocations = null;
        this.waitBehaviour = 0;
        if (this.groupTransition != null)
        {
            this.groupTransition.ClearTransition();
        }
    }

    public void NewDesignation(Designation d, Vector3i designationPosition, Vector3i dataPosition)
    {
        this.designation = d;
        this.dataPosition = dataPosition;
        if (designationPosition != Vector3i.invalid)
        {
            bool aggressive = false;
            if (d == Designation.Defend || d == Designation.AggressionShort || d == Designation.AggressionMedium || d == Designation.AggressionLong)
            {
                aggressive = true;
            }
            this.destinationPosition = new Destination(designationPosition, this.GetPlane().arcanusType, aggressive);
        }
        else
        {
            this.destinationPosition = null;
        }
        this.ignoredLocations = null;
        this.waitBehaviour = 0;
        if (this.groupTransition != null)
        {
            this.groupTransition.ClearTransition();
        }
    }

    private IEnumerator FindPlaceToSettleUsingPathfinding(int range, Vector3i position, global::WorldCode.Plane plane, bool allowLocalPlaneSwitch = true, bool clearPreviousDestination = true)
    {
        DataHeatMaps map = DataHeatMaps.Get(plane);
        while (!map.IsMapReady(DataHeatMaps.HMType.SettlementValue, this.GetWizard()))
        {
            yield return null;
        }
        HeatMap heatMap = map.GetHeatMap(DataHeatMaps.HMType.SettlementValue);
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, position, new FInt(range), this.owner.Get());
        PathfinderV2.FindArea(requestDataV);
        List<Vector3i> area = requestDataV.GetArea();
        SearcherDataV2 searcherData = plane.GetSearcherData();
        if (clearPreviousDestination)
        {
            this.destinationPosition = null;
        }
        int num = ((this.destinationPosition != null) ? this.destinationPosition.data1 : (-1));
        List<Vector3i> list = ((!(this.GetWizard() is PlayerWizardAI playerWizardAI) || !playerWizardAI.GetMoveManager().ValidateTransportFor(this.GetGroup())) ? area : HexNeighbors.GetRange(position, range));
        foreach (Vector3i item in list)
        {
            Hex hexAtWrapped = plane.GetHexAtWrapped(item);
            if (hexAtWrapped == null || !hexAtWrapped.IsLand())
            {
                continue;
            }
            int num2 = heatMap.GetValue(item);
            if (!area.Contains(item))
            {
                num2 = (int)((float)num2 * 0.8f);
            }
            if (num2 > 6 && num2 > num && searcherData.locations != null)
            {
                int index = searcherData.GetIndex(item);
                if (index == -1)
                {
                    Debug.LogWarning("Searcher data does not contains location seek by group designation!");
                    break;
                }
                if (!searcherData.locations[index])
                {
                    this.destinationPosition = new Destination(item, plane.arcanusType, aggressive: false);
                    this.destinationPosition.data1 = num2;
                    num = num2;
                }
            }
        }
        if (!allowLocalPlaneSwitch)
        {
            yield break;
        }
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (entity.Value is global::MOM.Location location && location.locationType == ELocationType.PlaneTower && area.Contains(location.GetPosition()) && location.GetPlane() == plane && (location.GetUnits().Count == 0 || location.GetOwnerID() == this.GetWizardID()))
            {
                int distanceWrapping = plane.GetDistanceWrapping(location.GetPosition(), position);
                int num3 = range - distanceWrapping;
                global::WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
                if (num3 > 1)
                {
                    yield return this.FindPlaceToSettleUsingPathfinding(num3, location.GetPosition(), otherPlane, allowLocalPlaneSwitch: false, clearPreviousDestination: false);
                }
            }
        }
    }

    private void FindNodeToMeld()
    {
        global::MOM.Location location = (this.GetWizard() as PlayerWizardAI).FindNodeToMeld(this.GetGroup());
        if (location != null)
        {
            Destination destination = new Destination(location, aggressive: true);
            this.destinationPosition = destination;
        }
    }

    public IEnumerator ResolveTurn()
    {
        global::MOM.Group group = this.owner.Get();
        if (group.CurentMP() == 0)
        {
            yield break;
        }
        AIPriorityTargets aIPriorityTargets = (this.GetWizard() as PlayerWizardAI)?.priorityTargets;
        if ((aIPriorityTargets?.aiTargets?.Count).GetValueOrDefault() > 0)
        {
            AITarget assignedTarget = aIPriorityTargets.GetAssignedTarget(group, useAdvancedPrioritization: true);
            if (assignedTarget != null && this.designation != Designation.Defend && this.designation != Designation.AggressionShort && this.designation != Designation.AggressionMedium && this.designation != Designation.AggressionLong)
            {
                Destination destination = null;
                if (assignedTarget.targetAchievable && !assignedTarget.preparationOnly)
                {
                    destination = new Destination(assignedTarget.target.Get() as IPlanePosition, aggressive: true);
                }
                this.NewDesignation(Designation.AggressionShort, destination);
            }
        }
        if (this.designation == Designation.None)
        {
            this.BehaviourNone();
        }
        if (this.designation == Designation.Settler)
        {
            yield return this.BehaviourSettler();
        }
        if (this.designation == Designation.Engineer)
        {
            yield return this.BehaviourEngineer();
        }
        if (this.designation == Designation.Melder)
        {
            yield return this.BehaviourMelder();
        }
        if (this.designation == Designation.Defend)
        {
            yield return this.BehaviourAggression(8, onlyExpeditions: true);
        }
        if (this.designation == Designation.AggressionShort)
        {
            yield return this.BehaviourAggression(12, onlyExpeditions: false);
        }
        if (this.designation == Designation.AggressionMedium)
        {
            yield return this.BehaviourAggression(20, onlyExpeditions: false);
        }
        if (this.designation == Designation.AggressionLong)
        {
            yield return this.BehaviourAggression(28, onlyExpeditions: false);
        }
        if (this.designation == Designation.Transfer)
        {
            yield return this.BehaviourTransfer();
        }
        if (this.designation == Designation.Retreat)
        {
            yield return this.BehaviourRetreat();
        }
    }

    private void BehaviourNone()
    {
        if (this.GetGroup().GetUnits() != null)
        {
            if (this.GetGroup().GetUnits().FindIndex((Reference<global::MOM.Unit> o) => o.Get().GetAttFinal(TAG.SETTLER_UNIT) > 0) >= 0)
            {
                this.NewDesignation(Designation.Settler, Vector3i.invalid);
            }
            else
            {
                this.NewDesignation(Designation.AggressionShort, this.GetPosition());
            }
        }
    }

    private IEnumerator BehaviourSettler()
    {
        global::MOM.Unit u = this.GetGroup().KickOutEngineer();
        global::MOM.Group group = u?.group?.Get();
        if (group != null)
        {
            yield return group.GetDesignation().ResolveTurn();
        }
        this.GetGroup().KickOutMelder();
        global::MOM.Group group2 = u?.group?.Get();
        if (group2 != null)
        {
            yield return group2.GetDesignation().ResolveTurn();
        }
        List<Reference<global::MOM.Unit>> list = this.GetGroup().GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().IsSettler());
        if (list.Count == 0)
        {
            if (this.GetGroup().GetUnits().Count > 0)
            {
                this.NewDesignation(Designation.AggressionShort, null);
            }
            yield break;
        }
        if (list.Count > 1)
        {
            global::MOM.Group group3 = this.GetGroup().KickOutSettler()?.group?.Get();
            if (group3 != null)
            {
                yield return group3.GetDesignation().ResolveTurn();
            }
        }
        if (this.destinationPosition != null)
        {
            List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.destinationPosition.arcanus);
            int townDistance = DifficultySettingsData.GetTownDistance();
            foreach (global::MOM.Location item in locationsOfThePlane)
            {
                if (this.destinationPosition.Reached(item))
                {
                    this.destinationPosition = null;
                    break;
                }
                if (item is TownLocation && this.Distance(item.GetPosition(), this.destinationPosition.position) <= townDistance)
                {
                    this.destinationPosition = null;
                    break;
                }
            }
        }
        if (this.destinationPosition == null)
        {
            yield return this.FindPlaceToSettleUsingPathfinding(12, this.GetPosition(), this.GetPlane());
            if (this.destinationPosition == null)
            {
                yield return this.FindPlaceToSettleUsingPathfinding(20, this.GetPosition(), this.GetPlane());
            }
        }
        if (this.destinationPosition != null)
        {
            yield return PlayerWizardAI.MoveGroup(this.GetGroup(), this.destinationPosition.position, this.destinationPosition.arcanus);
        }
        else
        {
            Reference<global::MOM.Unit> reference = this.owner.Get().GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsSettler());
            if (reference != null)
            {
                this.GetGroup().SellUnit(reference);
            }
        }
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            yield return null;
        }
        if (this.destinationPosition == null || (this.destinationPosition.Reached(this.owner.Get()) && !this.GetGroup().BuildTown()))
        {
            if (!(this.owner.Get().GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsSettler()) != null))
            {
                this.NewDesignation(Designation.Retreat, null);
            }
        }
        else
        {
            while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
            {
                yield return null;
            }
        }
    }

    private IEnumerator BehaviourMelder()
    {
        this.GetGroup().KickOutSettler();
        this.GetGroup().KickOutEngineer();
        List<Reference<global::MOM.Unit>> list = this.GetGroup().GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().IsMelder());
        if (list.Count == 0)
        {
            if (this.GetGroup().GetUnits().Count > 0)
            {
                this.NewDesignation(Designation.AggressionShort, null);
            }
            yield break;
        }
        if (list.Count > 1)
        {
            this.GetGroup().KickOutMelder();
        }
        if (this.destinationPosition != null)
        {
            global::MOM.Location location = GameManager.GetLocationsOfThePlane(this.destinationPosition.arcanus).Find((global::MOM.Location o) => o.GetPosition() == this.destinationPosition.position);
            if (location == null)
            {
                this.destinationPosition = null;
            }
            else
            {
                int num = location.melding?.meldOwner ?? (-1);
                if (num == this.GetWizardID())
                {
                    this.destinationPosition = null;
                }
                else if (location.GetUnits().Count > 0)
                {
                    this.destinationPosition = null;
                }
                else
                {
                    DiplomaticStatus statusToward = this.GetWizard().GetDiplomacy().GetStatusToward(num);
                    if (statusToward != null && !statusToward.openWar)
                    {
                        this.destinationPosition = null;
                    }
                }
            }
        }
        if (this.destinationPosition == null)
        {
            this.FindNodeToMeld();
        }
        if (this.destinationPosition != null)
        {
            global::MOM.Location loc = this.destinationPosition.GetAsLocation();
            int wizardID = this.GetGroup().GetOwnerID();
            yield return PlayerWizardAI.MoveGroup(this.GetGroup(), this.destinationPosition.position, this.destinationPosition.arcanus);
            if (loc == null)
            {
                yield break;
            }
            if (loc.GetOwnerID() == wizardID)
            {
                Reference<global::MOM.Unit> reference = loc.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsMelder());
                if (reference != null)
                {
                    loc.MeldAttempt(reference.Get());
                    reference.Get().Destroy();
                }
            }
            else
            {
                if (loc.GetUnits().Count != 0)
                {
                    yield break;
                }
                global::MOM.Group group = this.owner.Get();
                if (!(((group != null) ? new FInt?(group.CurentMP() + 1) : null) < loc.GetDistanceTo(group)))
                {
                    Reference<global::MOM.Unit> reference2 = group?.GetUnits()?.Find((Reference<global::MOM.Unit> o) => o.Get().IsMelder());
                    if (reference2 != null)
                    {
                        loc.MeldAttempt(reference2.Get());
                        reference2.Get().Destroy();
                    }
                }
            }
        }
        else
        {
            this.NewDesignation(Designation.Retreat, null);
        }
    }

    private IEnumerator BehaviourEngineer()
    {
        this.NewDesignation(Designation.Retreat, this.GetPosition());
        yield return null;
    }

    private IEnumerator BehaviourAggression(int range, bool onlyExpeditions)
    {
        this.GetPlane();
        global::MOM.Group g = this.GetGroup();
        this.GetPosition();
        if (!(GameManager.GetWizard(g.GetOwnerID()) is PlayerWizardAI playerWizardAI) || this.GetGroup().GetUnits().Count == 0)
        {
            yield break;
        }
        global::MOM.Group group = null;
        AIPlaneVisibility vis = playerWizardAI.GetPlaneVisibility(g.GetPlane());
        int num = 0;
        int num2 = int.MaxValue;
        foreach (global::MOM.Group visibleGroup in vis.visibleGroups)
        {
            if (visibleGroup.GetOwnerID() == this.GetWizardID() || g.GetDistanceTo(visibleGroup) > 4 || (visibleGroup.GetOwnerID() != 0 && !(playerWizardAI.GetDiplomacy()?.GetStatusToward(visibleGroup.GetOwnerID())?.openWar).GetValueOrDefault()) || (visibleGroup.IsHosted() && !(visibleGroup.locationHost.Get() is TownLocation) && visibleGroup.GetUnits().Count == 0))
            {
                continue;
            }
            int value = visibleGroup.GetValue();
            if (num == 0)
            {
                num = g.GetValue();
            }
            if (!((float)num > (float)value * 1.4f))
            {
                continue;
            }
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.GetPlane(), this.GetPosition(), visibleGroup.GetPosition(), this.owner.Get());
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path = requestDataV.GetPath();
            if (path != null && path.Count >= 2)
            {
                int num3 = path.Count - 1;
                if (num2 > num3)
                {
                    num2 = num3;
                    group = visibleGroup;
                }
            }
        }
        AIPriorityTargets aIPriorityTargets = (this.GetWizard() as PlayerWizardAI)?.priorityTargets;
        AITarget aiTarget = aIPriorityTargets?.GetAssignedTarget(g, useAdvancedPrioritization: true);
        if (group == null && aiTarget != null && aiTarget.ValidateTarget() && !aiTarget.preparationOnly)
        {
            if (aIPriorityTargets.aiTargets.IndexOf(aiTarget) > 0 && !aiTarget.targetAchievable)
            {
                aiTarget.RequestAchievabilityUpdate();
            }
            if (aiTarget.targetAchievable)
            {
                group = aiTarget.GetAsGroup();
            }
        }
        this.destinationPosition = null;
        if (group == null)
        {
            group = Destination.FindClosestPrey(this, range, onlyExpeditions);
        }
        if (group != null)
        {
            this.destinationPosition = new Destination(group, aggressive: true);
        }
        if (this.destinationPosition != null)
        {
            yield return PlayerWizardAI.MoveGroup(this.GetGroup(), this.destinationPosition.position, this.destinationPosition.arcanus);
        }
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            yield return null;
        }
        bool dStackAttempted = false;
        if (g.CurentMP() > 0)
        {
            if (this.designation == Designation.AggressionShort)
            {
                this.NewDesignation(Designation.AggressionMedium, null);
            }
            else if (this.designation == Designation.AggressionMedium)
            {
                dStackAttempted = true;
                if (this.CanAttemptDoomstack())
                {
                    yield return this.AttemptDoomstack();
                }
                if (this.actOrResuplyOthers)
                {
                    this.GetWizard();
                    if (vis.ownGroups != null)
                    {
                        global::MOM.Group group2 = null;
                        int num4 = int.MaxValue;
                        foreach (global::MOM.Group ownGroup in vis.ownGroups)
                        {
                            bool flag = false;
                            if (ownGroup.doomStack && ownGroup.GetUnits().Count < 9)
                            {
                                flag = true;
                            }
                            else
                            {
                                global::MOM.Location locationHostSmart = ownGroup.GetLocationHostSmart();
                                if (locationHostSmart == null || locationHostSmart.locationTactic == null)
                                {
                                    continue;
                                }
                                if (locationHostSmart.locationTactic.dangerRank > 1)
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                int distanceTo = this.GetGroup().GetDistanceTo(ownGroup);
                                if (distanceTo < num4)
                                {
                                    num4 = distanceTo;
                                    group2 = ownGroup;
                                }
                            }
                        }
                        if (group2 != null)
                        {
                            this.NewDesignation(Designation.AggressionMedium, new Destination(group2, aggressive: true));
                            yield break;
                        }
                    }
                }
                if (this.owner.Get().CurentMP() > 0 && g != null)
                {
                    if (aiTarget == null || (aiTarget?.supplyTowns?.Count).GetValueOrDefault() < 1)
                    {
                        this.NewDesignation(Designation.Retreat, null);
                    }
                    else
                    {
                        List<Reference<TownLocation>> obj = aiTarget?.supplyTowns;
                        TownLocation townLocation = null;
                        foreach (Reference<TownLocation> item in obj)
                        {
                            AILocationTactic aILocationTactic = item?.Get()?.locationTactic;
                            if (aILocationTactic != null && aILocationTactic.dangerRank > 1 && aILocationTactic.owner.Get() is TownLocation && townLocation != null && townLocation.GetPopUnits() < (aILocationTactic.owner.Get() as TownLocation).GetPopUnits())
                            {
                                Destination destination = new Destination(aILocationTactic.owner.Get(), aggressive: false);
                                this.NewDesignation(Designation.Retreat, destination);
                                yield break;
                            }
                        }
                    }
                }
            }
            else if (this.designation == Designation.Defend)
            {
                this.NewDesignation(Designation.AggressionShort, null);
            }
        }
        if (g.doomStack && !dStackAttempted && this.CanAttemptDoomstack())
        {
            yield return this.AttemptDoomstack();
        }
    }

    private bool CanAttemptDoomstack()
    {
        if (this.lastDoomstackAttempt == TurnManager.GetTurnNumber())
        {
            return false;
        }
        return true;
    }

    private IEnumerator AttemptDoomstack()
    {
        if (this.lastDoomstackAttempt == TurnManager.GetTurnNumber())
        {
            yield break;
        }
        this.lastDoomstackAttempt = TurnManager.GetTurnNumber();
        global::MOM.Group g = this.GetGroup();
        _ = g.GetUnits().Count;
        global::MOM.Group group = null;
        int num = 0;
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.GetPlane(), this.GetPosition(), new FInt(6 + settingAsInt * 3), this.owner.Get());
        PathfinderV2.FindArea(requestDataV);
        List<Vector3i> area = requestDataV.GetArea();
        if (area == null || area.Count < 2)
        {
            foreach (global::MOM.Group item in GameManager.GetGroupsOfPlane(this.GetPlane()))
            {
                if (item.GetOwnerID() != this.GetWizardID() || item == g || !area.Contains(item.GetPosition()))
                {
                    continue;
                }
                global::MOM.Location locationHostSmart = item.GetLocationHostSmart();
                if (locationHostSmart != null)
                {
                    if (item.GetUnits().Count == 1 || (locationHostSmart.locationTactic != null && locationHostSmart.locationTactic.dangerRank > 2))
                    {
                        continue;
                    }
                }
                else if (item.GetDesignation() != null)
                {
                    AIGroupDesignation aIGroupDesignation = item.GetDesignation();
                    if (aIGroupDesignation.designation != Designation.AggressionShort && aIGroupDesignation.designation != Designation.AggressionMedium && aIGroupDesignation.designation != Designation.AggressionLong && aIGroupDesignation.designation != Designation.Retreat)
                    {
                        continue;
                    }
                }
                if (locationHostSmart != null && (locationHostSmart.locationTactic == null || item.GetUnits().Count < locationHostSmart.locationTactic.MinimumExpectedUnits()))
                {
                    continue;
                }
                foreach (Reference<global::MOM.Unit> unit6 in item.GetUnits())
                {
                    global::MOM.Unit unit = unit6.Get();
                    if (!unit.IsSettler() && !unit.IsEngineer())
                    {
                        int worldUnitValue = unit.GetWorldUnitValue();
                        if (worldUnitValue > num)
                        {
                            num = worldUnitValue;
                            group = item;
                        }
                    }
                }
            }
        }
        if (group == null)
        {
            yield break;
        }
        List<Vector3i> pathTo = requestDataV.GetPathTo(group.GetPosition());
        if ((pathTo?.Count ?? 0) <= 1)
        {
            yield break;
        }
        if (pathTo[pathTo.Count - 1] == group.destination)
        {
            pathTo.RemoveLast();
        }
        g.MoveViaPath(pathTo, mergeCollidedAlliedGroups: true, enterCollidedAlliedTowns: false, aggressive: false);
        while (!GameManager.Get().IsFocusFree())
        {
            yield return null;
        }
        if (this.GetPlane().GetDistanceWrapping(g.GetPosition(), group.GetPosition()) > 1 || (group.GetUnits().Count <= 0 && g.GetUnits().Count <= 0))
        {
            yield break;
        }
        bool flag = group.GetLocationHost() != null;
        while (true)
        {
            int num2 = 0;
            global::MOM.Unit unit2 = null;
            if (g.GetUnits().Count == 9)
            {
                for (int i = 0; i < g.GetUnits().Count; i++)
                {
                    global::MOM.Unit unit3 = g.GetUnits()[i].Get();
                    if (i == 0)
                    {
                        num2 = unit3.GetWorldUnitValue();
                        unit2 = unit3;
                        continue;
                    }
                    int worldUnitValue2 = unit3.GetWorldUnitValue();
                    if (num2 > worldUnitValue2)
                    {
                        num2 = worldUnitValue2;
                        unit2 = unit3;
                    }
                }
            }
            int num3 = 0;
            global::MOM.Unit unit4 = null;
            if ((flag && group.GetUnits().Count < 2) || group.GetUnits().Count < 1)
            {
                for (int j = 0; j < group.GetUnits().Count; j++)
                {
                    global::MOM.Unit unit5 = group.GetUnits()[j].Get();
                    if (j == 0)
                    {
                        num3 = unit5.GetWorldUnitValue();
                        unit4 = unit5;
                        continue;
                    }
                    int worldUnitValue3 = unit5.GetWorldUnitValue();
                    if (num3 < worldUnitValue3)
                    {
                        num3 = worldUnitValue3;
                        unit4 = unit5;
                    }
                }
            }
            if (unit4 == null)
            {
                break;
            }
            if (unit2 != null)
            {
                if (unit4.GetWorldUnitValue() < unit2.GetWorldUnitValue())
                {
                    break;
                }
                g.RemoveUnit(unit2);
            }
            group.TransferUnit(g, unit4);
            Debug.Log("AttemptDoomstack Succeed from: \n  " + ((group != null) ? group.GetGroupString() : "Group Destroyed") + "\n TO \n" + this.GetGroup().GetGroupString());
            if (unit2 != null)
            {
                group.AddUnit(unit2);
            }
        }
    }

    private IEnumerator BehaviourTransfer()
    {
        if (this.destinationPosition == null)
        {
            this.NewDesignation(Designation.Retreat, null);
            yield break;
        }
        Vector3i p = this.owner.Get().GetPosition();
        FInt mp = this.owner.Get().CurentMP();
        yield return PlayerWizardAI.MoveGroup(this.GetGroup(), this.destinationPosition.position, this.destinationPosition.arcanus);
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            yield return null;
        }
        if (p == this.owner.Get().GetPosition() && mp > this.owner.Get().CurentMP() && this.owner.Get().GetUnits().Count > 0)
        {
            this.owner.Get().KickOutOneUnit();
        }
        if (this.owner.Get().CurentMP() > 0 && !this.destinationPosition.Reached(this))
        {
            this.NewDesignation(Designation.Retreat, null);
        }
    }

    private IEnumerator BehaviourRetreat()
    {
        Vector3i pos = this.owner.Get().GetPosition();
        if (!this.owner.Get().landMovement)
        {
            this.NewDesignation(Designation.AggressionShort, null);
        }
        else
        {
            if (this.owner == null || !this.owner.Get().alive)
            {
                yield break;
            }
            Destination destination = Destination.FindClosestFriendlyLocation(this);
            if (destination == null)
            {
                this.NewDesignation(Designation.AggressionShort, null);
                yield break;
            }
            yield return PlayerWizardAI.MoveGroup(this.GetGroup(), destination.position, destination.arcanus);
            while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
            {
                yield return null;
            }
            if (!(this.owner == null) && this.owner.Get().alive)
            {
                if (this.owner.Get().CurentMP() > 0 || pos == this.owner.Get().GetPosition())
                {
                    this.NewDesignation(Designation.AggressionShort, null);
                }
                else
                {
                    this.waitBehaviour = 0;
                }
            }
        }
    }

    private global::MOM.Group GetGroup()
    {
        return this.owner.Get();
    }

    public global::WorldCode.Plane GetPlane()
    {
        return this.owner.Get().GetPlane();
    }

    public Vector3i GetPosition()
    {
        return this.owner.Get().GetPosition();
    }

    public int GetWizardID()
    {
        return this.owner.Get().GetOwnerID();
    }

    public PlayerWizard GetWizard()
    {
        return GameManager.GetWizard(this.GetWizardID());
    }

    private int Distance(Vector3i a, Vector3i b)
    {
        return this.GetPlane().area.HexDistance(a, b);
    }
}
