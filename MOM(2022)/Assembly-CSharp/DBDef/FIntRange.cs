using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("FINT_RANGE", "")]
    public class FIntRange : DBClass
    {
        public static string abbreviation = "";

        [Prototype("MinimumCount", true)]
        public FInt minimumCount;

        [Prototype("MaximumCount", true)]
        public FInt maximumCount;

        public static explicit operator FIntRange(Enum e)
        {
            return DataBase.Get<FIntRange>(e);
        }

        public static explicit operator FIntRange(string e)
        {
            return DataBase.Get<FIntRange>(e, reportMissing: true);
        }
    }
}
