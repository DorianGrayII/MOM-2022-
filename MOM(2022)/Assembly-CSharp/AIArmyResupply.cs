using MHUtils;
using MOM;
using ProtoBuf;
using System;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class AIArmyResupply : IPlanePosition
{
    [ProtoMember(1)]
    public Reference<PlayerWizard> owner;
    [ProtoMember(2)]
    public AIWarArmy target;
    [ProtoMember(2)]
    public Reference<Group> group;
    [ProtoMember(3)]
    public AIWarArmy.Mobility armyMobilityDesignation;

    public void ActAfterTargetIsObsolete()
    {
    }

    public Vector3 GetPhysicalPosition()
    {
        if (this.GetPlane() == null)
        {
            return Vector3.zero;
        }
        Vector3 vector = HexCoordinates.HexToWorld3D(this.GetPosition());
        return (this.GetPlane().GetChunkFor(this.GetPosition()).go.transform.position + vector);
    }

    public WorldCode.Plane GetPlane()
    {
        Group local2;
        if (this.group != null)
        {
            local2 = this.group.Get();
        }
        else
        {
            Reference<Group> group = this.group;
            local2 = null;
        }
        return ((local2 == null) ? null : this.group.Get().GetPlane());
    }

    public Vector3i GetPosition()
    {
        Group local2;
        if (this.group != null)
        {
            local2 = this.group.Get();
        }
        else
        {
            Reference<Group> group = this.group;
            local2 = null;
        }
        return ((local2 == null) ? Vector3i.invalid : this.group.Get().GetPosition());
    }

    public bool Valid()
    {
        return ((this.group != null) && ((this.group.Get() != null) && this.group.Get().alive));
    }
}

