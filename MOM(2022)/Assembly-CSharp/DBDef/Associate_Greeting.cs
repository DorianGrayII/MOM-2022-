namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("ASSOCIATE_GREETING", "")]
    public class Associate_Greeting : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Wizard", true)]
        public Wizard wizard;
        [Prototype("Greeting", false)]
        public string greeting;

        public static explicit operator Associate_Greeting(Enum e)
        {
            return DataBase.Get<Associate_Greeting>(e, false);
        }

        public static explicit operator Associate_Greeting(string e)
        {
            return DataBase.Get<Associate_Greeting>(e, true);
        }
    }
}

