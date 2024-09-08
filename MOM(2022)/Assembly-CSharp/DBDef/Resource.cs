namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("RESOURCE", "RES")]
    public class Resource : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "RES";
        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;
        [Prototype("OptionalModel3dName", false)]
        public string model3d;
        [Prototype("TransmuteTo", false)]
        public Resource transmuteTo;
        [Prototype("BonusTypes", false)]
        public ResourceBonusType bonusTypes;
        [Prototype("OutpostGrowth", false)]
        public int outpostGrowth;
        [Prototype("Dlc", false)]
        public string dlc;
        [Prototype("Mineral", false)]
        public bool mineral;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Resource(Enum e)
        {
            return DataBase.Get<Resource>(e, false);
        }

        public static explicit operator Resource(string e)
        {
            return DataBase.Get<Resource>(e, true);
        }
    }
}

