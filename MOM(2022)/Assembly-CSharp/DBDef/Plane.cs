namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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

        public static explicit operator DBDef.Plane(Enum e)
        {
            return DataBase.Get<DBDef.Plane>(e, false);
        }

        public static explicit operator DBDef.Plane(string e)
        {
            return DataBase.Get<DBDef.Plane>(e, true);
        }

        public void Set_race(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.race = new Race[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Race))
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
                        Debug.LogError("race of type Race received invalid type from array! " + text1);
                    }
                    this.race[i] = list[i] as Race;
                }
            }
        }
    }
}

