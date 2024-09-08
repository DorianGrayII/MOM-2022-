using System.Collections.Generic;
using DBEnum;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class AIGroupTransition
{
    [ProtoMember(1)]
    public Reference<Group> cargo;

    [ProtoMember(2)]
    public Reference<Group> transport;

    [ProtoMember(3)]
    public Vector3i landPathTo;

    [ProtoMember(4)]
    public Vector3i landPathFrom;

    [ProtoMember(5)]
    public Vector3i waterPickup;

    [ProtoMember(6)]
    public Vector3i waterDropoff;

    [ProtoMember(7)]
    public int expectedContactTurn;

    public bool Validate()
    {
        if (this.cargo == null || this.cargo.Get().alive || this.transport == null || this.transport.Get().alive)
        {
            return true;
        }
        return false;
    }

    public void ClearTransition()
    {
        if (this.cargo != null && this.cargo.Get().alive)
        {
            this.cargo.Get().GetDesignation().groupTransition = null;
        }
        if (this.transport != null && this.transport.Get().alive)
        {
            this.transport.Get().GetDesignation().groupTransition = null;
        }
    }

    public static AIGroupTransition AttemptToformTransition(Group g, Group transport, Vector3i destination)
    {
        if (g == null || transport == null || g.GetPlane() != transport.GetPlane())
        {
            return null;
        }
        AIGroupTransition aIGroupTransition = new AIGroupTransition();
        aIGroupTransition.cargo = g;
        aIGroupTransition.transport = transport;
        global::WorldCode.Plane plane = g.GetPlane();
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, g.GetPosition(), destination, g, allTerrain: true);
        PathfinderV2.FindPath(requestDataV);
        List<Vector3i> path = requestDataV.GetPath();
        if (path == null || path.Count < 2)
        {
            return null;
        }
        int num = -1;
        int num2 = -1;
        int num3 = -1;
        int num4 = int.MaxValue;
        for (int i = 0; i < path.Count; i++)
        {
            Hex hexAt = plane.GetHexAt(path[i]);
            if (hexAt == null)
            {
                Debug.LogError(path[i]);
                return null;
            }
            if (hexAt.IsLand())
            {
                if (i == num + 1)
                {
                    num = i;
                }
                continue;
            }
            num3 = i;
            break;
        }
        if (num3 == -1)
        {
            return null;
        }
        int num5 = path.Count - 1;
        while (num5 > num3)
        {
            Hex hexAt2 = plane.GetHexAt(path[num5]);
            if (hexAt2 == null)
            {
                Debug.LogError(path[num5]);
                return null;
            }
            if (hexAt2.IsLand())
            {
                if (num5 == num2 - 1)
                {
                    num2 = num5;
                }
                num5--;
                continue;
            }
            num4 = num5;
            break;
        }
        if (num4 > path.Count)
        {
            return null;
        }
        RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(plane, transport.GetPosition(), path[num3], transport);
        PathfinderV2.FindPath(requestDataV2);
        List<Vector3i> path2 = requestDataV2.GetPath();
        if (path2 == null || path2.Count < 2)
        {
            return null;
        }
        RequestDataV2 requestDataV3 = RequestDataV2.CreateRequest(plane, path[num3], path[num4], transport);
        PathfinderV2.FindPath(requestDataV3);
        path2 = requestDataV3.GetPath();
        if (path2 == null || path2.Count < 2)
        {
            return null;
        }
        aIGroupTransition.landPathTo = path[num];
        aIGroupTransition.landPathFrom = path[num2];
        aIGroupTransition.waterPickup = path[num3];
        aIGroupTransition.waterDropoff = path[num4];
        return aIGroupTransition;
    }

    public Vector3i GetDestination(Group group)
    {
        if (this.cargo != this.transport)
        {
            if (group == this.cargo)
            {
                if (group.GetPosition() == this.landPathTo && this.transport.Get().GetPosition() == this.waterPickup && group.CurentMP() > 0)
                {
                    group.aiDesignation.NewOwner(this.transport);
                    group.aiDesignation = null;
                    this.cargo = this.transport;
                    return this.waterPickup;
                }
                return this.landPathTo;
            }
            return this.waterPickup;
        }
        if (group.GetPosition() == this.waterDropoff && GameManager.GetGroupsOfPlane(group.GetPlane()).Find((Group o) => o.GetPosition() == this.landPathFrom) == null)
        {
            if (group.landMovement)
            {
                group.aiDesignation.groupTransition = null;
                return this.waterDropoff;
            }
            Group group2 = new Group(group.GetPlane(), group.GetOwnerID());
            group2.Position = group.GetPosition();
            for (int num = group.GetUnits().Count - 1; num >= 0; num--)
            {
                Reference<Unit> reference = group.GetUnits()[num];
                if (reference.Get().GetAttFinal(TAG.CAN_WALK) > 0 || reference.Get().GetAttFinal(TAG.CAN_FLY) > 0)
                {
                    group.TransferUnit(group2, reference);
                }
            }
            group.aiDesignation.NewOwner(group2);
            group.aiDesignation = null;
            group2.aiDesignation.groupTransition = null;
            group2.MoveViaPath(new List<Vector3i> { this.waterDropoff, this.landPathFrom }, mergeCollidedAlliedGroups: false);
        }
        return this.waterDropoff;
    }
}
