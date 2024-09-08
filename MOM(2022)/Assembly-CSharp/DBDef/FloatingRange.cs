using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("FLOATING_RANGE", "")]
    public class FloatingRange : DBClass
    {
        public static string abbreviation = "";

        [Prototype("MinimumCount", true)]
        public float minimumCount;

        [Prototype("MaximumCount", true)]
        public float maximumCount;

        public static explicit operator FloatingRange(Enum e)
        {
            return DataBase.Get<FloatingRange>(e);
        }

        public static explicit operator FloatingRange(string e)
        {
            return DataBase.Get<FloatingRange>(e, reportMissing: true);
        }
    }
}
