namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("GAMEPLAY_CONFIGURATION", "")]
    public class GameplayConfiguration : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Option", true)]
        public Gc[] option;

        public static explicit operator GameplayConfiguration(Enum e)
        {
            return DataBase.Get<GameplayConfiguration>(e, false);
        }

        public static explicit operator GameplayConfiguration(string e)
        {
            return DataBase.Get<GameplayConfiguration>(e, true);
        }

        public void Set_option(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.option = new Gc[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Gc))
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
                        Debug.LogError("option of type Gc received invalid type from array! " + text1);
                    }
                    this.option[i] = list[i] as Gc;
                }
            }
        }
    }
}

