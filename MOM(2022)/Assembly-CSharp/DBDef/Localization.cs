using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("LOCALIZATION", "")]
    public class Localization : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Language", true)]
        public string language;

        [Prototype("Loc", true)]
        public Loc[] loc;

        public static explicit operator Localization(Enum e)
        {
            return DataBase.Get<Localization>(e);
        }

        public static explicit operator Localization(string e)
        {
            return DataBase.Get<Localization>(e, reportMissing: true);
        }

        public void Set_loc(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.loc = new Loc[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Loc))
                {
                    Debug.LogError("loc of type Loc received invalid type from array! " + list[i]);
                }
                this.loc[i] = list[i] as Loc;
            }
        }
    }
}
