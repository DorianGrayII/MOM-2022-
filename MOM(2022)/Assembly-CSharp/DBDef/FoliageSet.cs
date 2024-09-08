using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
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
            return DataBase.Get<FoliageSet>(e);
        }

        public static explicit operator FoliageSet(string e)
        {
            return DataBase.Get<FoliageSet>(e, reportMissing: true);
        }

        public void Set_foliage(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.foliage = new Foliage[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Foliage))
                {
                    Debug.LogError("foliage of type Foliage received invalid type from array! " + list[i]);
                }
                this.foliage[i] = list[i] as Foliage;
            }
        }
    }
}
