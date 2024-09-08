using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
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
            return DataBase.Get<Difficulty>(e);
        }

        public static explicit operator Difficulty(string e)
        {
            return DataBase.Get<Difficulty>(e, reportMissing: true);
        }

        public void Set_setting(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.setting = new DifficultyOption[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is DifficultyOption))
                {
                    Debug.LogError("setting of type DifficultyOption received invalid type from array! " + list[i]);
                }
                this.setting[i] = list[i] as DifficultyOption;
            }
        }
    }
}
