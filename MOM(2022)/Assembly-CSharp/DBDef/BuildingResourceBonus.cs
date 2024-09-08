namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("BUILDING_RESOURCE_BONUS", "")]
    public class BuildingResourceBonus : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Resource", true)]
        public Resource resource;
        [Prototype("BonusTypes", true)]
        public ResourceBonusType bonusTypes;

        public static explicit operator BuildingResourceBonus(Enum e)
        {
            return DataBase.Get<BuildingResourceBonus>(e, false);
        }

        public static explicit operator BuildingResourceBonus(string e)
        {
            return DataBase.Get<BuildingResourceBonus>(e, true);
        }
    }
}

