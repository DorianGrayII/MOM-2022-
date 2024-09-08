using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("TAG", "")]
    public class Tag : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("UseCount", false)]
        public bool useCount;

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("Parent", false)]
        public Tag parent;

        [Prototype("TagType", false)]
        public ETagType tagType;

        [Prototype("CanGoNegative", false)]
        public bool canGoNegative;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Tag(Enum e)
        {
            return DataBase.Get<Tag>(e);
        }

        public static explicit operator Tag(string e)
        {
            return DataBase.Get<Tag>(e, reportMissing: true);
        }
    }
}
