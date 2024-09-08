using MHUtils;
using MOM;
using ProtoBuf;
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

    public bool Valid()
    {
        if (this.group == null || this.group.Get() == null || !this.group.Get().alive)
        {
            return false;
        }
        return true;
    }

    public Vector3i GetPosition()
    {
        if (this.group?.Get() != null)
        {
            return this.group.Get().GetPosition();
        }
        return Vector3i.invalid;
    }

    public Vector3 GetPhysicalPosition()
    {
        if (this.GetPlane() != null)
        {
            Chunk chunkFor = this.GetPlane().GetChunkFor(this.GetPosition());
            Vector3 vector = HexCoordinates.HexToWorld3D(this.GetPosition());
            return chunkFor.go.transform.position + vector;
        }
        return Vector3.zero;
    }

    public global::WorldCode.Plane GetPlane()
    {
        if (this.group?.Get() != null)
        {
            return this.group.Get().GetPlane();
        }
        return null;
    }

    public void ActAfterTargetIsObsolete()
    {
    }
}
