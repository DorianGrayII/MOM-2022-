namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("RACE_TENSION", "")]
    public class RaceTension : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Race", true)]
        public Race race;
        [Prototype("Value", true)]
        public FInt value;

        public static explicit operator RaceTension(Enum e)
        {
            return DataBase.Get<RaceTension>(e, false);
        }

        public static explicit operator RaceTension(string e)
        {
            return DataBase.Get<RaceTension>(e, true);
        }
    }
}

