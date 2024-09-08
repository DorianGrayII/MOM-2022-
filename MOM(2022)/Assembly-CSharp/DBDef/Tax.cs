namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("TAX", "")]
    public class Tax : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Income", true)]
        public FInt income;
        [Prototype("Rebelion", true)]
        public FInt rebelion;

        public static explicit operator Tax(Enum e)
        {
            return DataBase.Get<Tax>(e, false);
        }

        public static explicit operator Tax(string e)
        {
            return DataBase.Get<Tax>(e, true);
        }
    }
}

