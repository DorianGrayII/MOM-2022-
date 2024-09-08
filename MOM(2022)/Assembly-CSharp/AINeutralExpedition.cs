// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AINeutralExpedition
using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MOM;
using ProtoBuf;
using WorldCode;

[ProtoContract]
public class AINeutralExpedition
{
    [ProtoMember(3)]
    public int cannotMove;

    [ProtoMember(4)]
    public Reference<Group> owner;

    [ProtoMember(5)]
    public Reference<Group> target;

    [ProtoMember(6)]
    public Reference<adfTownLocation> spawnLocation;

    [ProtoMember(7)]
    public bool rampage;

    public AINeutralExpedition()
    {
    }

    public AINeutralExpedition(Group group, TownLocation tl, IPlanePosition target)
    {
        this.owner = group;
        this.spawnLocation = tl;
        if (target is Location)
        {
            this.target = (target as Location).GetLocalGroup();
        }
        else
        {
            this.target = target as Group;
        }
    }

    public AINeutralExpedition(Group group)
    {
        this.owner = group;
        this.rampage = true;
    }

    public IEnumerator ResolveTurn(int aiDiff)
    {
        if (!this.owner.Get().alive)
        {
            yield break;
        }
        Vector3i position = this.owner.Get().GetPosition();
        if (this.rampage || this.target == null)
        {
            if (this.target == null || !this.target.Get().alive || this.owner.Get().GetPlane().GetDistanceWrapping(position, this.target.Get().Position) > 15)
            {
                this.FindTargetInRange(20, onlyEasy: false);
            }
        }
        else if (this.target == null || !this.target.Get().alive)
        {
            this.target = null;
            if (this.spawnLocation != null && this.spawnLocation.Get().aiForNeutralTown != null)
            {
                this.target = this.spawnLocation.Get().aiForNeutralTown.GetAttackTarget();
            }
        }
        if (this.target != null)
        {
            if (aiDiff > 1)
            {
                this.FindTargetInRange(5, onlyEasy: true);
            }
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.owner.Get().GetPlane(), position, this.target.Get().GetPosition(), this.owner.Get());
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path = requestDataV.GetPath();
            if (path != null && path.Count > 1)
            {
                Group group = this.owner.Get();
                bool followAIMovement = Settings.GetData().GetFollowAIMovement();
                if (group.IsModelVisible() && World.GetActivePlane() != group.GetPlane() && followAIMovement)
                {
                    World.ActivatePlane(group.GetPlane());
                    yield return null;
                }
                this.cannotMove = 0;
                this.owner.Get().MoveViaPath(path, mergeCollidedAlliedGroups: false);
            }
            else
            {
                this.cannotMove++;
                if (this.cannotMove > 2)
                {
                    this.target = null;
                }
                if (this.cannotMove > 4)
                {
                    this.owner.Get().Destroy();
                }
            }
        }
        else
        {
            this.owner.Get().Destroy();
        }
    }

    private void FindTargetInRange(int range, bool onlyEasy)
    {
        Vector3i position = this.owner.Get().GetPosition();
        List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.owner.Get().GetPlane());
        int num = int.MaxValue;
        int num2 = 0;
        int value = this.owner.Get().GetValue();
        if (this.target != null)
        {
            num = this.owner.Get().GetPlane().GetDistanceWrapping(position, this.target.Get().GetPosition());
            num2 = this.target.Get().GetValue();
        }
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.owner.Get().GetPlane(), position, new FInt(range), this.owner.Get());
        PathfinderV2.FindArea(requestDataV);
        List<Vector3i> area = requestDataV.GetArea();
        if (area == null)
        {
            return;
        }
        foreach (Group item in groupsOfPlane)
        {
            if (item.GetOwnerID() <= 0 || (item.IsHosted() && !(item.GetLocationHost() is TownLocation)) || area.IndexOf(item.GetPosition()) == -1)
            {
                continue;
            }
            int distanceWrapping = this.owner.Get().GetPlane().GetDistanceWrapping(position, item.GetPosition());
            int value2 = item.GetValue();
            if (onlyEasy)
            {
                if (this.target == null || value > value2 * 2 || value2 < num2)
                {
                    this.target = item;
                    num2 = value2;
                    num = distanceWrapping;
                }
            }
            else if (this.target == null || (2 * num > distanceWrapping && num2 > value2) || (num > distanceWrapping && (float)num2 < (float)value2 * 1.5f))
            {
                this.target = item;
                num2 = value2;
                num = distanceWrapping;
            }
        }
    }
}
