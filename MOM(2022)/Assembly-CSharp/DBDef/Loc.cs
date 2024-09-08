namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("LOC", "")]
    public class Loc : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Key", true)]
        public string key;
        [Prototype("Value", true)]
        public string value;

        public static explicit operator Loc(Enum e)
        {
            return DataBase.Get<Loc>(e, false);
        }

        public static explicit operator Loc(string e)
        {
            return DataBase.Get<Loc>(e, true);
        }
    }
}

