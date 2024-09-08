using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("GAMEPLAY_CONFIGURATION", "")]
    public class GameplayConfiguration : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Option", true)]
        public Gc[] option;

        public static explicit operator GameplayConfiguration(Enum e)
        {
            return DataBase.Get<GameplayConfiguration>(e);
        }

        public static explicit operator GameplayConfiguration(string e)
        {
            return DataBase.Get<GameplayConfiguration>(e, reportMissing: true);
        }

        public void Set_option(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.option = new Gc[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Gc))
                {
                    Debug.LogError("option of type Gc received invalid type from array! " + list[i]);
                }
                this.option[i] = list[i] as Gc;
            }
        }
    }
}
