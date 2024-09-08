namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("COUNTED_TAG", "")]
    public class CountedTag : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Amount", true)]
        public FInt amount;
        [Prototype("Tag", true)]
        public Tag tag;

        public static explicit operator CountedTag(Enum e)
        {
            return DataBase.Get<CountedTag>(e, false);
        }

        public static explicit operator CountedTag(string e)
        {
            return DataBase.Get<CountedTag>(e, true);
        }
    }
}

