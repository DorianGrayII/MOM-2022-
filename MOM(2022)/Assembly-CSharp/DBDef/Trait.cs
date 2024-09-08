using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("TRAIT", "")]
    public class Trait : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("Cost", false)]
        public int cost;

        [Prototype("PrerequisiteScript", false)]
        public string prerequisiteScript;

        [Prototype("RaceFilteringScript", false)]
        public string raceFilteringScript;

        [Prototype("InitialScript", true)]
        public string initialScript;

        [Prototype("StartingSpells", false)]
        public string[] startingSpells;

        [Prototype("Tags", false)]
        public CountedTag[] tags;

        [Prototype("Dlc", false)]
        public string dlc;

        [Prototype("RewardExclusion", false)]
        public bool rewardExclusion;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Trait(Enum e)
        {
            return DataBase.Get<Trait>(e);
        }

        public static explicit operator Trait(string e)
        {
            return DataBase.Get<Trait>(e, reportMissing: true);
        }

        public void Set_startingSpells(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.startingSpells = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is string))
                {
                    Debug.LogError("startingSpells of type string received invalid type from array! " + list[i]);
                }
                this.startingSpells[i] = list[i] as string;
            }
        }

        public void Set_tags(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.tags = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("tags of type CountedTag received invalid type from array! " + list[i]);
                }
                this.tags[i] = list[i] as CountedTag;
            }
        }
    }
}
