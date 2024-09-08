using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("PLANE", "")]
    public class Plane : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("Race", true)]
        public Race[] race;

        [Prototype("TypeName", true)]
        public string typeName;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Plane(Enum e)
        {
            return DataBase.Get<Plane>(e);
        }

        public static explicit operator Plane(string e)
        {
            return DataBase.Get<Plane>(e, reportMissing: true);
        }

        public void Set_race(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.race = new Race[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Race))
                {
                    Debug.LogError("race of type Race received invalid type from array! " + list[i]);
                }
                this.race[i] = list[i] as Race;
            }
        }
    }
}
