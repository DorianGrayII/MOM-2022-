using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("RELATIONSHIP", "")]
    public class Relationship : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;

        [Prototype("MinValue", true)]
        public int minValue;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Relationship(Enum e)
        {
            return DataBase.Get<Relationship>(e);
        }

        public static explicit operator Relationship(string e)
        {
            return DataBase.Get<Relationship>(e, reportMissing: true);
        }
    }
}
