namespace DBDef
{
    using MHUtils;
    using System;

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
            return DataBase.Get<FIntRange>(e, false);
        }

        public static explicit operator FIntRange(string e)
        {
            return DataBase.Get<FIntRange>(e, true);
        }
    }
}

