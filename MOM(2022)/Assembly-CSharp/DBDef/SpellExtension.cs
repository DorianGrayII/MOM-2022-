// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.SpellExtension
using System;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MOM;
using UnityEngine;

public static class SpellExtension
{
    public static int GetCastingTurnsInt(this Spell spell, PlayerWizard wizard, bool includeExtraManaCost = true)
    {
        int num = wizard.CalculateManaIncome(includeUpkeep: true);
        int castingProgress = wizard.GetMagicAndResearch().castingProgress;
        int worldCastingCost = spell.GetWorldCastingCost(wizard, includeExtraManaCost);
        if (worldCastingCost < 0)
        {
            return worldCastingCost;
        }
        int num2 = Mathf.Min(wizard.mana, wizard.turnSkillLeft);
        int num3 = wizard.mana - num2;
        worldCastingCost -= castingProgress;
        worldCastingCost -= num2;
        if (worldCastingCost <= 0)
        {
            return 0;
        }
        int totalCastingSkill = wizard.GetTotalCastingSkill();
        int num4 = (worldCastingCost + totalCastingSkill - 1) / totalCastingSkill;
        int num5 = Math.Max(0, totalCastingSkill - num);
        if (num5 > 0)
        {
            int num6 = num3 / num5;
            int num7 = num3 % num5;
            if (num4 > num6)
            {
                num4 = num6 + 1;
                worldCastingCost -= num6 * totalCastingSkill;
                worldCastingCost -= num7 + num;
                if (num <= 0 && worldCastingCost > 0)
                {
                    return 1000;
                }
                if (worldCastingCost > 0)
                {
                    num4 += (worldCastingCost + num - 1) / num;
                }
            }
        }
        return num4;
    }

    public static string GetCastingTurns(this Spell spell, PlayerWizard wizard, bool includeExtraManaCost = true)
    {
        int castingTurnsInt = spell.GetCastingTurnsInt(wizard, includeExtraManaCost);
        if (castingTurnsInt < 0)
        {
            return global::DBUtils.Localization.Get("UI_SPECIAL", true);
        }
        if (castingTurnsInt > 999)
        {
            return "∞";
        }
        if (castingTurnsInt > 99)
        {
            return "99+";
        }
        return castingTurnsInt.ToString();
    }

    public static int GetUpkeepCost(this Spell s)
    {
        int num = 0;
        if (s.summonFantasticUnitSpell && s.stringData != null && !string.IsNullOrEmpty(s.stringData[0]))
        {
            DBClass dBClass = DataBase.Get(s.stringData[0], reportMissing: false);
            if (dBClass != null && dBClass is Subrace && dBClass is global::DBDef.Unit unit)
            {
                CountedTag countedTag = Array.Find(unit.tags, (CountedTag o) => o.tag == (Tag)TAG.UPKEEP_MANA);
                if (countedTag != null)
                {
                    num += countedTag.amount.ToInt();
                }
            }
        }
        if (s.enchantmentData == null || s.enchantmentData.Length < 1)
        {
            return 0;
        }
        Enchantment[] enchantmentData = s.enchantmentData;
        foreach (Enchantment enchantment in enchantmentData)
        {
            num += enchantment.upkeepCost;
        }
        return num;
    }

    public static int GetWorldCastingCost(this Spell s, ISpellCaster w, bool includeExtraManaCost = false)
    {
        ERealm realm = s.realm;
        Tag tagForRealm = MagicAndResearch.GetTagForRealm(realm);
        int num = s.worldCost;
        if (s == (Spell)SPELL.ENCHANT_ITEM || s == (Spell)SPELL.CREATE_ARTEFACT)
        {
            num = w.GetWizardOwner().GetMagicAndResearch().craftItemSpell?.cost ?? (-1);
        }
        else if (tagForRealm != null && w is PlayerWizard)
        {
            PlayerWizard playerWizard = w as PlayerWizard;
            int num2 = playerWizard.GetAttributes().GetFinal(tagForRealm).ToInt();
            if (num2 > 0)
            {
                BooksAdvantage booksAdvantage = MagicAndResearch.GetBooksAdvantage(realm, num2);
                float num3 = 0f;
                num3 = ((booksAdvantage == null || booksAdvantage.bonus == null) ? 0f : booksAdvantage.bonus.castingDiscount.ToFloat());
                if (booksAdvantage != null && s.summonFantasticUnitSpell && playerWizard.lowerFantasticUnitsPercentSummonCost > FInt.ZERO)
                {
                    float num4 = Mathf.Max(1f - num3 - playerWizard.GetCastCostPercent(s).ToFloat() - playerWizard.lowerFantasticUnitsPercentSummonCost.ToFloat(), 0f);
                    num = Mathf.RoundToInt((float)num * num4);
                }
                else if (booksAdvantage != null)
                {
                    float num5 = Mathf.Max(1f - num3 - playerWizard.GetCastCostPercent(s).ToFloat(), 0f);
                    num = Mathf.RoundToInt((float)num * num5);
                }
            }
            else if (s.summonFantasticUnitSpell && playerWizard.lowerFantasticUnitsPercentSummonCost > FInt.ZERO)
            {
                float num6 = Mathf.Max(1f - playerWizard.GetCastCostPercent(s).ToFloat() - playerWizard.lowerFantasticUnitsPercentSummonCost.ToFloat(), 0f);
                num = Mathf.RoundToInt((float)num * num6);
            }
            else
            {
                num = Mathf.RoundToInt((float)num * (1f - playerWizard.GetCastCostPercent(s).ToFloat()));
            }
            if (includeExtraManaCost && playerWizard.GetMagicAndResearch().extensionItemSpellWorld != null)
            {
                num += playerWizard.GetMagicAndResearch().extensionItemSpellWorld.extraMana;
            }
        }
        return num;
    }

    public static int GetBattleCastingCost(this Spell s, ISpellCaster w, bool includeExtraManaCost = false)
    {
        ERealm realm = s.realm;
        Tag tagForRealm = MagicAndResearch.GetTagForRealm(realm);
        int num = s.battleCost;
        if (tagForRealm != null && w is PlayerWizard)
        {
            PlayerWizard playerWizard = w as PlayerWizard;
            int num2 = playerWizard.GetAttributes().GetFinal(tagForRealm).ToInt();
            if (num2 > 0)
            {
                BooksAdvantage booksAdvantage = MagicAndResearch.GetBooksAdvantage(realm, num2);
                float num3 = 0f;
                num3 = ((booksAdvantage == null || booksAdvantage.bonus == null) ? 0f : booksAdvantage.bonus.castingDiscount.ToFloat());
                if (booksAdvantage != null && s.summonFantasticUnitSpell && playerWizard.lowerFantasticUnitsPercentSummonCost > FInt.ZERO)
                {
                    float num4 = Mathf.Max(1f - num3 - playerWizard.GetCastCostPercent(s).ToFloat() - playerWizard.lowerFantasticUnitsPercentSummonCost.ToFloat(), 0f);
                    num = Mathf.RoundToInt((float)num * num4);
                }
                else if (booksAdvantage != null)
                {
                    float num5 = Mathf.Max(1f - num3 - playerWizard.GetCastCostPercent(s).ToFloat(), 0f);
                    num = Mathf.RoundToInt((float)num * num5);
                }
            }
            else if (s.summonFantasticUnitSpell && playerWizard.lowerFantasticUnitsPercentSummonCost > FInt.ZERO)
            {
                float num6 = Mathf.Max(1f - playerWizard.GetCastCostPercent(s).ToFloat() - playerWizard.lowerFantasticUnitsPercentSummonCost.ToFloat(), 0f);
                num = Mathf.RoundToInt((float)num * num6);
            }
            else
            {
                num = Mathf.RoundToInt((float)num * (1f - playerWizard.GetCastCostPercent(s).ToFloat()));
            }
        }
        if (includeExtraManaCost && w is PlayerWizard && (w as PlayerWizard).GetMagicAndResearch().extensionItemSpellBattle != null)
        {
            ExtensionItemSpell extensionItemSpellBattle = (w as PlayerWizard).GetMagicAndResearch().extensionItemSpellBattle;
            num = ((extensionItemSpellBattle.extraSkill <= 0) ? (num + extensionItemSpellBattle.extraMana) : (num + extensionItemSpellBattle.extraSkill));
        }
        if ((w is global::MOM.Unit || w is BattleUnit) && s.unitBattleCost != 0)
        {
            num = s.unitBattleCost;
        }
        return num;
    }

    public static int GetResearchCost(this Spell s, ISpellCaster caster)
    {
        if (!(caster is PlayerWizard))
        {
            return 0;
        }
        PlayerWizard playerWizard = caster as PlayerWizard;
        int num = s.researchCost;
        ERealm realm = s.realm;
        Tag tagForRealm = MagicAndResearch.GetTagForRealm(realm);
        if (tagForRealm != null)
        {
            int num2 = playerWizard.GetAttributes().GetFinal(tagForRealm).ToInt();
            if (num2 > 0)
            {
                BooksAdvantage booksAdvantage = MagicAndResearch.GetBooksAdvantage(realm, num2);
                if (booksAdvantage != null && s.summonFantasticUnitSpell && playerWizard.lowerResearchFantasticUnitsPercentCost > FInt.ZERO)
                {
                    float num3 = 0f;
                    num3 = ((booksAdvantage.bonus == null) ? Mathf.Max(1f - playerWizard.researchDiscontPercent[realm].ToFloat() - playerWizard.lowerResearchFantasticUnitsPercentCost.ToFloat(), 0f) : Mathf.Max(1f - booksAdvantage.bonus.researchDiscount.ToFloat() - playerWizard.researchDiscontPercent[realm].ToFloat() - playerWizard.lowerResearchFantasticUnitsPercentCost.ToFloat(), 0f));
                    num = Mathf.Max(1, Mathf.RoundToInt((float)num * num3));
                }
                else if (booksAdvantage != null)
                {
                    float num4 = 0f;
                    num4 = ((booksAdvantage.bonus == null) ? Mathf.Max(1f - playerWizard.researchDiscontPercent[realm].ToFloat(), 0f) : Mathf.Max(1f - booksAdvantage.bonus.researchDiscount.ToFloat() - playerWizard.researchDiscontPercent[realm].ToFloat(), 0f));
                    num = Mathf.Max(1, Mathf.RoundToInt((float)num * num4));
                }
            }
            else
            {
                num = Mathf.RoundToInt((float)num * (1f - playerWizard.researchDiscontPercent[realm].ToFloat()));
                if (s == (Spell)SPELL.SPELL_OF_MASTERY)
                {
                    int num5 = 0;
                    foreach (DBReference<Spell> spell in playerWizard.GetSpells())
                    {
                        num5 += spell.Get().researchCost;
                    }
                    num += playerWizard.startingSpellsResearchCost;
                    num -= num5;
                }
            }
        }
        return num;
    }

    public static int GetBattleCastingCostByDistance(this Spell s, ISpellCaster caster, bool includeExtraManaCost = false)
    {
        if (Battle.Get() == null)
        {
            return s.GetBattleCastingCost(caster);
        }
        Vector3i pos = Vector3i.zero;
        if (Battle.Get().gDefender != null)
        {
            pos = Battle.Get().gDefender.GetPosition();
        }
        else if (Battle.Get().gAttacker != null)
        {
            pos = Battle.Get().gAttacker.GetPosition();
        }
        bool battleOnArcanus = Battle.Get().battleOnArcanus;
        int num = s.GetBattleCastingCostByDistance(caster, pos, battleOnArcanus);
        if (includeExtraManaCost && caster is PlayerWizard && (caster as PlayerWizard).GetMagicAndResearch().extensionItemSpellBattle != null)
        {
            num += (caster as PlayerWizard).GetMagicAndResearch().extensionItemSpellBattle.extraMana;
        }
        return num;
    }

    public static int GetBattleCastingCostByDistance(this Spell s, ISpellCaster caster, Vector3i pos, bool arcanus)
    {
        int battleCastingCost = s.GetBattleCastingCost(caster);
        if (battleCastingCost > 0 && caster is PlayerWizard)
        {
            PlayerWizard playerWizard = caster as PlayerWizard;
            return (int)((float)battleCastingCost * playerWizard.GetDistanceCost(arcanus, pos));
        }
        return battleCastingCost;
    }

    public static int GetSpellTacticalValue(this Spell s, int manaCost, int tacticalGain)
    {
        if (manaCost < 1)
        {
            return tacticalGain * tacticalGain;
        }
        return (int)((float)(tacticalGain * tacticalGain) / (float)manaCost);
    }
}
