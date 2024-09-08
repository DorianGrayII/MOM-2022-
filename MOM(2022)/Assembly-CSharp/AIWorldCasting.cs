// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AIWorldCasting
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class AIWorldCasting
{
    [ProtoMember(1)]
    public Reference<PlayerWizardAI> owner;

    [ProtoMember(2)]
    public bool increasedWillOfSummon;

    public AIWorldCasting()
    {
    }

    public AIWorldCasting(Reference<PlayerWizardAI> owner)
    {
        this.owner = owner;
    }

    public IEnumerator ConsiderMagicalResources()
    {
        PlayerWizardAI wizard = this.owner.Get();
        MagicAndResearch mr = wizard.GetMagicAndResearch();
        int totalPower = mr.GetTotalPower();
        this.EnsureSpellForResearch(mr);
        Spell spell = (Spell)SPELL.SPELL_OF_MASTERY;
        if (mr.curentlyCasted != spell)
        {
            foreach (PlayerWizard wizard2 in GameManager.GetWizards())
            {
                if (wizard2 != wizard && wizard2.GetMagicAndResearch().curentlyCastSpell == spell)
                {
                    EnchantmentInstance enchantmentInstance = GameManager.Get().GetEnchantments().Find((EnchantmentInstance o) => o.owner.GetEntity() == wizard && o.source.Get() == (Enchantment)ENCH.PLANAR_SEAL);
                    if (enchantmentInstance != null)
                    {
                        GameManager.Get().RemoveEnchantment(enchantmentInstance);
                    }
                }
            }
        }
        int num = this.CalculateCurentManaLocationCosts();
        int num2 = this.CalculateCurentManaUnitCosts();
        int num3 = num + num2;
        bool flag = mr.curentlyCastSpell != null;
        if (flag)
        {
            wizard.GetTotalCastingSkill();
        }
        float num4 = 0.3f;
        bool flag2 = wizard.mana < 3 * wizard.GetTotalCastingSkill();
        for (float num5 = num4; num5 < 1f; num5 += 0.2f)
        {
            mr.manaShare = Mathf.Clamp01(num4);
            mr.researchShare = (1f - mr.manaShare) * 0.5f;
            mr.skillShare = 1f - mr.manaShare - mr.researchShare;
            int num6 = wizard.CalculateManaIncome(includeUpkeep: true);
            if ((!flag2 || !(num5 < 0.6f) || num6 >= 25) && (num6 > totalPower / 4 || -num6 < wizard.mana / 20 || (num5 >= 0.5f && num6 > 0) || (num5 >= 0.7f && -num6 < wizard.mana / 5)))
            {
                break;
            }
        }
        if (mr.manaShare > 0.7f)
        {
            List<global::MOM.Unit> list = new List<global::MOM.Unit>();
            foreach (global::MOM.Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetOwnerID() != wizard.ID || registeredGroup.GetUnits() == null)
                {
                    continue;
                }
                if (registeredGroup.GetLocationHostSmart() != null)
                {
                    if (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType)
                    {
                        continue;
                    }
                    AILocationTactic locationTactic = registeredGroup.GetLocationHostSmart().locationTactic;
                    if (locationTactic != null && locationTactic.dangerRank > 1)
                    {
                        continue;
                    }
                }
                else
                {
                    AIGroupDesignation designation = registeredGroup.GetDesignation();
                    if (designation != null && (designation.designation == AIGroupDesignation.Designation.AggressionMedium || designation.designation == AIGroupDesignation.Designation.AggressionLong))
                    {
                        continue;
                    }
                }
                foreach (Reference<global::MOM.Unit> unit2 in registeredGroup.GetUnits())
                {
                    if ((!(registeredGroup.transporter != null) || registeredGroup.transporter.Get() != unit2.Get()) && (!(unit2.Get().GetAttributes().GetFinal(TAG.UPKEEP_MANA) <= FInt.ZERO) || unit2.Get().IsHero()))
                    {
                        list.Add(unit2);
                    }
                }
            }
            if (list.Count > 0)
            {
                if (list.Count > 1)
                {
                    list.Sort((global::MOM.Unit a, global::MOM.Unit b) => -a.GetWorldUnitValue().CompareTo(b.GetWorldUnitValue()));
                }
                if (list.Count > 0)
                {
                    global::MOM.Unit unit = list[list.Count - 1];
                    unit.GetAttributes().GetFinal(TAG.UPKEEP_MANA);
                    unit.Destroy();
                }
            }
        }
        if (!flag)
        {
            if (wizard.banishedTurn > 0)
            {
                mr.curentlyCastSpell = (Spell)SPELL.SPELL_OF_RETURN;
                mr.craftItemSpell = null;
            }
            else if ((double)num3 < (double)totalPower * 0.2 || wizard.mana > wizard.GetTotalCastingSkill() * 4)
            {
                if (wizard.PrepareCastingForWarEffort())
                {
                    yield break;
                }
                Spell spell2 = this.PriorityTargetAcquired(wizard);
                if (spell2 != null)
                {
                    if ((double)(num3 + spell2.GetUpkeepCost()) > (double)totalPower * 0.5)
                    {
                        spell2 = null;
                    }
                }
                else if (spell2 == null)
                {
                    spell2 = this.PickSpell(totalPower);
                    if (spell2 != null && (double)(num3 + spell2.GetUpkeepCost()) > (double)totalPower * 0.35)
                    {
                        spell2 = null;
                    }
                }
                if (spell2 != null)
                {
                    mr.curentlyCastSpell = spell2;
                    mr.craftItemSpell = null;
                    if (spell2 == (Spell)SPELL.SPELL_OF_MASTERY)
                    {
                        PopupCastingSoM.OpenPopup(null, this.owner.Get());
                        while (PopupCastingSoM.IsOpen())
                        {
                            yield return null;
                        }
                    }
                }
            }
            else if ((double)num3 > (double)totalPower * 0.3)
            {
                this.ConsiderSpellAsReplacement(null);
            }
        }
        if (mr.curentlyCastSpell != null && mr.castingProgress >= mr.curentlyCastSpell.GetWorldCastingCost(this.owner.Get()))
        {
            Spell curentlyCastSpell = mr.curentlyCastSpell;
            Debug.Log("Ai " + wizard.name + " finished casting of " + curentlyCastSpell.dbName);
            this.PrepareManaForScaledSpells(curentlyCastSpell);
            if (!this.Cast(curentlyCastSpell))
            {
                int castingProgress = mr.castingProgress;
                this.owner.Get().mana += castingProgress;
            }
            else
            {
                this.UseManaForScaledSpells();
                if (curentlyCastSpell == (Spell)SPELL.SPELL_OF_RETURN)
                {
                    string message = global::DBUtils.Localization.Get("UI_WIZARD_RETURNED_DES", true, wizard.name);
                    PopupGeneral.OpenPopup(null, "UI_WIZARD_RETURNED", message, "UI_OK");
                }
            }
            this.ClearManaForScaledSpells();
            mr.castingProgress = 0;
            mr.curentlyCastSpell = null;
        }
        yield return null;
    }

    private Spell PriorityTargetAcquired(PlayerWizardAI wizard)
    {
        wizard.priorityTargets?.ValidatePriorityTargets();
        List<AITarget> list = wizard.priorityTargets?.aiTargets;
        if (list != null && list.Count > 0)
        {
            AITarget firstTarget = list[0];
            global::MOM.Group asGroup = firstTarget.GetAsGroup();
            global::MOM.Group group = firstTarget.targetAssignee?.Get();
            if (asGroup != null)
            {
                bool flag = false;
                if (group == null || group.GetUnits().Count < 4 || (group.GetUnits().Count < 7 && firstTarget.inRangeForSummon))
                {
                    flag = true;
                }
                List<DBReference<Spell>> spells = wizard.GetSpells();
                Spell spell = null;
                if (flag && !firstTarget.inRangeForSummon && firstTarget.requiresSummoningCircle)
                {
                    Spell s = (Spell)SPELL.SUMMONING_CIRCLE;
                    spell = spells.Find((DBReference<Spell> o) => o.Get() == s);
                    if (spell == null)
                    {
                        firstTarget.requiresSummoningCircle = false;
                    }
                    else
                    {
                        TownLocation summoningCircleTarget = this.GetSummoningCircleTarget();
                        if (summoningCircleTarget != null && this.owner.Get().summoningCircle?.Get() != summoningCircleTarget && firstTarget.GetAsGroup() != null && firstTarget.GetAsGroup().GetDistanceTo(this.owner.Get().summoningCircle?.Get()) > firstTarget.GetAsGroup().GetDistanceTo(summoningCircleTarget) + 10)
                        {
                            return s;
                        }
                        firstTarget.requiresSummoningCircle = false;
                    }
                }
                if (flag && firstTarget.inRangeForSummon)
                {
                    float num = 0f;
                    for (int i = 0; i < spells.Count; i++)
                    {
                        Spell spell2 = spells[i].Get();
                        if (this.SummonFilter(spell2))
                        {
                            Multitype<object, float> multitype = this.FindSummonTarget(spell2);
                            if (multitype != null && num * 1.25f < multitype.t1)
                            {
                                spell = spell2;
                                num = multitype.t1;
                            }
                        }
                    }
                    if (spell != null)
                    {
                        return spell;
                    }
                }
                return spells.Find((DBReference<Spell> o) => o.Get() == firstTarget.topEfficiencySpell?.Get());
            }
        }
        return null;
    }

    private bool ConsiderSpellAsReplacement(Spell spellConsidered)
    {
        return false;
    }

    private void EnsureSpellForResearch(MagicAndResearch mr)
    {
        if (!(mr.curentlyResearched == null))
        {
            return;
        }
        mr.FillResearchOptions();
        if (mr.curentResearchOptions == null || mr.curentResearchOptions.Count <= 0)
        {
            return;
        }
        Spell spell = null;
        foreach (DBReference<Spell> curentResearchOption in mr.curentResearchOptions)
        {
            if (spell == null || spell.researchCost > curentResearchOption.Get().researchCost)
            {
                spell = curentResearchOption.Get();
            }
        }
        mr.curentlyResearched = spell;
    }

    private void Notify(object target, Spell s, bool blocked)
    {
        SummaryInfo summaryInfo = new SummaryInfo
        {
            summaryType = (blocked ? SummaryInfo.SummaryType.eEnemySpellBlocked : SummaryInfo.SummaryType.eEnemySpellSuccessful),
            spell = s
        };
        if (target is global::MOM.Location location)
        {
            PlayerWizard wizardOwner = location.GetWizardOwner();
            if (wizardOwner != null && wizardOwner.IsHuman)
            {
                summaryInfo.location = location;
                summaryInfo.name = location.GetName();
                GameManager.GetHumanWizard().AddNotification(summaryInfo);
            }
        }
        else if (target is global::MOM.Group group)
        {
            PlayerWizard wizard = GameManager.GetWizard(group.GetOwnerID());
            if (wizard != null && wizard.IsHuman)
            {
                summaryInfo.group = group;
                GameManager.GetHumanWizard().AddNotification(summaryInfo);
            }
        }
        else if (target is PlayerWizard playerWizard)
        {
            if (playerWizard.IsHuman)
            {
                summaryInfo.casterAI = this.owner.Get();
                GameManager.GetHumanWizard().AddNotification(summaryInfo);
            }
        }
        else if (target is GameManager || target == null)
        {
            summaryInfo.casterAI = this.owner.Get();
            summaryInfo.graphic = s.GetDescriptionInfo().graphic;
            GameManager.GetHumanWizard().AddNotification(summaryInfo);
        }
    }

    private bool CastSummoningCircle(Spell s)
    {
        TownLocation summoningCircleTarget = this.GetSummoningCircleTarget();
        if (summoningCircleTarget != null && this.owner.Get().summoningCircle?.Get() != summoningCircleTarget && this.owner.Get().GetID() == summoningCircleTarget.GetOwnerID())
        {
            bool flag = false;
            if (!this.CounterMagicWorld(s))
            {
                if ((bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), summoningCircleTarget, s))
                {
                    this.increasedWillOfSummon = true;
                }
                else
                {
                    Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
                }
            }
            else
            {
                this.Notify(summoningCircleTarget, s, !flag);
            }
            return true;
        }
        return false;
    }

    private bool CastWorldSummon(Spell s, Multitype<object, float> target)
    {
        if (this.owner.Get().wizardTower != null)
        {
            if (!string.IsNullOrEmpty(s.targetingScript) && !(bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), this.owner.Get().wizardTower.Get(), s))
            {
                return false;
            }
            bool flag = false;
            if (!this.CounterMagicWorld(s))
            {
                if (!(bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), s, this.owner.Get().wizardTower.Get(), null))
                {
                    Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
                }
            }
            else
            {
                this.Notify(null, s, !flag);
            }
            this.increasedWillOfSummon = false;
            return true;
        }
        return false;
    }

    private bool CastTargetLocation(Spell s, Multitype<object, float> target)
    {
        if (s.targetType == DataBase.Get<TargetType>(TARGET_TYPE.LOCATION_ENEMY) || s.targetType == DataBase.Get<TargetType>(TARGET_TYPE.LOCATION_ENEMY_NODE))
        {
            Multitype<object, float> multitype = target ?? this.FindEnemyLocation(s);
            if (multitype == null || multitype.t0 == null)
            {
                return false;
            }
            bool flag = false;
            if (!this.CounterMagicWorld(s))
            {
                flag = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype.t0, s);
                if (flag)
                {
                    if (multitype?.t0 is global::MOM.Location location && location.GetOwnerID() == PlayerWizard.HumanID())
                    {
                        this.Notify(multitype.t0, s, !flag);
                    }
                }
                else
                {
                    Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
                }
            }
            else
            {
                this.Notify(multitype.t0, s, !flag);
            }
            return true;
        }
        Multitype<object, float> multitype2 = target ?? this.FindAllyLocation(s);
        if (multitype2 == null || multitype2.t0 == null)
        {
            return false;
        }
        bool flag2 = false;
        if (!this.CounterMagicWorld(s))
        {
            flag2 = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype2.t0, s);
            if (flag2)
            {
                this.Notify(multitype2.t0, s, !flag2);
            }
            else
            {
                Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
            }
        }
        else
        {
            this.Notify(multitype2.t0, s, !flag2);
        }
        return true;
    }

    private bool CastTargetUnit(Spell s, Multitype<object, float> target)
    {
        if (s.targetType == DataBase.Get<TargetType>(TARGET_TYPE.UNIT_ENEMY))
        {
            Multitype<object, float> multitype = target ?? this.FindEnemyUnit(s);
            if (multitype == null || multitype.t0 == null)
            {
                return false;
            }
            bool flag = false;
            if (!this.CounterMagicWorld(s))
            {
                flag = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype.t0, s);
                if (flag)
                {
                    if (multitype?.t0 is global::MOM.Unit unit && unit.GetWizardOwnerID() == PlayerWizard.HumanID())
                    {
                        this.Notify(multitype.t0, s, !flag);
                    }
                }
                else
                {
                    Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
                }
            }
            else
            {
                this.Notify(multitype.t0, s, !flag);
            }
            return true;
        }
        Multitype<object, float> multitype2 = target ?? this.FindAllyUnit(s);
        if (multitype2 == null || multitype2.t0 == null)
        {
            return false;
        }
        bool flag2 = false;
        if (!this.CounterMagicWorld(s))
        {
            flag2 = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype2.t0, s);
            if (flag2)
            {
                this.Notify(multitype2.t0, s, !flag2);
            }
            else
            {
                Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
            }
        }
        else
        {
            this.Notify(multitype2.t0, s, !flag2);
        }
        return true;
    }

    private bool CastTargetGroup(Spell s, Multitype<object, float> target)
    {
        if (s.targetType == DataBase.Get<TargetType>(TARGET_TYPE.GROUP_ENEMY))
        {
            Multitype<object, float> multitype = target ?? this.FindEnemyGroup(s);
            if (multitype == null || multitype.t0 == null)
            {
                return false;
            }
            bool flag = false;
            if (!this.CounterMagicWorld(s))
            {
                flag = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype.t0, s);
                if (flag)
                {
                    if (multitype?.t0 is global::MOM.Group group && group.GetOwnerID() == PlayerWizard.HumanID())
                    {
                        this.Notify(multitype.t0, s, !flag);
                    }
                }
                else
                {
                    Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
                }
            }
            else
            {
                this.Notify(multitype.t0, s, !flag);
            }
            return true;
        }
        Multitype<object, float> multitype2 = target ?? this.FindAllyGroup(s);
        if (multitype2 == null || multitype2.t0 == null)
        {
            return false;
        }
        bool flag2 = false;
        if (!this.CounterMagicWorld(s))
        {
            flag2 = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype2.t0, s);
            if (flag2)
            {
                this.Notify(multitype2.t0, s, !flag2);
            }
            else
            {
                Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
            }
        }
        else
        {
            this.Notify(multitype2.t0, s, !flag2);
        }
        return true;
    }

    private bool CastTargetWizard(Spell s, Multitype<object, float> target)
    {
        if (s.targetType == DataBase.Get<TargetType>(TARGET_TYPE.WIZARD_ENEMY))
        {
            Multitype<object, float> multitype = target ?? this.FindEnemyWizard(s);
            if (multitype == null || multitype.t0 == null)
            {
                return false;
            }
            bool flag = false;
            if (!this.CounterMagicWorld(s))
            {
                flag = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype.t0, s);
                if (flag)
                {
                    if (multitype?.t0 is PlayerWizard playerWizard && playerWizard.GetID() == PlayerWizard.HumanID())
                    {
                        this.Notify(multitype.t0, s, !flag);
                    }
                }
                else
                {
                    Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
                }
            }
            else
            {
                this.Notify(multitype.t0, s, !flag);
            }
            return true;
        }
        Multitype<object, float> multitype2 = target ?? this.FindAllyWizard(s);
        if (multitype2 == null || multitype2.t0 == null)
        {
            return false;
        }
        bool flag2 = false;
        if (!this.CounterMagicWorld(s))
        {
            flag2 = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype2.t0, s);
            if (flag2)
            {
                this.Notify(multitype2.t0, s, !flag2);
            }
            else
            {
                Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
            }
        }
        else
        {
            this.Notify(multitype2.t0, s, !flag2);
        }
        return true;
    }

    private bool CastTargetGlobal(Spell s, Multitype<object, float> target)
    {
        Multitype<object, float> multitype = this.FindGlobalSpellValue(s);
        if (multitype == null || multitype.t1 < 1f)
        {
            return false;
        }
        bool flag = false;
        if (!this.CounterMagicWorld(s))
        {
            flag = (bool)ScriptLibrary.Call(s.worldScript, this.owner.Get(), multitype.t0, s);
            if (flag)
            {
                this.Notify(multitype.t0, s, !flag);
            }
            else
            {
                Debug.LogWarning("Spell " + s.dbName + " passed all tests, but failed to cast in the end");
            }
        }
        else
        {
            this.Notify(multitype.t0, s, !flag);
        }
        return true;
    }

    public bool Cast(Spell s, Multitype<object, float> target = null)
    {
        if (s == (Spell)SPELL.SUMMONING_CIRCLE)
        {
            return this.CastSummoningCircle(s);
        }
        if (s.targetType.enumType == ETargetType.WorldSummon)
        {
            return this.CastWorldSummon(s, target);
        }
        if (s.targetType.enumType == ETargetType.TargetLocation)
        {
            return this.CastTargetLocation(s, target);
        }
        if (s.targetType.enumType == ETargetType.TargetUnit)
        {
            return this.CastTargetUnit(s, target);
        }
        if (s.targetType.enumType == ETargetType.TargetGroup)
        {
            return this.CastTargetGroup(s, target);
        }
        if (s.targetType.enumType == ETargetType.TargetWizard)
        {
            return this.CastTargetWizard(s, target);
        }
        if (s.targetType.enumType == ETargetType.TargetGlobal)
        {
            return this.CastTargetGlobal(s, target);
        }
        Debug.LogWarning("Unsupported spell type for AI: " + s.targetType.dbName);
        return false;
    }

    private int CalculateCurentManaLocationCosts()
    {
        PlayerWizardAI playerWizardAI = this.owner.Get();
        int num = 0;
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (!(registeredLocation is TownLocation townLocation) || registeredLocation.GetOwnerID() != playerWizardAI.ID || townLocation.buildings == null)
            {
                continue;
            }
            foreach (DBReference<Building> building in townLocation.buildings)
            {
                num += building.Get().upkeepManaCost;
            }
        }
        return num;
    }

    private int CalculateCurentManaUnitCosts()
    {
        PlayerWizardAI playerWizardAI = this.owner.Get();
        int num = 0;
        foreach (global::MOM.Group registeredGroup in GameManager.Get().registeredGroups)
        {
            if (registeredGroup.GetOwnerID() != playerWizardAI.ID || registeredGroup.GetUnits() == null || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType))
            {
                continue;
            }
            foreach (Reference<global::MOM.Unit> unit in registeredGroup.GetUnits())
            {
                num += unit.Get().GetAttributes().GetFinal(TAG.UPKEEP_MANA)
                    .ToInt();
            }
        }
        return num;
    }

    private Multitype<object, float> FindSummonTarget(Spell s)
    {
        if (s.worldScript == null || s.targetType.enumType != ETargetType.WorldSummon)
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        int num = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), null, s);
        int num2 = (s.worldCost + s.upkeepCost * 10) / 3;
        float t = ((num2 == 0) ? ((float)num) : ((float)num / (float)num2));
        return new Multitype<object, float>(null, t);
    }

    private Multitype<object, float> FindAllyLocation(Spell s)
    {
        if (s.worldScript == null || s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.LOCATION_FRIENDLY))
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        global::MOM.Location t = null;
        float num = 0f;
        List<global::MOM.Location> registeredLocations = GameManager.Get().registeredLocations;
        for (int i = 0; i < registeredLocations.Count; i++)
        {
            global::MOM.Location location = registeredLocations[i];
            if (location is TownLocation && location.GetOwnerID() == this.owner.Get().GetID() && (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), location, s)))
            {
                int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), location, s);
                int num3 = s.worldCost + s.upkeepCost * 10;
                float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                if (num4 > num)
                {
                    num = num4;
                    t = location;
                }
            }
        }
        return new Multitype<object, float>(t, num);
    }

    private Multitype<object, float> FindEnemyLocation(Spell s)
    {
        if (s.worldScript == null)
        {
            return null;
        }
        if (s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.LOCATION_ENEMY))
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        global::MOM.Location t = null;
        float num = 0f;
        List<global::MOM.Location> registeredLocations = GameManager.Get().registeredLocations;
        for (int i = 0; i < registeredLocations.Count; i++)
        {
            global::MOM.Location location = registeredLocations[i];
            if (!(location is TownLocation) || location.GetOwnerID() == 0 || location.GetOwnerID() == this.owner.Get().GetID())
            {
                continue;
            }
            if (location.GetPlane().arcanusType)
            {
                if (!this.owner.Get().arcanusVisibility.knownLocationPositions.Contains(location.GetPosition()))
                {
                    continue;
                }
            }
            else if (!this.owner.Get().myrrorVisibility.knownLocationPositions.Contains(location.GetPosition()))
            {
                continue;
            }
            if (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), location, s))
            {
                int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), location, s);
                int num3 = s.worldCost + s.upkeepCost * 10;
                float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                if (num4 > num)
                {
                    num = num4;
                    t = location;
                }
            }
        }
        return new Multitype<object, float>(t, num);
    }

    private Multitype<object, float> FindAllyUnit(Spell s)
    {
        if (s.worldScript == null)
        {
            return null;
        }
        if (s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.UNIT_FRIENDLY) && s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.UNIT_FRIENDLY_HERO))
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        global::MOM.Unit unit = null;
        float num = 0f;
        AITarget aITarget = this.owner.Get().priorityTargets?.GetIfPrimalAssignee();
        if (aITarget?.topEfficiencySpell?.Get() != s)
        {
            aITarget = null;
        }
        if (aITarget?.targetAssignee?.Get() != null && aITarget.targetAssignee.Get().alive)
        {
            foreach (Reference<global::MOM.Unit> unit2 in aITarget.targetAssignee.Get().GetUnits())
            {
                if (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), unit2.Get(), s))
                {
                    int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), unit2.Get(), s);
                    int num3 = s.worldCost + s.upkeepCost * 10;
                    float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                    if (num4 > num)
                    {
                        num = num4;
                        unit = unit2;
                    }
                }
            }
            if (unit != null)
            {
                return new Multitype<object, float>(unit, num);
            }
        }
        List<global::MOM.Group> registeredGroups = GameManager.Get().registeredGroups;
        for (int i = 0; i < registeredGroups.Count; i++)
        {
            global::MOM.Group group = registeredGroups[i];
            if (group.GetOwnerID() != this.owner.Get().GetID() || !group.alive || !group.doomStack || (group.GetLocationHost()?.otherPlaneLocation?.Get() != null && group.plane.arcanusType))
            {
                continue;
            }
            foreach (Reference<global::MOM.Unit> unit3 in group.GetUnits())
            {
                if (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), unit3.Get(), s))
                {
                    int num5 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), unit3.Get(), s);
                    int num6 = s.worldCost + s.upkeepCost * 10;
                    float num7 = ((num6 == 0) ? ((float)num5) : ((float)num5 / (float)num6));
                    if (num7 > num)
                    {
                        num = num7;
                        unit = unit3;
                    }
                }
            }
        }
        if (unit == null)
        {
            return null;
        }
        return new Multitype<object, float>(unit, num);
    }

    private Multitype<object, float> FindAllyGroup(Spell s)
    {
        if (s.worldScript == null)
        {
            return null;
        }
        if (s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.GROUP_FRIENDLY))
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        global::MOM.Group t = null;
        float num = 0f;
        AITarget aITarget = this.owner.Get().priorityTargets?.GetIfPrimalAssignee();
        if (aITarget?.topEfficiencySpell?.Get() != s)
        {
            aITarget = null;
        }
        if (aITarget?.targetAssignee?.Get() != null && aITarget.targetAssignee.Get().alive)
        {
            global::MOM.Group group = aITarget.targetAssignee.Get();
            bool flag = true;
            if (!string.IsNullOrEmpty(s.targetingScript))
            {
                flag = (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), group, s);
            }
            if (flag)
            {
                int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), group, s);
                int num3 = s.worldCost + s.upkeepCost * 10;
                float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                if (num4 > 0f)
                {
                    return new Multitype<object, float>(group, num4);
                }
            }
        }
        List<global::MOM.Group> registeredGroups = GameManager.Get().registeredGroups;
        for (int i = 0; i < registeredGroups.Count; i++)
        {
            global::MOM.Group group2 = registeredGroups[i];
            if (group2.GetOwnerID() == this.owner.Get().GetID() && group2.alive && group2.doomStack && (group2.GetLocationHost()?.otherPlaneLocation?.Get() == null || !group2.plane.arcanusType) && (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), group2, s)))
            {
                int num5 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), group2, s);
                int num6 = s.worldCost + s.upkeepCost * 10;
                float num7 = ((num6 == 0) ? ((float)num5) : ((float)num5 / (float)num6));
                if (num7 > num)
                {
                    num = num7;
                    t = group2;
                }
            }
        }
        return new Multitype<object, float>(t, num);
    }

    private Multitype<object, float> FindEnemyUnit(Spell s)
    {
        if (s.worldScript == null)
        {
            return null;
        }
        if (s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.UNIT_ENEMY))
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        global::MOM.Unit t = null;
        float num = 0f;
        List<global::MOM.Group> registeredGroups = GameManager.Get().registeredGroups;
        for (int i = 0; i < registeredGroups.Count; i++)
        {
            global::MOM.Group group = registeredGroups[i];
            if (group.GetOwnerID() == this.owner.Get().GetID() || (group.GetLocationHost()?.otherPlaneLocation?.Get() != null && group.plane.arcanusType))
            {
                continue;
            }
            foreach (Reference<global::MOM.Unit> unit in group.GetUnits())
            {
                if (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), group, s))
                {
                    int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), unit, s);
                    int num3 = s.worldCost + s.upkeepCost * 10;
                    float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                    if (num4 > num)
                    {
                        num = num4;
                        t = unit;
                    }
                }
            }
        }
        return new Multitype<object, float>(t, num);
    }

    private Multitype<object, float> FindEnemyGroup(Spell s)
    {
        if (s.worldScript == null)
        {
            return null;
        }
        if (s.targetType != DataBase.Get<TargetType>(TARGET_TYPE.GROUP_ENEMY))
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        global::MOM.Group t = null;
        float num = 0f;
        List<global::MOM.Group> registeredGroups = GameManager.Get().registeredGroups;
        for (int i = 0; i < registeredGroups.Count; i++)
        {
            global::MOM.Group group = registeredGroups[i];
            if (group.GetOwnerID() != this.owner.Get().GetID() && (group.GetLocationHost()?.otherPlaneLocation?.Get() == null || !group.plane.arcanusType) && (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), group, s)))
            {
                int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), group, s);
                int num3 = s.worldCost + s.upkeepCost * 10;
                float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                if (num4 > num)
                {
                    num = num4;
                    t = group;
                }
            }
        }
        return new Multitype<object, float>(t, num);
    }

    private Multitype<object, float> FindAllyWizard(Spell s)
    {
        if (s.worldScript == null || s.targetType.enumType != ETargetType.TargetWizard)
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        if (!string.IsNullOrEmpty(s.targetingScript) && !(bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), this.owner.Get(), s))
        {
            return null;
        }
        int num = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), this.owner.Get(), s);
        int num2 = s.worldCost + s.upkeepCost * 10;
        float t = ((num2 == 0) ? ((float)num) : ((float)num / (float)num2));
        return new Multitype<object, float>(this.owner.Get(), t);
    }

    private Multitype<object, float> FindEnemyWizard(Spell s)
    {
        if (s.worldScript == null || s.targetType.enumType != ETargetType.TargetWizard)
        {
            return null;
        }
        if (string.IsNullOrEmpty(s.aiWorldEvaluationScript))
        {
            Debug.LogWarning("Spell " + s.dbName + " aiWorldEvaluationScript is empty");
            return null;
        }
        PlayerWizard t = null;
        float num = 0f;
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (wizard == this.owner.Get())
            {
                continue;
            }
            DiplomaticStatus statusToward = this.owner.Get().GetDiplomacy().GetStatusToward(wizard);
            if (!(wizard is PlayerWizardAI) && statusToward != null && !statusToward.IsAllied() && statusToward.GetRelationship() <= 50 && (string.IsNullOrEmpty(s.targetingScript) || (bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), wizard, s)))
            {
                int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), this.owner.Get(), s);
                int num3 = s.worldCost + s.upkeepCost * 10;
                float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                if (num < num4)
                {
                    num = num4;
                    t = wizard;
                }
            }
        }
        return new Multitype<object, float>(t, num);
    }

    private Multitype<object, float> FIndGlobalEnchantmentTargetValue(Spell s)
    {
        GameManager obj = GameManager.Get();
        float num = 0f;
        object obj2 = null;
        int num2 = s.worldCost + s.upkeepCost * 10;
        foreach (EnchantmentInstance enchantment in obj.GetEnchantments())
        {
            int num3 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), enchantment, s);
            float num4 = ((num2 == 0) ? ((float)num3) : ((float)num3 / (float)num2));
            if (num < num4)
            {
                num = num4;
                obj2 = enchantment;
            }
        }
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (!wizard.isAlive)
            {
                continue;
            }
            foreach (EnchantmentInstance enchantment2 in wizard.GetEnchantments())
            {
                if (!enchantment2.source.Get().allowDispel)
                {
                    continue;
                }
                PlayerWizard playerWizard = enchantment2.owner.Get<PlayerWizard>();
                if (playerWizard != null)
                {
                    DiplomaticStatus statusToward = this.owner.Get().GetDiplomacy().GetStatusToward(playerWizard);
                    if (statusToward == null || (wizard != this.owner.Get() && !statusToward.openWar))
                    {
                        continue;
                    }
                }
                int num5 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), enchantment2, s);
                float num6 = ((num2 == 0) ? ((float)num5) : ((float)num5 / (float)num2));
                if (num < num6)
                {
                    num = num6;
                    obj2 = enchantment2;
                }
            }
        }
        if (obj2 != null)
        {
            return new Multitype<object, float>(obj2, num);
        }
        return new Multitype<object, float>(GameManager.Get(), 0f);
    }

    private Multitype<object, float> FIndGlobalTargetValue(Spell s)
    {
        GameManager gameManager = GameManager.Get();
        float num = 0f;
        object obj = null;
        int num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), gameManager, s);
        int num3 = s.worldCost + s.upkeepCost * 10;
        float num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
        if (num < num4)
        {
            num = num4;
            obj = gameManager;
        }
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (wizard.isAlive && this.owner.Get().GetDiplomacy().GetStatusToward(wizard) != null)
            {
                num2 = (int)ScriptLibrary.Call(s.aiWorldEvaluationScript, this.owner.Get(), wizard, s);
                num4 = ((num3 == 0) ? ((float)num2) : ((float)num2 / (float)num3));
                if (num < num4)
                {
                    num = num4;
                    obj = wizard;
                }
            }
        }
        if (obj != null)
        {
            return new Multitype<object, float>(obj, num);
        }
        return new Multitype<object, float>(GameManager.Get(), 0f);
    }

    private Multitype<object, float> FindGlobalSpellValue(Spell s)
    {
        if (s.targetType == (TargetType)TARGET_TYPE.GLOBAL_ENCHANTMENT)
        {
            return this.FIndGlobalEnchantmentTargetValue(s);
        }
        if (!string.IsNullOrEmpty(s.targetingScript) && !(bool)ScriptLibrary.Call(s.targetingScript, new SpellCastData(this.owner.Get(), null), GameManager.Get(), s))
        {
            return new Multitype<object, float>(GameManager.Get(), 0f);
        }
        return this.FIndGlobalTargetValue(s);
    }

    private Spell PickSpell(int power)
    {
        List<DBReference<Spell>> spells = this.owner.Get().GetSpells();
        DifficultyOption setting = DifficultySettingsData.GetSetting("UI_DIFF_AI_SKILL");
        float num = ((power < 25) ? 1.5f : 1f);
        float num2 = ((power < 30) ? ((float)power / 50f) : 1f);
        num *= (1 + this.owner.Get().channelerFantasticUnitsUpkeepDiscount + this.owner.Get().channelerFantasticUnitsUpkeepDiscount).ToFloat();
        float num3 = 1f + this.owner.Get().channelerFantasticUnitsUpkeepDiscount.ToFloat();
        float num4 = this.NeedOfSummon(spells) * num;
        float num5 = this.NeedOfUnitCurse() + num4;
        float num6 = this.NeedOfUnitBless() * num3 + num5;
        float num7 = this.NeedOfLocationCurse() * num2 + num6;
        float num8 = this.NeedOfLocationBless() * num3 * num2 + num7;
        float num9 = this.NeedOfWizardCurse() * num2 + num8;
        float num10 = this.NeedOfWizardBless() * num3 * num2 + num9;
        float num11 = this.NeedOfTerrainTransform() * num2 + num10;
        float num12 = this.NeedOfGlobalEffect() * num3 * num2 + num11;
        float num13 = global::UnityEngine.Random.Range(0f, num12);
        if (num13 < num4)
        {
            TownLocation summoningCircleTarget = this.GetSummoningCircleTarget();
            if (summoningCircleTarget != null && this.owner.Get().summoningCircle != summoningCircleTarget)
            {
                Spell s = (Spell)SPELL.SUMMONING_CIRCLE;
                if (spells.FindIndex((DBReference<Spell> o) => o == s) <= -1)
                {
                    return s;
                }
            }
            if (this.owner.Get().summoningCircle?.Get() == null)
            {
                Debug.LogWarning("No good location for summoning circle could be find! this should never happen as no town-case should be handled before this process");
                return null;
            }
            spells.RandomSort();
            Spell result = null;
            float num14 = 0f;
            int num15 = 0;
            for (int i = 0; i < spells.Count; i++)
            {
                Spell spell = spells[i].Get();
                if (!this.SummonFilter(spell))
                {
                    continue;
                }
                Multitype<object, float> multitype = this.FindSummonTarget(spell);
                if (multitype != null)
                {
                    if (num14 * 1.25f < multitype.t1)
                    {
                        result = spell;
                        num14 = multitype.t1;
                    }
                    else
                    {
                        num15++;
                    }
                    if (setting.value == "1")
                    {
                        return result;
                    }
                    if (setting.value == "2" && num15 >= 2)
                    {
                        return result;
                    }
                    if (setting.value == "3" && num15 >= 4)
                    {
                        return result;
                    }
                }
            }
            return result;
        }
        if (num13 < num5)
        {
            spells.RandomSort();
            Spell result2 = null;
            float num16 = 0f;
            int num17 = 0;
            for (int j = 0; j < spells.Count; j++)
            {
                Spell spell2 = spells[j].Get();
                Multitype<object, float> multitype2 = null;
                if (spell2.targetType.enumType == ETargetType.TargetUnit)
                {
                    multitype2 = this.FindEnemyUnit(spell2);
                }
                else if (spell2.targetType.enumType == ETargetType.TargetGroup)
                {
                    multitype2 = this.FindEnemyGroup(spell2);
                }
                if (multitype2 != null)
                {
                    if (num16 * 1.25f < multitype2.t1)
                    {
                        result2 = spell2;
                        num16 = multitype2.t1;
                    }
                    else
                    {
                        num17++;
                    }
                    if (setting.value == "1")
                    {
                        return result2;
                    }
                    if (setting.value == "2" && num17 >= 2)
                    {
                        return result2;
                    }
                    if (setting.value == "3" && num17 >= 4)
                    {
                        return result2;
                    }
                }
            }
            return result2;
        }
        if (num13 < num6)
        {
            spells.RandomSort();
            Spell result3 = null;
            float num18 = 0f;
            int num19 = 0;
            for (int k = 0; k < spells.Count; k++)
            {
                Spell spell3 = spells[k].Get();
                Multitype<object, float> multitype3 = null;
                if (spell3.targetType.enumType == ETargetType.TargetUnit)
                {
                    multitype3 = this.FindAllyUnit(spell3);
                }
                else if (spell3.targetType.enumType == ETargetType.TargetGroup)
                {
                    multitype3 = this.FindAllyGroup(spell3);
                }
                if (multitype3 != null)
                {
                    if (num18 * 1.25f < multitype3.t1)
                    {
                        result3 = spell3;
                        num18 = multitype3.t1;
                    }
                    else
                    {
                        num19++;
                    }
                    if (setting.value == "1")
                    {
                        return result3;
                    }
                    if (setting.value == "2" && num19 >= 2)
                    {
                        return result3;
                    }
                    if (setting.value == "3" && num19 >= 4)
                    {
                        return result3;
                    }
                }
            }
            return result3;
        }
        if (num13 < num7)
        {
            spells.RandomSort();
            Spell result4 = null;
            float num20 = 0f;
            int num21 = 0;
            for (int l = 0; l < spells.Count; l++)
            {
                Spell spell4 = spells[l].Get();
                if (spell4.targetType == null || spell4.targetType.enumType != ETargetType.TargetLocation)
                {
                    continue;
                }
                Multitype<object, float> multitype4 = this.FindEnemyLocation(spell4);
                if (multitype4 != null)
                {
                    if (num20 * 1.25f < multitype4.t1)
                    {
                        result4 = spell4;
                        num20 = multitype4.t1;
                    }
                    else
                    {
                        num21++;
                    }
                    if (setting.value == "1")
                    {
                        return result4;
                    }
                    if (setting.value == "2" && num21 >= 2)
                    {
                        return result4;
                    }
                    if (setting.value == "3" && num21 >= 4)
                    {
                        return result4;
                    }
                }
            }
            return result4;
        }
        if (num13 < num8)
        {
            spells.RandomSort();
            Spell result5 = null;
            float num22 = 0f;
            int num23 = 0;
            for (int m = 0; m < spells.Count; m++)
            {
                Spell spell5 = spells[m].Get();
                if (spell5.targetType == null || spell5.targetType.enumType != ETargetType.TargetLocation)
                {
                    continue;
                }
                Multitype<object, float> multitype5 = this.FindAllyLocation(spell5);
                if (multitype5 != null)
                {
                    if (num22 * 1.25f < multitype5.t1)
                    {
                        result5 = spell5;
                        num22 = multitype5.t1;
                    }
                    else
                    {
                        num23++;
                    }
                    if (setting.value == "1")
                    {
                        return result5;
                    }
                    if (setting.value == "2" && num23 >= 2)
                    {
                        return result5;
                    }
                    if (setting.value == "3" && num23 >= 4)
                    {
                        return result5;
                    }
                }
            }
            return result5;
        }
        if (num13 < num9)
        {
            spells.RandomSort();
            Spell result6 = null;
            float num24 = 0f;
            int num25 = 0;
            for (int n = 0; n < spells.Count; n++)
            {
                Spell spell6 = spells[n].Get();
                if (spell6.targetType == null || spell6.targetType.enumType != ETargetType.TargetWizard)
                {
                    continue;
                }
                Multitype<object, float> multitype6 = this.FindEnemyWizard(spell6);
                if (multitype6 != null)
                {
                    if (num24 * 1.25f < multitype6.t1)
                    {
                        result6 = spell6;
                        num24 = multitype6.t1;
                    }
                    else
                    {
                        num25++;
                    }
                    if (setting.value == "1")
                    {
                        return result6;
                    }
                    if (setting.value == "2" && num25 >= 2)
                    {
                        return result6;
                    }
                    if (setting.value == "3" && num25 >= 4)
                    {
                        return result6;
                    }
                }
            }
            return result6;
        }
        if (num13 < num10)
        {
            spells.RandomSort();
            Spell result7 = null;
            float num26 = 0f;
            int num27 = 0;
            for (int num28 = 0; num28 < spells.Count; num28++)
            {
                Spell spell7 = spells[num28].Get();
                if (spell7.targetType == null || spell7.targetType.enumType != ETargetType.TargetWizard)
                {
                    continue;
                }
                Multitype<object, float> multitype7 = this.FindAllyWizard(spell7);
                if (multitype7 != null)
                {
                    if (num26 * 1.25f < multitype7.t1)
                    {
                        result7 = spell7;
                        num26 = multitype7.t1;
                    }
                    else
                    {
                        num27++;
                    }
                    if (setting.value == "1")
                    {
                        return result7;
                    }
                    if (setting.value == "2" && num27 >= 2)
                    {
                        return result7;
                    }
                    if (setting.value == "3" && num27 >= 4)
                    {
                        return result7;
                    }
                }
            }
            return result7;
        }
        if (num13 < num11)
        {
            return null;
        }
        if (num13 < num12)
        {
            spells.RandomSort();
            Spell result8 = null;
            int num29 = 0;
            float num30 = 0f;
            for (int num31 = 0; num31 < spells.Count; num31++)
            {
                Spell spell8 = spells[num31].Get();
                if (spell8.targetType == null || spell8.targetType.enumType != ETargetType.TargetGlobal || string.IsNullOrEmpty(spell8.worldScript))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(spell8.aiWorldEvaluationScript))
                {
                    Debug.LogError("Missing evaluation for spell " + spell8.dbName);
                    continue;
                }
                Multitype<object, float> multitype8 = this.FindGlobalSpellValue(spell8);
                if (multitype8 != null)
                {
                    if (multitype8.t1 > num30)
                    {
                        result8 = spell8;
                        num30 = multitype8.t1;
                    }
                    num29++;
                    if (setting.value == "1")
                    {
                        return result8;
                    }
                    if (setting.value == "2" && num29 >= 2)
                    {
                        return result8;
                    }
                    if (setting.value == "3" && num29 >= 4)
                    {
                        return result8;
                    }
                }
            }
            return result8;
        }
        return null;
    }

    public bool SummonFilter(Spell s)
    {
        if (s.stringData == null || s.stringData.Length < 1)
        {
            return false;
        }
        if (TurnManager.GetTurnNumber() > 150 && s.rarity == ERarity.Common)
        {
            return false;
        }
        Subrace subrace = DataBase.Get<Subrace>(s.stringData[0], reportMissing: false);
        if (subrace == null)
        {
            return false;
        }
        if (subrace.skills != null)
        {
            if (Array.Find(subrace.skills, (Skill o) => o.dbName.Contains("MELD")) != null)
            {
                return false;
            }
            if (Array.Find(subrace.skills, (Skill o) => o.dbName.Contains("TRANSPORTER")) != null)
            {
                return false;
            }
        }
        return true;
    }

    private TownLocation GetSummoningCircleTarget()
    {
        if (this.owner?.Get() != null)
        {
            if (this.owner.Get().warEfforts != null && this.owner.Get().warEfforts.Count > 0 && this.owner.Get().warEfforts[0].mostEngagedRegion != null)
            {
                SingleRegion mostEngagedRegion = this.owner.Get().warEfforts[0].mostEngagedRegion;
                if (mostEngagedRegion.locations != null)
                {
                    TownLocation townLocation = mostEngagedRegion.locations.BestAt((TownLocation a, TownLocation b) => -a.Population.CompareTo(b.Population));
                    if (townLocation != null)
                    {
                        return townLocation;
                    }
                }
            }
            TownLocation townLocation2 = null;
            {
                foreach (global::MOM.Group ownGroup in this.owner.Get().GetOwnGroups())
                {
                    if (ownGroup.IsHosted() && ownGroup.GetLocationHost() is TownLocation townLocation3 && townLocation3.locationTactic != null && townLocation3.locationTactic.dangerRank > 0 && (townLocation2 == null || townLocation2.Population < townLocation3.Population))
                    {
                        townLocation2 = townLocation3;
                    }
                }
                return townLocation2;
            }
        }
        return null;
    }

    private float NeedOfSummon(List<DBReference<Spell>> spells)
    {
        PlayerWizardAI playerWizardAI = this.owner.Get();
        Reference<TownLocation> summoningCircle = playerWizardAI.summoningCircle;
        if (summoningCircle?.Get().locationTactic == null)
        {
            return 0f;
        }
        AILocationTactic locationTactic = summoningCircle.Get().locationTactic;
        bool flag = false;
        if (spells != null)
        {
            ERarity eRarity = ERarity.Uncommon;
            foreach (DBReference<Spell> spell in spells)
            {
                if (spell.Get().summonFantasticUnitSpell && spell.Get().rarity >= eRarity)
                {
                    flag = true;
                    break;
                }
            }
        }
        int num = locationTactic.needToBuildArmy;
        List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(summoningCircle.Get().GetPlane());
        V3iRect area = summoningCircle.Get().GetPlane().area;
        foreach (global::MOM.Location item in locationsOfThePlane)
        {
            if (item is TownLocation && item.GetOwnerID() == playerWizardAI.ID && area.HexDistance(item.GetPosition(), summoningCircle.Get().GetPosition()) < 15)
            {
                int b = ((item.locationTactic != null) ? item.locationTactic.needToBuildArmy : 0);
                num = Mathf.Max(num, b);
            }
        }
        float num2 = (float)num / 3f;
        if (flag)
        {
            num2 += 0.1f;
        }
        if (this.increasedWillOfSummon)
        {
            num2 += 0.3f;
        }
        return num2;
    }

    private float NeedOfLocationCurse()
    {
        float num = 0.15f;
        if (this.owner.Get().GetAttFinal((Tag)TAG.CHAOS_MAGIC_BOOK) > 0)
        {
            num += 0.1f;
        }
        if (this.owner.Get().GetAttFinal((Tag)TAG.DEATH_MAGIC_BOOK) > 0)
        {
            num += 0.05f;
        }
        if (this.owner.Get().GetPersonality() == (Personality)PERSONALITY.LAWFUL)
        {
            num -= 0.05f;
        }
        return global::UnityEngine.Random.Range(0f, num);
    }

    private float NeedOfLocationBless()
    {
        float num = 0.15f;
        if (this.owner.Get().GetAttFinal((Tag)TAG.LIFE_MAGIC_BOOK) > 0)
        {
            num += 0.1f;
        }
        if (this.owner.Get().GetAttFinal((Tag)TAG.NATURE_MAGIC_BOOK) > 0)
        {
            num += 0.05f;
        }
        if (this.owner.Get().GetPersonality() == (Personality)PERSONALITY.MANIACAL)
        {
            num -= 0.05f;
        }
        return global::UnityEngine.Random.Range(0f, num);
    }

    private float NeedOfUnitCurse()
    {
        float num = 0.25f;
        if (this.owner.Get().GetAttFinal((Tag)TAG.CHAOS_MAGIC_BOOK) > 0)
        {
            num += 0.2f;
        }
        if (this.owner.Get().GetAttFinal((Tag)TAG.DEATH_MAGIC_BOOK) > 0)
        {
            num += 0.1f;
        }
        if (this.owner.Get().GetPersonality() == (Personality)PERSONALITY.LAWFUL)
        {
            num -= 0.1f;
        }
        return global::UnityEngine.Random.Range(0f, num);
    }

    private float NeedOfUnitBless()
    {
        float num = 0.25f;
        if (this.owner.Get().GetAttFinal((Tag)TAG.LIFE_MAGIC_BOOK) > 0)
        {
            num += 0.2f;
        }
        if (this.owner.Get().GetAttFinal((Tag)TAG.NATURE_MAGIC_BOOK) > 0)
        {
            num += 0.1f;
        }
        if (this.owner.Get().GetPersonality() == (Personality)PERSONALITY.MANIACAL)
        {
            num -= 0.1f;
        }
        return global::UnityEngine.Random.Range(0f, num);
    }

    private float NeedOfWizardCurse()
    {
        float num = 0.1f;
        if (this.owner.Get().GetAttFinal((Tag)TAG.CHAOS_MAGIC_BOOK) > 0)
        {
            num += 0.1f;
        }
        if (this.owner.Get().GetAttFinal((Tag)TAG.DEATH_MAGIC_BOOK) > 0)
        {
            num += 0.05f;
        }
        if (this.owner.Get().GetPersonality() == (Personality)PERSONALITY.LAWFUL)
        {
            num -= 0.05f;
        }
        return global::UnityEngine.Random.Range(0f, num);
    }

    private float NeedOfWizardBless()
    {
        float num = 0.1f;
        if (this.owner.Get().GetAttFinal((Tag)TAG.LIFE_MAGIC_BOOK) > 0)
        {
            num += 0.1f;
        }
        if (this.owner.Get().GetAttFinal((Tag)TAG.NATURE_MAGIC_BOOK) > 0)
        {
            num += 0.05f;
        }
        if (this.owner.Get().GetPersonality() == (Personality)PERSONALITY.MANIACAL)
        {
            num -= 0.05f;
        }
        return global::UnityEngine.Random.Range(0f, num);
    }

    private float NeedOfTerrainTransform()
    {
        return global::UnityEngine.Random.Range(0f, 0.05f);
    }

    private float NeedOfGlobalEffect()
    {
        return global::UnityEngine.Random.Range(0f, 0.05f);
    }

    private bool CounterMagicWorld(Spell s)
    {
        if ((bool)ScriptLibrary.Call("CounterMagicWorld", s, this.owner.Get()))
        {
            return true;
        }
        return false;
    }

    private void PrepareManaForScaledSpells(Spell s)
    {
        if (s.changeableCost != null)
        {
            PlayerWizardAI playerWizardAI = this.owner.Get();
            int worldCastingCost = s.GetWorldCastingCost(playerWizardAI);
            int num = s.changeableCost.maxMultipier * worldCastingCost;
            if (worldCastingCost > 0)
            {
                int num2 = Mathf.Min(playerWizardAI.mana - num, num);
                int extraPower = Mathf.RoundToInt((float)(s.changeableCost.maxMultipier * s.worldCost / s.changeableCost.costPerPoint * num2) / (float)num);
                playerWizardAI.GetMagicAndResearch().extensionItemSpellWorld.extraPower = extraPower;
                playerWizardAI.GetMagicAndResearch().extensionItemSpellWorld.extraMana = num2;
            }
        }
    }

    private void UseManaForScaledSpells()
    {
        PlayerWizardAI playerWizardAI = this.owner.Get();
        int extraMana = playerWizardAI.GetMagicAndResearch().extensionItemSpellWorld.extraMana;
        playerWizardAI.mana = Mathf.Max(0, playerWizardAI.mana - extraMana);
    }

    private void ClearManaForScaledSpells()
    {
        this.owner.Get().GetMagicAndResearch().extensionItemSpellWorld.Clear();
    }
}
