using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("RESOURCE_BONUS_TYPE", "")]
    public class ResourceBonusType : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Money", true)]
        public FInt money;

        [Prototype("Power", true)]
        public FInt power;

        [Prototype("Food", true)]
        public FInt food;

        [Prototype("UnitProductionCost", true)]
        public FInt unitProductionCost;

        [Prototype("BuildingProductionCost", true)]
        public FInt buildingProductionCost;

        public static explicit operator ResourceBonusType(Enum e)
        {
            return DataBase.Get<ResourceBonusType>(e);
        }

        public static explicit operator ResourceBonusType(string e)
        {
            return DataBase.Get<ResourceBonusType>(e, reportMissing: true);
        }
    }
}
