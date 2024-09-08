namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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
            return DataBase.Get<Trait>(e, false);
        }

        public static explicit operator Trait(string e)
        {
            return DataBase.Get<Trait>(e, true);
        }

        public void Set_startingSpells(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.startingSpells = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is string))
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
                        Debug.LogError("startingSpells of type string received invalid type from array! " + text1);
                    }
                    this.startingSpells[i] = list[i] as string;
                }
            }
        }

        public void Set_tags(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.tags = new CountedTag[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is CountedTag))
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
                        Debug.LogError("tags of type CountedTag received invalid type from array! " + text1);
                    }
                    this.tags[i] = list[i] as CountedTag;
                }
            }
        }
    }
}

