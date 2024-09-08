using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
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
            return DataBase.Get<TownName>(e);
        }

        public static explicit operator TownName(string e)
        {
            return DataBase.Get<TownName>(e, reportMissing: true);
        }

        public void Set_names(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.names = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is string))
                {
                    Debug.LogError("names of type string received invalid type from array! " + list[i]);
                }
                this.names[i] = list[i] as string;
            }
        }
    }
}
