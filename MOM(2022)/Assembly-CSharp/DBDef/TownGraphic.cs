namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("TOWN_GRAPHIC", "")]
    public class TownGraphic : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Outpost", true)]
        public string outpost;
        [Prototype("Settlement", true)]
        public string settlement;
        [Prototype("Hamlet", true)]
        public string hamlet;
        [Prototype("Village", true)]
        public string village;
        [Prototype("Town", true)]
        public string town;
        [Prototype("City", true)]
        public string city;

        public static explicit operator TownGraphic(Enum e)
        {
            return DataBase.Get<TownGraphic>(e, false);
        }

        public static explicit operator TownGraphic(string e)
        {
            return DataBase.Get<TownGraphic>(e, true);
        }
    }
}

