using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("SPELLS_SECTION", "")]
    public class SpellsSection : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Rarity", true)]
        public ERarity rarity;

        [Prototype("Count", true)]
        public int count;

        public static explicit operator SpellsSection(Enum e)
        {
            return DataBase.Get<SpellsSection>(e);
        }

        public static explicit operator SpellsSection(string e)
        {
            return DataBase.Get<SpellsSection>(e, reportMissing: true);
        }
    }
}
