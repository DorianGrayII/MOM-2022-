// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AIWarTarget
using MOM;
using ProtoBuf;

[ProtoContract]
public class AIWarTarget
{
    [ProtoMember(1)]
    public Reference<PlayerWizard> owner;

    [ProtoMember(2)]
    public Reference<Group> group;

    [ProtoMember(3)]
    public bool consideredAnInvader;

    [ProtoMember(4)]
    public bool isGate;

    [ProtoMember(5)]
    public bool haveAssignee;

    public bool Valid(int ownerCheck = -1)
    {
        if (this.owner?.Get() == null || this.GetGroup() == null || (ownerCheck > -1 && (this.owner.Get().GetID() != ownerCheck || this.GetGroup().GetOwnerID() != ownerCheck)))
        {
            return false;
        }
        if (this.GetGroup() != null && this.GetGroup().GetOwnerID() == this.owner.Get().GetID() && (this.GetGroup().alive || this.GetTargetLocation() != null))
        {
            return true;
        }
        return false;
    }

    public Group GetGroup()
    {
        return this.group?.Get();
    }

    public Location GetTargetLocation()
    {
        if (this.group != null && this.group.Get().GetLocationHostSmart() != null)
        {
            return this.group.Get().GetLocationHostSmart();
        }
        return null;
    }
}
