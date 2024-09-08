using System;
using MHUtils;

namespace DBDef
{
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
            return DataBase.Get<TownPopulation>(e);
        }

        public static explicit operator TownPopulation(string e)
        {
            return DataBase.Get<TownPopulation>(e, reportMissing: true);
        }
    }
}
