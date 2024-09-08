namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("FOLIAGE_SET", "")]
    public class FoliageSet : DBClass
    {
        public static string abbreviation = "";
        [Prototype("SetName", true)]
        public string setName;
        [Prototype("Foliage", true)]
        public Foliage[] foliage;

        public static explicit operator FoliageSet(Enum e)
        {
            return DataBase.Get<FoliageSet>(e, false);
        }

        public static explicit operator FoliageSet(string e)
        {
            return DataBase.Get<FoliageSet>(e, true);
        }

        public void Set_foliage(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.foliage = new Foliage[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Foliage))
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
                        Debug.LogError("foliage of type Foliage received invalid type from array! " + text1);
                    }
                    this.foliage[i] = list[i] as Foliage;
                }
            }
        }
    }
}

