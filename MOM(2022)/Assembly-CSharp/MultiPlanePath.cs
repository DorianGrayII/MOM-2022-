using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

public class MultiPlanePath
{
    private List<PlanePathStage> stages;

    private Destination destination;

    public MultiPlanePath(Destination d)
    {
        this.destination = d;
    }

    public bool PlanStaging(AIGroupDesignation a, FInt mpRange, Vector3i position, global::WorldCode.Plane plane, bool allowSwitch = true, bool withAvoidance = false)
    {
        object obj = this.BuildFullStaging(a, mpRange, position, plane, allowSwitch, withAvoidance);
        return this.PackStaging(obj);
    }

    private bool PackStaging(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is PlanePathStage)
        {
            this.stages = new List<PlanePathStage> { obj as PlanePathStage };
            return true;
        }
        if (obj is List<PlanePathStage>)
        {
            this.stages = obj as List<PlanePathStage>;
            return true;
        }
        Debug.LogError("unknown type returned from stage builder");
        return false;
    }

    private object BuildFullStaging(object acting, FInt mpRange, Vector3i position, global::WorldCode.Plane plane, bool allowSwitch = true, bool withAvoidance = false)
    {
        AIGroupDesignation aIGroupDesignation = acting as AIGroupDesignation;
        AIWarArmy aIWarArmy = acting as AIWarArmy;
        global::MOM.Group group = aIGroupDesignation?.owner.Get();
        if (group == null)
        {
            group = aIWarArmy.group;
        }
        object obj = null;
        if (plane.arcanusType == this.destination.arcanus)
        {
            obj = this.TrackWaterStaging(group, mpRange, position, plane);
        }
        if (obj is PlanePathStage)
        {
            return obj as PlanePathStage;
        }
        PlanePathStage planePathStage = (withAvoidance ? this.TrackAvoidanceStaging(group, mpRange, position, plane, allowSwitch) : this.TrackStaging(group, mpRange, position, plane, allowSwitch));
        if (obj is List<PlanePathStage> list)
        {
            PlanePathStage stage = list.Find((PlanePathStage o) => o.transportStage);
            PlayerWizardAI playerWizardAI = GameManager.GetWizard(group.GetOwnerID()) as PlayerWizardAI;
            if (playerWizardAI.WaitTimeForTransporter(stage, group) < 0)
            {
                return planePathStage;
            }
            if (planePathStage != null)
            {
                PlanePathStage planePathStage2 = list[list.Count - 1];
                if (planePathStage.mpCost <= planePathStage2.mpCost)
                {
                    return planePathStage;
                }
            }
            int num = playerWizardAI.WaitTimeForTransporter(stage, group, bookTransport: true);
            if (aIGroupDesignation != null)
            {
                aIGroupDesignation.nextMultistageRequest = TurnManager.GetTurnNumber() + num;
            }
            return list;
        }
        return planePathStage;
    }

    public object TrackWaterStaging(global::MOM.Group g, FInt mpRange, Vector3i position, global::WorldCode.Plane plane)
    {
        if (plane.arcanusType != this.destination.arcanus)
        {
            return null;
        }
        if (g.nonCorporealMovement || g.waterMovement)
        {
            return null;
        }
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, position, this.destination.position, g);
        if (!g.nonCorporealMovement && g.waterMovement != g.landMovement)
        {
            requestDataV.AllowEmbarkAtCost(2 + g.GetMaxMP() * 5);
        }
        requestDataV.MakeItAvoidanceSearch(g);
        PathfinderV2.FindPath(requestDataV);
        List<Vector3i> path = requestDataV.GetPath();
        if (path != null && path.Count > 1)
        {
            bool flag = plane.GetHexAt(path[0]).IsLand();
            List<int> list = new List<int>();
            for (int i = 1; i < path.Count; i++)
            {
                Vector3i positionWrapping = plane.GetPositionWrapping(path[i]);
                bool flag2 = plane.GetHexAt(positionWrapping).IsLand();
                if (flag != flag2)
                {
                    flag = flag2;
                    list.Add(i - 1);
                }
            }
            if (list.Count == 0)
            {
                PlanePathStage planePathStage = new PlanePathStage();
                planePathStage.path = path;
                planePathStage.arcanus = plane.arcanusType;
                planePathStage.mpCost = requestDataV.GetCostTo(planePathStage.path[planePathStage.path.Count - 1]);
                return planePathStage;
            }
            List<PlanePathStage> list2 = new List<PlanePathStage>();
            PlanePathStage planePathStage2 = new PlanePathStage();
            int num = 0;
            int num2 = list[0] + 1;
            planePathStage2.path = path.GetRange(num, num2 - num);
            planePathStage2.arcanus = plane.arcanusType;
            planePathStage2.mpCost = requestDataV.GetCostTo(planePathStage2.path[planePathStage2.path.Count - 1]);
            list2.Add(planePathStage2);
            bool flag3 = plane.GetHexAtWrapped((planePathStage2.path.Count > 1) ? planePathStage2.path[1] : planePathStage2.path[0]).IsLand();
            planePathStage2.transportStage = !flag3;
            for (int j = 1; j < list.Count; j++)
            {
                planePathStage2 = new PlanePathStage();
                num = list[j - 1];
                num2 = list[j] + 1;
                planePathStage2.path = path.GetRange(num, num2 - num);
                planePathStage2.arcanus = plane.arcanusType;
                planePathStage2.mpCost = requestDataV.GetCostTo(planePathStage2.path[planePathStage2.path.Count - 1]);
                list2.Add(planePathStage2);
                flag3 = plane.GetHexAtWrapped(planePathStage2.path[1]).IsLand();
                planePathStage2.transportStage = !flag3;
            }
            planePathStage2 = new PlanePathStage();
            num = list[list.Count - 1];
            num2 = path.Count;
            planePathStage2.path = path.GetRange(num, num2 - num);
            planePathStage2.arcanus = plane.arcanusType;
            planePathStage2.mpCost = requestDataV.GetCostTo(planePathStage2.path[planePathStage2.path.Count - 1]);
            list2.Add(planePathStage2);
            flag3 = plane.GetHexAtWrapped(planePathStage2.path[1]).IsLand();
            planePathStage2.transportStage = !flag3;
            return list2;
        }
        return null;
    }

    public PlanePathStage TrackAvoidanceStaging(global::MOM.Group g, FInt mpRange, Vector3i position, global::WorldCode.Plane plane, bool allowSwitch = true)
    {
        if (this.destination == null)
        {
            return null;
        }
        RequestDataV2 requestDataV;
        if (plane.arcanusType == this.destination.arcanus)
        {
            requestDataV = RequestDataV2.CreateRequest(plane, position, this.destination.position, g);
            requestDataV.MakeItAvoidanceSearch(g);
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path = requestDataV.GetPath();
            if (path != null && path.Count > 1)
            {
                PlanePathStage planePathStage = new PlanePathStage();
                planePathStage.path = path;
                planePathStage.arcanus = plane.arcanusType;
                planePathStage.mpCost = requestDataV.GetCostTo(planePathStage.path[planePathStage.path.Count - 1]);
                return planePathStage;
            }
        }
        if (!allowSwitch)
        {
            return null;
        }
        requestDataV = RequestDataV2.CreateRequest(plane, position, mpRange, g);
        requestDataV.MakeItAvoidanceSearch(g);
        PathfinderV2.FindArea(requestDataV);
        List<Vector3i> area = requestDataV.GetArea();
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (!(entity.Value is global::MOM.Location location) || location.locationType != ELocationType.PlaneTower || !area.Contains(location.GetPosition()) || location.GetPlane() != plane || (location.GetUnits().Count != 0 && location.GetOwnerID() != g.GetOwnerID()))
            {
                continue;
            }
            int distanceWrapping = plane.GetDistanceWrapping(location.GetPosition(), position);
            FInt fInt = mpRange - distanceWrapping;
            global::WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
            if (fInt > 1)
            {
                PlanePathStage planePathStage2 = this.TrackStaging(g, fInt, location.GetPosition(), otherPlane, allowSwitch: false);
                if (planePathStage2 != null)
                {
                    return planePathStage2;
                }
            }
        }
        return null;
    }

    public PlanePathStage TrackStaging(global::MOM.Group g, FInt mpRange, Vector3i position, global::WorldCode.Plane plane, bool allowSwitch = true)
    {
        if (this.destination == null)
        {
            return null;
        }
        RequestDataV2 requestDataV;
        if (plane.arcanusType == this.destination.arcanus)
        {
            requestDataV = RequestDataV2.CreateRequest(plane, position, this.destination.position, g);
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path = requestDataV.GetPath();
            if (path != null && path.Count > 1)
            {
                PlanePathStage planePathStage = new PlanePathStage();
                planePathStage.path = path;
                planePathStage.arcanus = plane.arcanusType;
                planePathStage.mpCost = requestDataV.GetCostTo(planePathStage.path[planePathStage.path.Count - 1]);
                return planePathStage;
            }
        }
        if (!allowSwitch || !GameManager.Get().allowPlaneSwitch)
        {
            return null;
        }
        requestDataV = RequestDataV2.CreateRequest(plane, position, mpRange, g);
        PathfinderV2.FindArea(requestDataV);
        List<Vector3i> area = requestDataV.GetArea();
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (!(entity.Value is global::MOM.Location location) || location.locationType != ELocationType.PlaneTower || !area.Contains(location.GetPosition()) || location.GetPlane() != plane || (location.GetUnits().Count != 0 && location.GetOwnerID() != g.GetOwnerID()))
            {
                continue;
            }
            int distanceWrapping = plane.GetDistanceWrapping(location.GetPosition(), position);
            FInt fInt = mpRange - distanceWrapping;
            global::WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
            if (fInt > 1)
            {
                PlanePathStage planePathStage2 = this.TrackStaging(g, fInt, location.GetPosition(), otherPlane, allowSwitch: false);
                if (planePathStage2 != null)
                {
                    return planePathStage2;
                }
            }
        }
        return null;
    }

    public IEnumerator MoveViaMPath(global::MOM.Group group, AIGroupDesignation a)
    {
        while (group != null && this.stages != null && this.stages.Count >= 1 && group.alive && group.GetUnits().Count >= 1 && !(group.CurentMP() == 0))
        {
            if (group.GetPlane().arcanusType != this.stages[0].arcanus)
            {
                group = group.PlaneSwitch();
                group.EnsureHasMP();
            }
            if (this.stages[0].path == null || this.stages[0].path.Count < 2 || this.stages[0].pathFinished)
            {
                this.stages.RemoveAt(0);
                continue;
            }
            List<Vector3i> path = this.stages[0].path;
            if (path[path.Count - 1] == group.GetPosition())
            {
                this.stages.RemoveAt(0);
                continue;
            }
            if (path[0] != group.GetPosition())
            {
                int num = path.IndexOf(group.GetPosition());
                if (num == -1 || num == path.Count - 1)
                {
                    break;
                }
                path.RemoveRange(0, num);
            }
            bool canEnterTown = a.designation == AIGroupDesignation.Designation.Transfer || a.designation == AIGroupDesignation.Designation.Defend || a.designation == AIGroupDesignation.Designation.Melder || a.designation == AIGroupDesignation.Designation.Retreat;
            bool cameraFollow = Settings.GetData().GetFollowAIMovement();
            if (group.IsModelVisible() && World.GetActivePlane() != group.GetPlane() && cameraFollow)
            {
                World.ActivatePlane(group.GetPlane());
                yield return null;
            }
            group = this.MoveViaPath(group, this.stages[0], canEnterTown, this.destination.aggressive);
            while (group != null)
            {
                bool flag = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement);
                bool flag2 = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle);
                if (!flag && !flag2)
                {
                    break;
                }
                if (flag && !flag2 && group != null && group.alive && !group.IsGroupInvisible() && group.GetOwnerID() != GameManager.GetHumanWizard().GetID() && path.Count > 1 && cameraFollow && FOW.Get().IsVisible(group.GetPosition(), group.GetPlane()))
                {
                    if (World.GetActivePlane() != group.GetPlane())
                    {
                        World.ActivatePlane(group.GetPlane());
                    }
                    CharacterActor characterActor = group.GetMapFormation().GetCharacterActors().Find((CharacterActor o) => o != null);
                    if (characterActor != null)
                    {
                        CameraController.CenterAt(characterActor.transform.localPosition);
                    }
                }
                yield return null;
            }
            while (!GameManager.Get().IsFocusFree())
            {
                yield return null;
            }
            if (group != null && group.alive && !FOW.Get().IsVisible(group.GetPosition(), group.GetPlane()))
            {
                group.DestroyMapFormation();
            }
        }
    }

    public global::MOM.Group MoveViaPath(global::MOM.Group group, PlanePathStage stage, bool canEnterTown, bool aggressive)
    {
        List<Vector3i> path = stage.path;
        if (path == null || path.Count < 2)
        {
            stage.pathFinished = true;
            return group;
        }
        int num = 0;
        for (int i = 1; i < path.Count; i++)
        {
            Hex hexAtWrapped = group.GetPlane().GetHexAtWrapped(path[i]);
            if (hexAtWrapped.IsLand() && (group.landMovement || group.nonCorporealMovement))
            {
                num++;
                continue;
            }
            if (hexAtWrapped.IsLand() || (!group.waterMovement && !group.nonCorporealMovement))
            {
                break;
            }
            num++;
        }
        if (num > 0)
        {
            if (num < path.Count - 1)
            {
                List<Vector3i> range = path.GetRange(0, num + 1);
                IGroup group2 = group.MoveViaPath(range, mergeCollidedAlliedGroups: true, canEnterTown, aggressive);
                stage.path = path.GetRange(num, path.Count - 1 - num);
                return group2 as global::MOM.Group;
            }
            return group.MoveViaPath(path, mergeCollidedAlliedGroups: true, canEnterTown, aggressive) as global::MOM.Group;
        }
        Vector3i vector3i = path[1];
        PlayerWizardAI playerWizardAI = GameManager.GetWizard(group.GetOwnerID()) as PlayerWizardAI;
        if (playerWizardAI.transportRequests != null)
        {
            TransportRequest transportRequest = playerWizardAI.transportRequests.Find((TransportRequest o) => o.cargo == group);
            if (transportRequest == null || !transportRequest.IsValid())
            {
                return null;
            }
            if (vector3i == transportRequest.transport.GetPosition() && this.MergeWithTransporter(transportRequest))
            {
                stage.path = path.GetRange(1, path.Count - 1);
                return group;
            }
        }
        if (group.Position != vector3i && group.GetPlane().GetHexAtWrapped(vector3i).IsLand() && this.DropOneUnit(group, vector3i, cannotWalk: true, cannotSwim: false))
        {
            return group;
        }
        return null;
    }

    private bool MergeWithTransporter(TransportRequest transportRequest)
    {
        global::MOM.Group cargo = transportRequest.cargo;
        global::MOM.Group transport = transportRequest.transport;
        bool flag = transport.GetPlane().GetHexAtWrapped(transport.GetPosition()).IsLand();
        while (cargo.GetUnits().Count + transport.GetUnits().Count > 9)
        {
            if (!this.DropOneUnit(cargo, transport.GetPosition(), flag, !flag) && !this.DropOneUnit(transport, cargo.GetPosition(), flag, !flag))
            {
                return false;
            }
        }
        Vector3i position = transport.GetPosition();
        cargo.AddUnitsIfPossible(transport.GetUnits());
        cargo.Position = position;
        (GameManager.GetWizard(cargo.GetOwnerID()) as PlayerWizardAI).transportRequests.Remove(transportRequest);
        return true;
    }

    private bool DropOneUnit(global::MOM.Group g, Vector3i omitThisPosition, bool cannotWalk, bool cannotSwim)
    {
        List<Reference<global::MOM.Unit>> units = g.GetUnits();
        if (units.Count < 2)
        {
            return false;
        }
        global::MOM.Unit unit = null;
        int num = int.MaxValue;
        foreach (Reference<global::MOM.Unit> item in units)
        {
            int num2 = item.Get().GetWorldUnitValue();
            if (item.Get().GetAttFinal(TAG.CAN_FLY) == 0 && item.Get().GetAttFinal(TAG.NON_CORPOREAL) == 0)
            {
                if (cannotSwim && item.Get().GetAttFinal(TAG.CAN_SWIM) == 0)
                {
                    num2 -= 10000;
                }
                if (cannotWalk && item.Get().GetAttFinal(TAG.CAN_WALK) == 0)
                {
                    num2 -= 10000;
                }
            }
            if (unit == null || num2 <= num)
            {
                unit = item;
                num = num2;
            }
        }
        bool flag = unit.GetAttFinal(TAG.CAN_FLY) > 0 || unit.GetAttFinal(TAG.NON_CORPOREAL) > 0 || unit.GetAttFinal(TAG.CAN_WALK) > 0;
        bool flag2 = unit.GetAttFinal(TAG.CAN_FLY) > 0 || unit.GetAttFinal(TAG.NON_CORPOREAL) > 0 || unit.GetAttFinal(TAG.CAN_SWIM) > 0;
        Vector3i[] neighbours = HexNeighbors.neighbours;
        foreach (Vector3i vector3i in neighbours)
        {
            Vector3i vector3i2 = g.GetPosition() + vector3i;
            if (vector3i2 == omitThisPosition)
            {
                continue;
            }
            Hex hexAtWrapped = g.GetPlane().GetHexAtWrapped(vector3i2);
            if (!(hexAtWrapped.IsLand() && flag) && !(!hexAtWrapped.IsLand() && flag2))
            {
                continue;
            }
            bool flag3 = false;
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                if (!(entity.Value is global::MOM.Group))
                {
                    continue;
                }
                global::MOM.Group group = entity.Value as global::MOM.Group;
                if (group.GetPlane() == g.GetPlane() && !(group.GetPosition() != hexAtWrapped.Position))
                {
                    if (group.GetOwnerID() != g.GetOwnerID())
                    {
                        flag3 = true;
                        break;
                    }
                    group.AddUnit(unit);
                    group.UpdateMapFormation(createIfMissing: false);
                    return true;
                }
            }
            if (!flag3)
            {
                global::MOM.Group group2 = new global::MOM.Group(World.GetActivePlane(), g.GetOwnerID());
                group2.Position = hexAtWrapped.Position;
                group2.AddUnit(unit);
                if (group2.IsGroupDiscoveredAndVisible())
                {
                    group2.GetMapFormation();
                }
                return true;
            }
        }
        return false;
    }
}
