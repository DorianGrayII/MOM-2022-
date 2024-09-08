namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("SPELL_PACK", "")]
    public class SpellPack : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Spell", false)]
        public Spell[] spells;

        public static explicit operator SpellPack(Enum e)
        {
            return DataBase.Get<SpellPack>(e, false);
        }

        public static explicit operator SpellPack(string e)
        {
            return DataBase.Get<SpellPack>(e, true);
        }

        public void Set_spells(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.spells = new Spell[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Spell))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("spells of type Spell received invalid type from array! " + text1);
                    }
                    this.spells[i] = list[i] as Spell;
                }
            }
        }
    }
}

