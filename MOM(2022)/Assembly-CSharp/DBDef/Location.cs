using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("LOCATION", "")]
    public class Location : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("LocationType", false)]
        public ELocationType locationType;

        [Prototype("GuardianCreationScript", false)]
        public string guardianCreationScript;

        [Prototype("RampagingScript", false)]
        public string rampagingScript;

        [Prototype("LocationEvent", false)]
        public LocationEvent locationEvent;

        [Prototype("Dlc", false)]
        public string dlc;

        [Prototype("UnitBonus", false)]
        public Enchantment[] unitBonus;

        [Prototype("CustomBattleMap", false)]
        public string customBattleMap;

        [Prototype("CustomBattleLighting", false)]
        public float customBattleLighting;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Location(Enum e)
        {
            return DataBase.Get<Location>(e);
        }

        public static explicit operator Location(string e)
        {
            return DataBase.Get<Location>(e, reportMissing: true);
        }

        public void Set_unitBonus(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.unitBonus = new Enchantment[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Enchantment))
                {
                    Debug.LogError("unitBonus of type Enchantment received invalid type from array! " + list[i]);
                }
                this.unitBonus[i] = list[i] as Enchantment;
            }
        }
    }
}
