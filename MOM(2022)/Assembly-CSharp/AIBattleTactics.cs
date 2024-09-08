using System.Collections.Generic;
using MHUtils;
using MOM;

public class AIBattleTactics
{
    public List<AIBattleUnitPlan> attackerUnitPlans = new List<AIBattleUnitPlan>();

    public List<AIBattleUnitPlan> defenderUnitPlans = new List<AIBattleUnitPlan>();

    public AIBattleUnitPlan GetPlan(BattleUnit b)
    {
        if (b == null)
        {
            return null;
        }
        int value = b.GetBattleUnitValue();
        List<AIBattleUnitPlan> list = (b.attackingSide ? this.attackerUnitPlans : this.defenderUnitPlans);
        int attCount = b.attributes.GetFinalDictionary().Count;
        AIBattleUnitPlan aIBattleUnitPlan = list.Find((AIBattleUnitPlan o) => !o.obsolete && o.source == b.dbSource.Get() && o.sourceValue == value && o.attributeCount == attCount);
        if (aIBattleUnitPlan == null)
        {
            aIBattleUnitPlan = new AIBattleUnitPlan();
            aIBattleUnitPlan.source = b.dbSource.Get();
            aIBattleUnitPlan.sourceValue = value;
            aIBattleUnitPlan.attributeCount = attCount;
            list.Add(aIBattleUnitPlan);
        }
        return aIBattleUnitPlan;
    }

    public List<AIAttackOption> GetOptions(BattleUnit attacker, List<BattleUnit> defender, Battle b)
    {
        List<AIAttackOption> list = new List<AIAttackOption>();
        if (attacker.Mp <= 0)
        {
            return list;
        }
        if (attacker.GetCurentFigure().rangedAmmo > 0)
        {
            foreach (BattleUnit item in defender)
            {
                AIAttackOption attackOption = this.GetAttackOption(attacker, item, b, melee: false);
                if (!list.Contains(attackOption))
                {
                    list.Add(attackOption);
                }
            }
        }
        foreach (BattleUnit item2 in defender)
        {
            AIAttackOption attackOption2 = this.GetAttackOption(attacker, item2, b, melee: true);
            if (!list.Contains(attackOption2))
            {
                list.Add(attackOption2);
            }
        }
        return list;
    }

    public AIAttackOption GetAttackOption(BattleUnit attacker, BattleUnit defender, Battle b, bool melee)
    {
        AIBattleUnitPlan plan = this.GetPlan(attacker);
        AIBattleUnitPlan dPlan = this.GetPlan(defender);
        AIAttackOption aIAttackOption = plan.attacknOptions.Find((AIAttackOption o) => o.melee == melee && o.target == dPlan);
        if (aIAttackOption == null)
        {
            Multitype<int, int> strategicAttackGain = attacker.GetStrategicAttackGain(defender, b, melee);
            aIAttackOption = new AIAttackOption();
            aIAttackOption.melee = melee;
            aIAttackOption.target = dPlan;
            if (strategicAttackGain == null || !defender.IsAlive())
            {
                aIAttackOption.attackValid = false;
            }
            else
            {
                aIAttackOption.attackValid = true;
                aIAttackOption.ownValueChange = strategicAttackGain.t0;
                aIAttackOption.targetValueChange = strategicAttackGain.t1;
            }
            plan.attacknOptions.Add(aIAttackOption);
        }
        return aIAttackOption;
    }

    public bool IsFromOptionType(AIAttackOption option, BattleUnit target)
    {
        return this.GetPlan(target) == option.target;
    }

    public List<BattleUnit> FilterByOption(List<BattleUnit> targets, AIAttackOption option)
    {
        return targets.FindAll((BattleUnit o) => o.currentlyVisible && this.GetPlan(o) == option.target);
    }
}
