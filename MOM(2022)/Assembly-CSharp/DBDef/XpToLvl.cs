using System;
using System.Collections.Generic;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("XP_TO_LVL", "")]
    public class XpToLvl : DBClass
    {
        public static string abbreviation = "";

        [Prototype("ExpReq", true)]
        public int[] expReq;

        public static explicit operator XpToLvl(Enum e)
        {
            return DataBase.Get<XpToLvl>(e);
        }

        public static explicit operator XpToLvl(string e)
        {
            return DataBase.Get<XpToLvl>(e, reportMissing: true);
        }

        public void Set_expReq(List<object> list)
        {
            if (list != null && list.Count != 0)
            {
                this.expReq = new int[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    this.expReq[i] = (int)list[i];
                }
            }
        }
    }
}
