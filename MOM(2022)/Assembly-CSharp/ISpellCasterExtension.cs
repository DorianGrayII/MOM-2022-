using DBDef;
using MOM;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Extension]
public static class ISpellCasterExtension
{
    [Extension]
    public static void AddSpell(ISpellCaster obj, Spell s)
    {
        obj.GetSpellManager().Add(s);
        if (obj is PlayerWizard)
        {
            (obj as PlayerWizard).GetMagicAndResearch().RemoveFromCurrentResearchOptions(s);
        }
    }

    [Extension]
    public static SpellManager CopySpellManager(ISpellCaster obj, ISpellCaster newOwner)
    {
        return obj.GetSpellManager().CopySpellManager(newOwner);
    }

    [Extension]
    public static List<DBReference<Spell>> GetSpells(ISpellCaster obj)
    {
        return obj.GetSpellManager().GetSpells();
    }

    [Extension]
    public static List<Spell> GetSpellsConverted(ISpellCaster obj)
    {
        List<Spell> spells = new List<Spell>(GetSpells(obj).Count);
        GetSpells(obj).ForEach(delegate (DBReference<Spell> o) {
            spells.Add(o.Get());
        });
        return spells;
    }

    [Extension]
    public static bool IsAttackerInBattle(ISpellCaster obj, Battle b)
    {
        if ((obj != null) && (b != null))
        {
            string text1;
            if (obj is PlayerWizard)
            {
                return ReferenceEquals(b.attacker.GetWizardOwner(), obj);
            }
            if (obj is BattlePlayer)
            {
                return ReferenceEquals(b.attacker, obj);
            }
            if (obj is BattleUnit)
            {
                return ((b.attackerUnits != null) && b.attackerUnits.Contains(obj as BattleUnit));
            }
            if (obj != null)
            {
                text1 = obj.ToString();
            }
            else
            {
                ISpellCaster local1 = obj;
                text1 = null;
            }
            Debug.LogError("Not Implemented type of " + text1);
        }
        return false;
    }

    [Extension]
    public static void RemoveSpell(ISpellCaster obj, Spell s)
    {
        obj.GetSpellManager().Remove(s);
    }

    [Extension]
    public static bool TargetForSpellValid(ISpellCaster obj, Battle battle, object chosenTarget, Spell spell)
    {
        if (string.IsNullOrEmpty(spell.targetingScript))
        {
            return true;
        }
        object[] parameters = new object[] { new SpellCastData(obj, battle), chosenTarget, spell };
        return (bool) ScriptLibrary.Call(spell.targetingScript, parameters);
    }
}

