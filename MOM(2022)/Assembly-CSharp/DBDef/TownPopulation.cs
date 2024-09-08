namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("TOWN_POPULATION", "")]
    public class TownPopulation : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Graphic", false)]
        public string graphic;
        [Prototype("Production", false)]
        public FInt production;
        [Prototype("Farmer", false)]
        public FInt farmer;
        [Prototype("PowerProduction", false)]
        public FInt powerProduction;

        public static explicit operator TownPopulation(Enum e)
        {
            return DataBase.Get<TownPopulation>(e, false);
        }

        public static explicit operator TownPopulation(string e)
        {
            return DataBase.Get<TownPopulation>(e, true);
        }
    }
}

