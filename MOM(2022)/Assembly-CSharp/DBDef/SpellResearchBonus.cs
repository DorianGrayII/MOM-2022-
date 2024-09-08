using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("SPELL_RESEARCH_BONUS", "")]
    public class SpellResearchBonus : DBClass
    {
        public static string abbreviation = "";

        [Prototype("CastingDiscount", false)]
        public FInt castingDiscount;

        [Prototype("ResearchDiscount", false)]
        public FInt researchDiscount;

        public static explicit operator SpellResearchBonus(Enum e)
        {
            return DataBase.Get<SpellResearchBonus>(e);
        }

        public static explicit operator SpellResearchBonus(string e)
        {
            return DataBase.Get<SpellResearchBonus>(e, reportMissing: true);
        }
    }
}
