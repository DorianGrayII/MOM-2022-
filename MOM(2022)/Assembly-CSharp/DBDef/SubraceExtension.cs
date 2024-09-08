namespace DBDef
{
    using DBEnum;
    using MHUtils;
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class SubraceExtension
    {
        [Extension]
        public static FInt GetTag(Subrace obj, Tag t)
        {
            if (obj.tags != null)
            {
                foreach (CountedTag tag in obj.tags)
                {
                    if (ReferenceEquals(tag.tag, t))
                    {
                        return tag.amount;
                    }
                }
            }
            return FInt.ZERO;
        }

        [Extension]
        public static FInt GetTag(Subrace obj, TAG t)
        {
            Tag tag = (Tag) t;
            return GetTag(obj, tag);
        }

        [Extension]
        public static string GetTagSTR(Subrace obj, TAG t)
        {
            return GetTag(obj, t).ToInt().ToString();
        }
    }
}

