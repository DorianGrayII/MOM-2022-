namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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
            return DataBase.Get<Localization>(e, false);
        }

        public static explicit operator Localization(string e)
        {
            return DataBase.Get<Localization>(e, true);
        }

        public void Set_loc(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.loc = new Loc[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Loc))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("loc of type Loc received invalid type from array! " + text1);
                    }
                    this.loc[i] = list[i] as Loc;
                }
            }
        }
    }
}

