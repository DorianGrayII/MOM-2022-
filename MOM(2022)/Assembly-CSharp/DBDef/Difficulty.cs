namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("DIFFICULTY", "")]
    public class Difficulty : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Name", true)]
        public string name;
        [Prototype("Dlc", false)]
        public string dlc;
        [Prototype("FullValue", true)]
        public float fullValue;
        [Prototype("TooltipName", true)]
        public string tooltipName;
        [Prototype("TooltipDescription", true)]
        public string tooltipDescription;
        [Prototype("Setting", true)]
        public DifficultyOption[] setting;

        public static explicit operator Difficulty(Enum e)
        {
            return DataBase.Get<Difficulty>(e, false);
        }

        public static explicit operator Difficulty(string e)
        {
            return DataBase.Get<Difficulty>(e, true);
        }

        public void Set_setting(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.setting = new DifficultyOption[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is DifficultyOption))
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
                        Debug.LogError("setting of type DifficultyOption received invalid type from array! " + text1);
                    }
                    this.setting[i] = list[i] as DifficultyOption;
                }
            }
        }
    }
}

