namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("TOWN_NAME", "")]
    public class TownName : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Race", true)]
        public Race race;
        [Prototype("Names", true)]
        public string[] names;

        public static explicit operator TownName(Enum e)
        {
            return DataBase.Get<TownName>(e, false);
        }

        public static explicit operator TownName(string e)
        {
            return DataBase.Get<TownName>(e, true);
        }

        public void Set_names(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.names = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is string))
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
                        Debug.LogError("names of type string received invalid type from array! " + text1);
                    }
                    this.names[i] = list[i] as string;
                }
            }
        }
    }
}

