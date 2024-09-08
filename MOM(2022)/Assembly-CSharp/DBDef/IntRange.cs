using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("INT_RANGE", "")]
    public class IntRange : DBClass
    {
        public static string abbreviation = "";

        [Prototype("MinimumCount", true)]
        public int minimumCount;

        [Prototype("MaximumCount", true)]
        public int maximumCount;

        public static explicit operator IntRange(Enum e)
        {
            return DataBase.Get<IntRange>(e);
        }

        public static explicit operator IntRange(string e)
        {
            return DataBase.Get<IntRange>(e, reportMissing: true);
        }
    }
}
