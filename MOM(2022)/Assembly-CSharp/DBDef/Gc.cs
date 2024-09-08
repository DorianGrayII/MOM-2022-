namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("GC", "")]
    public class Gc : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Name", true)]
        public string name;
        [Prototype("Setting", true)]
        public string setting;

        public static explicit operator Gc(Enum e)
        {
            return DataBase.Get<Gc>(e, false);
        }

        public static explicit operator Gc(string e)
        {
            return DataBase.Get<Gc>(e, true);
        }
    }
}

