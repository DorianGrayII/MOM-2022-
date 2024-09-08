namespace DBDef
{
    using MHUtils;
    using System;

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
            return DataBase.Get<SpellResearchBonus>(e, false);
        }

        public static explicit operator SpellResearchBonus(string e)
        {
            return DataBase.Get<SpellResearchBonus>(e, true);
        }
    }
}

