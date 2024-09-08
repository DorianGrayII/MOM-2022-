using System.Collections.Generic;
using DBDef;
using MOM;
using UnityEngine;

public static class ISpellCasterExtension
{
    public static void AddSpell(this ISpellCaster obj, Spell s)
    {
        obj.GetSpellManager().Add(s);
        if (obj is PlayerWizard)
        {
            (obj as PlayerWizard).GetMagicAndResearch().RemoveFromCurrentResearchOptions(s);
        }
    }

    public static void RemoveSpell(this ISpellCaster obj, Spell s)
    {
        obj.GetSpellManager().Remove(s);
    }

    public static List<DBReference<Spell>> GetSpells(this ISpellCaster obj)
    {
        return obj.GetSpellManager().GetSpells();
    }

    public static List<Spell> GetSpellsConverted(this ISpellCaster obj)
    {
        List<Spell> spells = new List<Spell>(obj.GetSpells().Count);
        obj.GetSpells().ForEach(delegate(DBReference<Spell> o)
        {
            spells.Add(o.Get());
        });
        return spells;
    }

    public static SpellManager CopySpellManager(this ISpellCaster obj, ISpellCaster newOwner)
    {
        return obj.GetSpellManager().CopySpellManager(newOwner);
    }

    public static bool TargetForSpellValid(this ISpellCaster obj, Battle battle, object chosenTarget, Spell spell)
    {
        if (!string.IsNullOrEmpty(spell.targetingScript))
        {
            return (bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(obj, battle), chosenTarget, spell);
        }
        return true;
    }

    public static bool IsAttackerInBattle(this ISpellCaster obj, Battle b)
    {
        if (obj == null || b == null)
        {
            return false;
        }
        if (obj is PlayerWizard)
        {
            return b.attacker.GetWizardOwner() == obj;
        }
        if (obj is BattlePlayer)
        {
            return b.attacker == obj;
        }
        if (obj is BattleUnit)
        {
            if (b.attackerUnits != null)
            {
                return b.attackerUnits.Contains(obj as BattleUnit);
            }
            return false;
        }
        Debug.LogError("Not Implemented type of " + obj);
        return false;
    }
}
