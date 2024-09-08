namespace DBDef
{
    using MHUtils;
    using System;

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
            return DataBase.Get<IntRange>(e, false);
        }

        public static explicit operator IntRange(string e)
        {
            return DataBase.Get<IntRange>(e, true);
        }
    }
}

