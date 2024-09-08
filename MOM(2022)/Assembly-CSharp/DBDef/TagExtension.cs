using MHUtils;
using MOM;

namespace DBDef
{
    public static class TagExtension
    {
        public static void AddFinal(this NetDictionary<DBReference<Tag>, FInt> dict, Tag t, int val)
        {
            dict.AddFinal(t, new FInt(val));
        }

        public static void AddFinal(this NetDictionary<DBReference<Tag>, FInt> dict, Tag t, FInt val)
        {
            if (t.parent == null)
            {
                dict[t] += val;
                return;
            }
            FInt fInt = FInt.ZERO;
            if (dict.ContainsKey(t))
            {
                fInt = dict[t];
            }
            dict[t] = fInt + val;
            dict.AddFinal(t.parent, val);
        }

        public static void SetFinal(this NetDictionary<DBReference<Tag>, FInt> dict, Tag t, FInt val)
        {
            if (t.parent == null)
            {
                dict[t] = val;
                return;
            }
            FInt fInt = FInt.ZERO;
            if (dict.ContainsKey(t))
            {
                fInt = dict[t];
            }
            dict[t] = val;
            dict.AddFinal(t.parent, val - fInt);
        }

        public static FInt GetFinal(this NetDictionary<DBReference<Tag>, FInt> dict, Tag t)
        {
            if (dict.ContainsKey(t))
            {
                return dict[t];
            }
            return FInt.ZERO;
        }
    }
}
