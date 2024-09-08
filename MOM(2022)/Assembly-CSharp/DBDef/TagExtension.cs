namespace DBDef
{
    using MHUtils;
    using MOM;
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class TagExtension
    {
        [Extension]
        public static void AddFinal(NetDictionary<DBReference<Tag>, FInt> dict, Tag t, FInt val)
        {
            if (t.parent == null)
            {
                NetDictionary<DBReference<Tag>, FInt> dictionary = dict;
                DBReference<Tag> reference = t;
                dictionary[reference] += val;
            }
            else
            {
                FInt zERO = FInt.ZERO;
                if (dict.ContainsKey(t))
                {
                    zERO = dict[t];
                }
                dict[t] = zERO + val;
                AddFinal(dict, t.parent, val);
            }
        }

        [Extension]
        public static void AddFinal(NetDictionary<DBReference<Tag>, FInt> dict, Tag t, int val)
        {
            AddFinal(dict, t, new FInt(val));
        }

        [Extension]
        public static FInt GetFinal(NetDictionary<DBReference<Tag>, FInt> dict, Tag t)
        {
            return (!dict.ContainsKey(t) ? FInt.ZERO : dict[t]);
        }

        [Extension]
        public static void SetFinal(NetDictionary<DBReference<Tag>, FInt> dict, Tag t, FInt val)
        {
            if (t.parent == null)
            {
                dict[t] = val;
            }
            else
            {
                FInt zERO = FInt.ZERO;
                if (dict.ContainsKey(t))
                {
                    zERO = dict[t];
                }
                dict[t] = val;
                AddFinal(dict, t.parent, val - zERO);
            }
        }
    }
}

