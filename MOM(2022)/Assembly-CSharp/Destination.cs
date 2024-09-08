using System.Collections.Generic;
using System.Linq;
using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class Destination
{
    [ProtoMember(1)]
    public Vector3i position;

    [ProtoMember(2)]
    public bool arcanus;

    [ProtoMember(3)]
    public int data1;

    [ProtoMember(4)]
    public bool aggressive;

    [ProtoIgnore]
    private MultiPlanePath pathManager;

    public Destination()
    {
    }

    public Destination(Vector3i p, bool arcanus, bool aggressive)
    {
        this.position = p;
        this.arcanus = arcanus;
        this.aggressive = aggressive;
    }

    public Destination(IPlanePosition a, bool aggressive)
    {
        this.position = a.GetPosition();
        this.arcanus = a.GetPlane().arcanusType;
        this.aggressive = aggressive;
    }

    public bool Reached(AIGroupDesignation a)
    {
        if (a == null)
        {
            return false;
        }
        if (a.GetPlane().arcanusType == this.arcanus)
        {
            return a.GetPosition() == this.position;
        }
        return false;
    }

    public bool Reached(IPlanePosition p)
    {
        if (p is global::MOM.Location)
        {
            global::MOM.Location location = p as global::MOM.Location;
            if (location.locationType == ELocationType.PlaneTower)
            {
                return location.GetPosition() == this.position;
            }
        }
        if (p.GetPlane().arcanusType == this.arcanus)
        {
            return p.GetPosition() == this.position;
        }
        return false;
    }

    public override string ToString()
    {
        return $"Destination Plane:{this.arcanus}, Position {this.position}, Data: {this.data1}";
    }

    public static Destination FindClosestFriendlyLocation(AIGroupDesignation a)
    {
        return Destination.FindClosestFriendlyLocation(a, a.GetPlane(), a.GetPosition(), new FInt(25), allowPlaneShift: true);
    }

    private static Destination FindClosestFriendlyLocation(AIGroupDesignation a, global::WorldCode.Plane plane, Vector3i pos, FInt distance, bool allowPlaneShift, bool includeTownsOnly = true)
    {
        global::MOM.Group group = a.owner.Get();
        List<Vector3i> list = null;
        if (group.arcanusIsPlaneOfOrigin == plane.arcanusType && Random.Range(0f, 1f) < 0.7f)
        {
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, group.positionOfOrigin, distance, a.owner.Get());
            PathfinderV2.FindArea(requestDataV);
            list = requestDataV.GetArea();
        }
        else
        {
            RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(plane, pos, distance, a.owner.Get());
            PathfinderV2.FindArea(requestDataV2);
            list = requestDataV2.GetArea();
        }
        List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(plane);
        global::MOM.Location location = null;
        int num = int.MaxValue;
        foreach (global::MOM.Location item in locationsOfThePlane)
        {
            int distanceWrapping = plane.GetDistanceWrapping(item.GetPosition(), pos);
            if (item.GetOwnerID() != a.GetWizardID() || !list.Contains(item.GetPosition()))
            {
                continue;
            }
            if (distanceWrapping < distance)
            {
                if (includeTownsOnly && !(item is TownLocation))
                {
                    continue;
                }
                if (location == null || (item is TownLocation && !(location is TownLocation)))
                {
                    location = item;
                    num = distanceWrapping;
                }
                else if (item is TownLocation == location is TownLocation)
                {
                    if (num < distanceWrapping)
                    {
                        continue;
                    }
                    location = item;
                    num = distanceWrapping;
                }
            }
            if (!allowPlaneShift || location is TownLocation || item.locationType != ELocationType.PlaneTower)
            {
                continue;
            }
            global::WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
            Destination destination = Destination.FindClosestFriendlyLocation(a, otherPlane, item.GetPosition(), distance - distanceWrapping, allowPlaneShift: false);
            if (destination != null)
            {
                global::MOM.Location asLocation = destination.GetAsLocation();
                if (asLocation != null)
                {
                    location = asLocation;
                    num = plane.GetDistanceWrapping(item.GetPosition(), pos) + plane.GetDistanceWrapping(pos, asLocation.GetPosition());
                }
            }
        }
        if (location == null)
        {
            return null;
        }
        return new Destination(location.GetPosition(), location.GetPlane().arcanusType, aggressive: false);
    }

    public static global::MOM.Group FindClosestPrey(AIGroupDesignation a, int range, bool onlyExpeditions)
    {
        return Destination.FindClosestPrey(a, a.GetPlane(), a.GetPosition(), new FInt(range), allowPlaneShift: true, onlyExpeditions);
    }

    private static global::MOM.Group FindClosestPrey(AIGroupDesignation a, global::WorldCode.Plane plane, Vector3i pos, FInt distance, bool allowPlaneShift, bool onlyExpeditions)
    {
        global::MOM.Group group = a.owner.Get();
        PlayerWizardAI playerWizardAI = GameManager.GetWizard(group.GetOwnerID()) as PlayerWizardAI;
        AITarget aITarget = playerWizardAI?.priorityTargets?.GetAssignedTarget(group);
        List<Vector3i> list = null;
        if (aITarget == null && a.designation != AIGroupDesignation.Designation.AggressionLong && group.doomStack && group.arcanusIsPlaneOfOrigin == plane.arcanusType)
        {
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, pos, distance / 2, a.owner.Get());
            PathfinderV2.FindArea(requestDataV);
            list = requestDataV.GetArea();
            requestDataV = RequestDataV2.CreateRequest(plane, group.positionOfOrigin, distance, a.owner.Get());
            PathfinderV2.FindArea(requestDataV);
            list.AddRange(requestDataV.GetArea().Except(list));
        }
        else
        {
            RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(plane, pos, distance, a.owner.Get());
            PathfinderV2.FindArea(requestDataV2);
            list = requestDataV2.GetArea();
        }
        if (list == null)
        {
            return null;
        }
        int num = group.GetValue();
        if (playerWizardAI != null)
        {
            int mana = playerWizardAI.mana;
            int num2;
            if ((num2 = playerWizardAI.GetTotalCastingSkill()) > mana * 3)
            {
                num2 = mana / 3;
            }
            int count = group.GetUnits().Count;
            num += num2 * (1 + Mathf.Min(3, count));
        }
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(plane);
        global::MOM.Group result = null;
        int num3 = int.MaxValue;
        global::MOM.Group group2 = null;
        int num4 = int.MaxValue;
        foreach (global::MOM.Group item in groupsOfPlane)
        {
            if (item == group)
            {
                continue;
            }
            if (item.GetOwnerID() == a.GetWizardID())
            {
                if (item.IsHosted() || !list.Contains(item.GetPosition()))
                {
                    continue;
                }
                if (playerWizardAI.priorityTargets?.GetAssignedTarget(item) != null)
                {
                    if (item.GetUnits().Count < 9)
                    {
                        int distanceTo = item.GetDistanceTo(group);
                        if (group2 == null || distanceTo < num4)
                        {
                            num4 = distanceTo;
                            group2 = item;
                        }
                    }
                }
                else if (aITarget == null && item.doomStack && item.GetUnits().Count < 9 && group.GetUnits().Count <= item.GetUnits().Count)
                {
                    int distanceTo2 = item.GetDistanceTo(group);
                    if (distanceTo2 < num3)
                    {
                        num3 = distanceTo2;
                        result = item;
                    }
                }
            }
            else
            {
                if (item.GetLocationHostSmart() != null && onlyExpeditions)
                {
                    continue;
                }
                if (item.GetOwnerID() > 0)
                {
                    DiplomaticStatus statusToward = a.GetWizard().GetDiplomacy().GetStatusToward(item.GetOwnerID());
                    if (statusToward == null || !statusToward.openWar)
                    {
                        continue;
                    }
                }
                if (!list.Contains(item.GetPosition()))
                {
                    continue;
                }
                int distanceWrapping = plane.GetDistanceWrapping(pos, item.GetPosition());
                if (num3 > distanceWrapping)
                {
                    int num5 = item.GetValue();
                    if (item.GetLocationHostSmart() != null && GameManager.GetWizard(item.GetOwnerID()) != null && playerWizardAI.wizardTower == item.GetLocationHostSmart())
                    {
                        num5 += 1000;
                    }
                    if (item != null && (float)num5 < (float)num * 0.8f)
                    {
                        result = item;
                        num3 = distanceWrapping;
                    }
                }
            }
        }
        if (num4 < num3 && group2 != null)
        {
            return group2;
        }
        if (allowPlaneShift)
        {
            foreach (global::MOM.Location item2 in GameManager.GetLocationsOfThePlane(plane))
            {
                int distanceWrapping2 = plane.GetDistanceWrapping(item2.GetPosition(), pos);
                if (item2.GetOwnerID() != a.GetWizardID() || !list.Contains(item2.GetPosition()) || item2.locationType != ELocationType.PlaneTower)
                {
                    continue;
                }
                global::WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
                global::MOM.Group group3 = Destination.FindClosestPrey(a, otherPlane, item2.GetPosition(), distance - distanceWrapping2, allowPlaneShift: false, onlyExpeditions);
                if (group3 != null)
                {
                    int distanceWrapping3 = otherPlane.GetDistanceWrapping(group3.GetPosition(), item2.GetPosition());
                    if (distanceWrapping2 + distanceWrapping3 < num3)
                    {
                        result = group3;
                        num3 = distanceWrapping2 + distanceWrapping3;
                    }
                }
            }
        }
        return result;
    }

    public global::MOM.Location GetAsLocation()
    {
        global::WorldCode.Plane plane = null;
        plane = ((!this.arcanus) ? World.GetMyrror() : World.GetArcanus());
        global::MOM.Location location = GameManager.GetLocationsOfThePlane(plane).Find((global::MOM.Location o) => o.GetPosition() == this.position);
        if (location != null)
        {
            return location;
        }
        return null;
    }
}
