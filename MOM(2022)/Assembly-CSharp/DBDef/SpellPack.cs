using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("SPELL_PACK", "")]
    public class SpellPack : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Spell", false)]
        public Spell[] spells;

        public static explicit operator SpellPack(Enum e)
        {
            return DataBase.Get<SpellPack>(e);
        }

        public static explicit operator SpellPack(string e)
        {
            return DataBase.Get<SpellPack>(e, reportMissing: true);
        }

        public void Set_spells(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.spells = new Spell[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Spell))
                {
                    Debug.LogError("spells of type Spell received invalid type from array! " + list[i]);
                }
                this.spells[i] = list[i] as Spell;
            }
        }
    }
}
