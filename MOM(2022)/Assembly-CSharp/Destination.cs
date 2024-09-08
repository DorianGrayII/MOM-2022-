using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

    public Destination(IPlanePosition a, bool aggressive)
    {
        this.position = a.GetPosition();
        this.arcanus = a.GetPlane().arcanusType;
        this.aggressive = aggressive;
    }

    public Destination(Vector3i p, bool arcanus, bool aggressive)
    {
        this.position = p;
        this.arcanus = arcanus;
        this.aggressive = aggressive;
    }

    public static Destination FindClosestFriendlyLocation(AIGroupDesignation a)
    {
        return FindClosestFriendlyLocation(a, a.GetPlane(), a.GetPosition(), new FInt(0x19), true, true);
    }

    private static Destination FindClosestFriendlyLocation(AIGroupDesignation a, WorldCode.Plane plane, Vector3i pos, FInt distance, bool allowPlaneShift, bool includeTownsOnly)
    {
        MOM.Group group = a.owner.Get();
        List<Vector3i> area = null;
        if ((group.arcanusIsPlaneOfOrigin == plane.arcanusType) && (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f))
        {
            RequestDataV2 rd = RequestDataV2.CreateRequest(plane, group.positionOfOrigin, distance, a.owner.Get(), false);
            PathfinderV2.FindArea(rd, false);
            area = rd.GetArea();
        }
        else
        {
            RequestDataV2 rd = RequestDataV2.CreateRequest(plane, pos, distance, a.owner.Get(), false);
            PathfinderV2.FindArea(rd, false);
            area = rd.GetArea();
        }
        MOM.Location location = null;
        int num = 0x7fffffff;
        foreach (MOM.Location location2 in GameManager.GetLocationsOfThePlane(plane))
        {
            int distanceWrapping = plane.GetDistanceWrapping(location2.GetPosition(), pos);
            if ((location2.GetOwnerID() == a.GetWizardID()) && area.Contains(location2.GetPosition()))
            {
                if (distanceWrapping < distance)
                {
                    if (includeTownsOnly && !(location2 is TownLocation))
                    {
                        continue;
                    }
                    if ((location == null) || ((location2 is TownLocation) && !(location is TownLocation)))
                    {
                        location = location2;
                        num = distanceWrapping;
                    }
                    else if ((location2 is TownLocation) == (location is TownLocation))
                    {
                        if (num < distanceWrapping)
                        {
                            continue;
                        }
                        location = location2;
                        num = distanceWrapping;
                    }
                }
                if (allowPlaneShift && (!(location is TownLocation) && (location2.locationType == ELocationType.PlaneTower)))
                {
                    WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
                    Destination destination = FindClosestFriendlyLocation(a, otherPlane, location2.GetPosition(), distance - distanceWrapping, false, true);
                    if (destination != null)
                    {
                        MOM.Location asLocation = destination.GetAsLocation();
                        if (asLocation != null)
                        {
                            location = asLocation;
                            num = plane.GetDistanceWrapping(location2.GetPosition(), pos) + plane.GetDistanceWrapping(pos, asLocation.GetPosition());
                        }
                    }
                }
            }
        }
        return ((location != null) ? new Destination(location.GetPosition(), location.GetPlane().arcanusType, false) : null);
    }

    public static MOM.Group FindClosestPrey(AIGroupDesignation a, int range, bool onlyExpeditions)
    {
        return FindClosestPrey(a, a.GetPlane(), a.GetPosition(), new FInt(range), true, onlyExpeditions);
    }

    private static MOM.Group FindClosestPrey(AIGroupDesignation a, WorldCode.Plane plane, Vector3i pos, FInt distance, bool allowPlaneShift, bool onlyExpeditions)
    {
        object assignedTarget;
        MOM.Group g = a.owner.Get();
        PlayerWizardAI wizard = GameManager.GetWizard(g.GetOwnerID()) as PlayerWizardAI;
        if (wizard == null)
        {
            assignedTarget = null;
        }
        else if (wizard.priorityTargets != null)
        {
            assignedTarget = wizard.priorityTargets.GetAssignedTarget(g, false);
        }
        else
        {
            AIPriorityTargets priorityTargets = wizard.priorityTargets;
            assignedTarget = null;
        }
        AITarget target = (AITarget) assignedTarget;
        List<Vector3i> second = null;
        if ((target != null) || ((a.designation == AIGroupDesignation.Designation.AggressionLong) || (!g.doomStack || (g.arcanusIsPlaneOfOrigin != plane.arcanusType))))
        {
            RequestDataV2 rd = RequestDataV2.CreateRequest(plane, pos, distance, a.owner.Get(), false);
            PathfinderV2.FindArea(rd, false);
            second = rd.GetArea();
        }
        else
        {
            RequestDataV2 rd = RequestDataV2.CreateRequest(plane, pos, distance / 2, a.owner.Get(), false);
            PathfinderV2.FindArea(rd, false);
            second = rd.GetArea();
            rd = RequestDataV2.CreateRequest(plane, g.positionOfOrigin, distance, a.owner.Get(), false);
            PathfinderV2.FindArea(rd, false);
            second.AddRange(Enumerable.Except<Vector3i>(rd.GetArea(), second));
        }
        if (second == null)
        {
            return null;
        }
        int num = g.GetValue();
        if (wizard != null)
        {
            int num5;
            int mana = wizard.mana;
            if ((num5 = wizard.GetTotalCastingSkill()) > (mana * 3))
            {
                num5 = mana / 3;
            }
            num += num5 * (1 + Mathf.Min(3, g.GetUnits().Count));
        }
        MOM.Group group2 = null;
        int num2 = 0x7fffffff;
        MOM.Group group3 = null;
        int num3 = 0x7fffffff;
        foreach (MOM.Group group4 in GameManager.GetGroupsOfPlane(plane))
        {
            if (!ReferenceEquals(group4, g))
            {
                if (group4.GetOwnerID() != a.GetWizardID())
                {
                    if ((group4.GetLocationHostSmart() != null) & onlyExpeditions)
                    {
                        continue;
                    }
                    if (group4.GetOwnerID() > 0)
                    {
                        DiplomaticStatus statusToward = a.GetWizard().GetDiplomacy().GetStatusToward(group4.GetOwnerID());
                        if ((statusToward == null) || !statusToward.openWar)
                        {
                            continue;
                        }
                    }
                    if (second.Contains(group4.GetPosition()))
                    {
                        int distanceWrapping = plane.GetDistanceWrapping(pos, group4.GetPosition());
                        if (num2 > distanceWrapping)
                        {
                            int num10 = group4.GetValue();
                            if ((group4.GetLocationHostSmart() != null) && ((GameManager.GetWizard(group4.GetOwnerID()) != null) && (wizard.wizardTower == group4.GetLocationHostSmart())))
                            {
                                num10 += 0x3e8;
                            }
                            if ((group4 != null) && (num10 < (num * 0.8f)))
                            {
                                group2 = group4;
                                num2 = distanceWrapping;
                            }
                        }
                    }
                    continue;
                }
                if (!group4.IsHosted() && second.Contains(group4.GetPosition()))
                {
                    AITarget assignedTarget;
                    if (wizard.priorityTargets != null)
                    {
                        assignedTarget = wizard.priorityTargets.GetAssignedTarget(group4, false);
                    }
                    else
                    {
                        AIPriorityTargets priorityTargets = wizard.priorityTargets;
                        assignedTarget = null;
                    }
                    if (assignedTarget != null)
                    {
                        if (group4.GetUnits().Count < 9)
                        {
                            int distanceTo = PlanePositionExtension.GetDistanceTo(group4, g);
                            if ((group3 == null) || (distanceTo < num3))
                            {
                                num3 = distanceTo;
                                group3 = group4;
                            }
                        }
                    }
                    else if ((target == null) && (group4.doomStack && ((group4.GetUnits().Count < 9) && (g.GetUnits().Count <= group4.GetUnits().Count))))
                    {
                        int distanceTo = PlanePositionExtension.GetDistanceTo(group4, g);
                        if (distanceTo < num2)
                        {
                            num2 = distanceTo;
                            group2 = group4;
                        }
                    }
                }
            }
        }
        if ((num3 < num2) && (group3 != null))
        {
            return group3;
        }
        if (allowPlaneShift)
        {
            foreach (MOM.Location location in GameManager.GetLocationsOfThePlane(plane))
            {
                int distanceWrapping = plane.GetDistanceWrapping(location.GetPosition(), pos);
                if ((location.GetOwnerID() == a.GetWizardID()) && (second.Contains(location.GetPosition()) && (location.locationType == ELocationType.PlaneTower)))
                {
                    WorldCode.Plane otherPlane = World.GetOtherPlane(plane);
                    MOM.Group group5 = FindClosestPrey(a, otherPlane, location.GetPosition(), distance - distanceWrapping, false, onlyExpeditions);
                    if (group5 != null)
                    {
                        int num12 = otherPlane.GetDistanceWrapping(group5.GetPosition(), location.GetPosition());
                        if ((distanceWrapping + num12) < num2)
                        {
                            group2 = group5;
                            num2 = distanceWrapping + num12;
                        }
                    }
                }
            }
        }
        return group2;
    }

    public MOM.Location GetAsLocation()
    {
        WorldCode.Plane p = null;
        p = !this.arcanus ? World.GetMyrror() : World.GetArcanus();
        MOM.Location location = GameManager.GetLocationsOfThePlane(p).Find(o => o.GetPosition() == this.position);
        return ((location == null) ? null : location);
    }

    public bool Reached(AIGroupDesignation a)
    {
        return ((a != null) ? ((a.GetPlane().arcanusType == this.arcanus) && (a.GetPosition() == this.position)) : false);
    }

    public bool Reached(IPlanePosition p)
    {
        if (p is MOM.Location)
        {
            MOM.Location location = p as MOM.Location;
            if (location.locationType == ELocationType.PlaneTower)
            {
                return (location.GetPosition() == this.position);
            }
        }
        return ((p.GetPlane().arcanusType == this.arcanus) && (p.GetPosition() == this.position));
    }

    public override string ToString()
    {
        return string.Format("Destination Plane:{0}, Position {1}, Data: {2}", this.arcanus, this.position, this.data1);
    }
}

