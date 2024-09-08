using System;
using MHUtils;

namespace DBDef
{
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
            return DataBase.Get<Tax>(e);
        }

        public static explicit operator Tax(string e)
        {
            return DataBase.Get<Tax>(e, reportMissing: true);
        }
    }
}
