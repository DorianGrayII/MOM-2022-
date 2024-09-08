using System;
using MHUtils;

namespace DBDef
{
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
            return DataBase.Get<CountedTag>(e);
        }

        public static explicit operator CountedTag(string e)
        {
            return DataBase.Get<CountedTag>(e, reportMissing: true);
        }
    }
}
