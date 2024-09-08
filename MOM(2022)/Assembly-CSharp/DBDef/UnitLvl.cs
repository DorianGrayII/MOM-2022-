using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("UNIT_LVL", "")]
    public class UnitLvl : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("Level", true)]
        public int level;

        [Prototype("UnitClass", true)]
        public Tag unitClass;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator UnitLvl(Enum e)
        {
            return DataBase.Get<UnitLvl>(e);
        }

        public static explicit operator UnitLvl(string e)
        {
            return DataBase.Get<UnitLvl>(e, reportMissing: true);
        }
    }
}
