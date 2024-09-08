using DBEnum;
using MHUtils;

namespace DBDef
{
    public static class SubraceExtension
    {
        public static FInt GetTag(this Subrace obj, Tag t)
        {
            if (obj.tags == null)
            {
                return FInt.ZERO;
            }
            CountedTag[] tags = obj.tags;
            foreach (CountedTag countedTag in tags)
            {
                if (countedTag.tag == t)
                {
                    return countedTag.amount;
                }
            }
            return FInt.ZERO;
        }

        public static FInt GetTag(this Subrace obj, TAG t)
        {
            Tag t2 = (Tag)t;
            return obj.GetTag(t2);
        }

        public static string GetTagSTR(this Subrace obj, TAG t)
        {
            return obj.GetTag(t).ToInt().ToString();
        }
    }
}
