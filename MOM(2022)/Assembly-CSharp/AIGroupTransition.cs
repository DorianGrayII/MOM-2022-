using DBEnum;
using MHUtils;
using MOM;
using ProtoBuf;
using System;
using System.Collections.Generic;
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

    public static AIGroupTransition AttemptToformTransition(Group g, Group transport, Vector3i destination)
    {
        if ((g == null) || ((transport == null) || !ReferenceEquals(g.GetPlane(), transport.GetPlane())))
        {
            return null;
        }
        AIGroupTransition transition = new AIGroupTransition {
            cargo = g,
            transport = transport
        };
        WorldCode.Plane p = g.GetPlane();
        RequestDataV2 rd = RequestDataV2.CreateRequest(p, g.GetPosition(), destination, g, true, false, false);
        PathfinderV2.FindPath(rd);
        List<Vector3i> path = rd.GetPath();
        if ((path == null) || (path.Count < 2))
        {
            return null;
        }
        int num = -1;
        int num2 = -1;
        int num3 = -1;
        int num4 = 0x7fffffff;
        int num5 = 0;
        while (true)
        {
            if (num5 < path.Count)
            {
                Hex hexAt = p.GetHexAt(path[num5]);
                if (hexAt == null)
                {
                    Debug.LogError(path[num5]);
                    return null;
                }
                if (hexAt.IsLand())
                {
                    if (num5 == (num + 1))
                    {
                        num = num5;
                    }
                    num5++;
                    continue;
                }
                num3 = num5;
            }
            if (num3 == -1)
            {
                return null;
            }
            int num6 = path.Count - 1;
            while (true)
            {
                if (num6 > num3)
                {
                    Hex hexAt = p.GetHexAt(path[num6]);
                    if (hexAt == null)
                    {
                        Debug.LogError(path[num6]);
                        return null;
                    }
                    if (hexAt.IsLand())
                    {
                        if (num6 == (num2 - 1))
                        {
                            num2 = num6;
                        }
                        num6--;
                        continue;
                    }
                    num4 = num6;
                }
                if (num4 > path.Count)
                {
                    return null;
                }
                RequestDataV2 av2 = RequestDataV2.CreateRequest(p, transport.GetPosition(), path[num3], transport, false, false, false);
                PathfinderV2.FindPath(av2);
                List<Vector3i> list2 = av2.GetPath();
                if ((list2 == null) || (list2.Count < 2))
                {
                    return null;
                }
                RequestDataV2 av3 = RequestDataV2.CreateRequest(p, path[num3], path[num4], transport, false, false, false);
                PathfinderV2.FindPath(av3);
                list2 = av3.GetPath();
                if ((list2 == null) || (list2.Count < 2))
                {
                    return null;
                }
                transition.landPathTo = path[num];
                transition.landPathFrom = path[num2];
                transition.waterPickup = path[num3];
                transition.waterDropoff = path[num4];
                return transition;
            }
        }
    }

    public void ClearTransition()
    {
        if ((this.cargo != null) && this.cargo.Get().alive)
        {
            this.cargo.Get().GetDesignation(true).groupTransition = null;
        }
        if ((this.transport != null) && this.transport.Get().alive)
        {
            this.transport.Get().GetDesignation(true).groupTransition = null;
        }
    }

    public Vector3i GetDestination(Group group)
    {
        if (this.cargo != this.transport)
        {
            if (group == this.cargo)
            {
                if (!(group.GetPosition() == this.landPathTo) || (!(this.transport.Get().GetPosition() == this.waterPickup) || (group.CurentMP() <= 0)))
                {
                    return this.landPathTo;
                }
                group.aiDesignation.NewOwner((Group) this.transport);
                group.aiDesignation = null;
                this.cargo = this.transport;
            }
            return this.waterPickup;
        }
        if ((group.GetPosition() == this.waterDropoff) && (GameManager.GetGroupsOfPlane(group.GetPlane()).Find(o => o.GetPosition() == this.landPathFrom) == null))
        {
            if (group.landMovement)
            {
                group.aiDesignation.groupTransition = null;
                return this.waterDropoff;
            }
            Group g = new Group(group.GetPlane(), group.GetOwnerID(), false) {
                Position = group.GetPosition()
            };
            int num = group.GetUnits().Count - 1;
            while (true)
            {
                if (num < 0)
                {
                    group.aiDesignation.NewOwner(g);
                    group.aiDesignation = null;
                    g.aiDesignation.groupTransition = null;
                    List<Vector3i> path = new List<Vector3i>();
                    path.Add(this.waterDropoff);
                    path.Add(this.landPathFrom);
                    g.MoveViaPath(path, false, true, true, true);
                    break;
                }
                Reference<Unit> reference = group.GetUnits()[num];
                if ((IAttributeableExtension.GetAttFinal(reference.Get(), TAG.CAN_WALK) > 0) || (IAttributeableExtension.GetAttFinal(reference.Get(), TAG.CAN_FLY) > 0))
                {
                    group.TransferUnit(g, (Unit) reference);
                }
                num--;
            }
        }
        return this.waterDropoff;
    }

    public bool Validate()
    {
        return ((this.cargo == null) || (this.cargo.Get().alive || ((this.transport == null) || this.transport.Get().alive)));
    }
}

