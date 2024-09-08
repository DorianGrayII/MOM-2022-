using MHUtils;
using MOM;
using System;
using System.Collections.Generic;

public class AIBattleTactics
{
    public List<AIBattleUnitPlan> attackerUnitPlans = new List<AIBattleUnitPlan>();
    public List<AIBattleUnitPlan> defenderUnitPlans = new List<AIBattleUnitPlan>();

    public List<BattleUnit> FilterByOption(List<BattleUnit> targets, AIAttackOption option)
    {
        return targets.FindAll(o => o.currentlyVisible && ReferenceEquals(this.GetPlan(o), option.target));
    }

    public AIAttackOption GetAttackOption(BattleUnit attacker, BattleUnit defender, Battle b, bool melee)
    {
        AIBattleUnitPlan plan = this.GetPlan(attacker);
        AIBattleUnitPlan dPlan = this.GetPlan(defender);
        AIAttackOption item = plan.attacknOptions.Find(o => (o.melee == melee) && ReferenceEquals(o.target, dPlan));
        if (item == null)
        {
            Multitype<int, int> multitype = attacker.GetStrategicAttackGain(defender, b, melee);
            item = new AIAttackOption {
                melee = melee,
                target = dPlan
            };
            if ((multitype == null) || !defender.IsAlive())
            {
                item.attackValid = false;
            }
            else
            {
                item.attackValid = true;
                item.ownValueChange = multitype.t0;
                item.targetValueChange = multitype.t1;
            }
            plan.attacknOptions.Add(item);
        }
        return item;
    }

    public List<AIAttackOption> GetOptions(BattleUnit attacker, List<BattleUnit> defender, Battle b)
    {
        List<AIAttackOption> list = new List<AIAttackOption>();
        if (attacker.Mp > 0)
        {
            if (attacker.GetCurentFigure().rangedAmmo > 0)
            {
                foreach (BattleUnit unit in defender)
                {
                    AIAttackOption item = this.GetAttackOption(attacker, unit, b, false);
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            foreach (BattleUnit unit2 in defender)
            {
                AIAttackOption item = this.GetAttackOption(attacker, unit2, b, true);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    public AIBattleUnitPlan GetPlan(BattleUnit b)
    {
        if (b == null)
        {
            return null;
        }
        int value = b.GetBattleUnitValue();
        List<AIBattleUnitPlan> list = b.attackingSide ? this.attackerUnitPlans : this.defenderUnitPlans;
        int attCount = b.attributes.GetFinalDictionary(false).Count;
        AIBattleUnitPlan item = list.Find(o => !o.obsolete && ((o.source == b.dbSource.Get()) && ((o.sourceValue == value) && (o.attributeCount == attCount))));
        if (item == null)
        {
            item = new AIBattleUnitPlan {
                source = b.dbSource.Get(),
                sourceValue = value,
                attributeCount = attCount
            };
            list.Add(item);
        }
        return item;
    }

    public bool IsFromOptionType(AIAttackOption option, BattleUnit target)
    {
        return ReferenceEquals(this.GetPlan(target), option.target);
    }
}

