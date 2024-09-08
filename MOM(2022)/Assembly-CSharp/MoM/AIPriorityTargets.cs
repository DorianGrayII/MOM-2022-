// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.AIPriorityTargets
using System.Collections;
using System.Collections.Generic;
using MOM;
using ProtoBuf;
using WorldCode;

[ProtoContract]
public class AIPriorityTargets
{
    [ProtoMember(1)]
    public List<AITarget> aiTargets;

    [ProtoMember(2)]
    public int owner;

    public AIPriorityTargets()
    {
    }

    public AIPriorityTargets(int owner)
    {
        this.owner = owner;
    }

    public void ValidatePriorityTargets()
    {
        for (int num = this.aiTargets.Count - 1; num >= 0; num--)
        {
            if (!this.aiTargets[num].ValidateTarget())
            {
                this.aiTargets.RemoveAt(num);
            }
        }
    }

    public void FindPriorityTargets()
    {
        if (this.aiTargets == null)
        {
            this.aiTargets = new List<AITarget>();
        }
        GameManager.GetWizard(this.owner);
        this.ValidatePriorityTargets();
        this.FindLocationsToClear();
        this.FindDangersToClear();
        this.aiTargets.Sort(delegate(AITarget a, AITarget b)
        {
            if (a.preparationOnly != b.preparationOnly)
            {
                return a.preparationOnly.CompareTo(b.preparationOnly);
            }
            if (a.targetAchievable != b.targetAchievable)
            {
                if (!a.targetAchievable)
                {
                    return 1;
                }
                return -1;
            }
            if (a.HasAssignee() && b.HasAssignee())
            {
                int distanceTo = a.targetAssignee.Get().GetDistanceTo(a.GetAsGroup());
                int distanceTo2 = b.targetAssignee.Get().GetDistanceTo(b.GetAsGroup());
                return distanceTo.CompareTo(distanceTo2);
            }
            return (a.targetIsGroup != b.targetIsGroup) ? ((!a.targetIsGroup) ? 1 : (-1)) : 0;
        });
    }

    public void AssignPriorityTargets()
    {
        foreach (AITarget aiTarget in this.aiTargets)
        {
            aiTarget.EnsureAssingee();
        }
    }

    public IEnumerator Update()
    {
        if (this.aiTargets == null)
        {
            this.aiTargets = new List<AITarget>();
        }
        this.FindPriorityTargets();
        this.AssignPriorityTargets();
        if (this.aiTargets.Count < 1)
        {
            yield break;
        }
        int totalFullUpdates = 0;
        for (int i = 0; i < this.aiTargets.Count; i++)
        {
            bool flag = totalFullUpdates < 5 && this.aiTargets[i].nextUpdate <= TurnManager.GetTurnNumber();
            if (flag)
            {
                totalFullUpdates++;
            }
            yield return this.aiTargets[i].Update(flag);
        }
    }

    public AITarget GetSupportedTarget(TownLocation tl)
    {
        if (this.aiTargets == null)
        {
            return null;
        }
        foreach (AITarget aiTarget in this.aiTargets)
        {
            if (aiTarget.supplyTowns != null && aiTarget.supplyTowns.Count >= 1 && aiTarget.supplyTowns.Find((Reference<TownLocation> o) => o?.Get() == tl) != null)
            {
                if (aiTarget.ValidateTarget())
                {
                    return null;
                }
                return aiTarget;
            }
        }
        return null;
    }

    public AITarget GetAssignedTarget(Group g, bool useAdvancedPrioritization = false)
    {
        if (this.aiTargets == null || this.aiTargets.Count < 1)
        {
            return null;
        }
        AITarget aITarget = null;
        if (useAdvancedPrioritization)
        {
            int num = int.MaxValue;
            foreach (AITarget aiTarget in this.aiTargets)
            {
                if (aiTarget?.targetAssignee?.Get() == g)
                {
                    int distanceTo = g.GetDistanceTo(aiTarget.GetAsGroup());
                    if (aITarget == null || (!aiTarget.preparationOnly && aiTarget.preparationOnly) || (!aiTarget.preparationOnly && aiTarget.targetAchievable && !aITarget.targetAchievable) || (!aiTarget.preparationOnly && aiTarget.targetAchievable && distanceTo < num))
                    {
                        aITarget = aiTarget;
                        num = distanceTo;
                    }
                }
            }
        }
        if (aITarget == null)
        {
            aITarget = this.aiTargets?.Find((AITarget o) => o.targetAssignee?.Get() == g);
        }
        if (aITarget == null || !aITarget.ValidateTarget())
        {
            return null;
        }
        return aITarget;
    }

    public AITarget GetIfPrimalAssignee()
    {
        if ((this.aiTargets?.Count ?? 0) > 0)
        {
            return this.aiTargets[0];
        }
        return null;
    }

    private void FindDangersToClear()
    {
        World.GetArcanus();
        List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(arcanus: true);
        List<Group> groupsOfPlane2 = GameManager.GetGroupsOfPlane(arcanus: false);
        PlayerWizard wizard = GameManager.GetWizard(this.owner);
        if (this.aiTargets == null)
        {
            this.aiTargets = new List<AITarget>();
        }
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (!(entity.Value is TownLocation townLocation) || townLocation.GetOwnerID() != this.owner)
            {
                continue;
            }
            foreach (Group g in townLocation.GetPlane().arcanusType ? groupsOfPlane : groupsOfPlane2)
            {
                if (g.IsHosted() || g.GetOwnerID() == this.owner || g.GetDistanceTo(townLocation) >= 12)
                {
                    continue;
                }
                if (g.GetOwnerID() > 0)
                {
                    DiplomaticStatus diplomaticStatus = wizard.GetDiplomacy()?.GetStatusToward(g.GetOwnerID());
                    if (diplomaticStatus == null || !diplomaticStatus.openWar)
                    {
                        continue;
                    }
                }
                if (this.aiTargets.Find((AITarget o) => o.target.Get() == g) == null)
                {
                    AITarget item = new AITarget(this.owner, g);
                    this.aiTargets.Add(item);
                }
            }
        }
    }

    private void FindLocationsToClear()
    {
        World.GetArcanus();
        List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(arcanus: true);
        List<Location> locationsOfThePlane2 = GameManager.GetLocationsOfThePlane(arcanus: false);
        PlayerWizard wizard = GameManager.GetWizard(this.owner);
        if (this.aiTargets == null)
        {
            this.aiTargets = new List<AITarget>();
        }
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (!(entity.Value is TownLocation townLocation) || townLocation.GetOwnerID() != this.owner)
            {
                continue;
            }
            foreach (Location i in townLocation.GetPlane().arcanusType ? locationsOfThePlane : locationsOfThePlane2)
            {
                if (i.GetOwnerID() == this.owner)
                {
                    continue;
                }
                int distanceTo = townLocation.GetDistanceTo(i);
                if (distanceTo > 12 || this.aiTargets.Find((AITarget o) => o.target.Get() == i) != null)
                {
                    continue;
                }
                if (i.GetOwnerID() > 0)
                {
                    if (!(i is TownLocation) && i.GetUnits().Count == 0)
                    {
                        continue;
                    }
                    DiplomaticStatus diplomaticStatus = wizard.GetDiplomacy()?.GetStatusToward(i.GetOwnerID());
                    if (i.GetOwnerID() != PlayerWizard.HumanID() && (diplomaticStatus == null || !diplomaticStatus.openWar))
                    {
                        continue;
                    }
                    if (diplomaticStatus != null && diplomaticStatus.openWar)
                    {
                        AITarget item = new AITarget(this.owner, i);
                        this.aiTargets.Add(item);
                        continue;
                    }
                }
                else if (distanceTo > 8)
                {
                    continue;
                }
                int num = i.GetLocalGroup()?.GetValue() ?? 0;
                if ((num != 0 || i is TownLocation) && num < TurnManager.GetTurnNumber() * 20)
                {
                    AITarget item2 = new AITarget(this.owner, i);
                    this.aiTargets.Add(item2);
                }
            }
        }
    }
}
