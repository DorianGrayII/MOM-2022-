namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("PERSONALITY", "")]
    public class Personality : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";
        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;
        [Prototype("TriggerFactorTags", false)]
        public CountedTag[] triggerFactorTags;
        [Prototype("Hostility", false)]
        public int hostility;
        [Prototype("WorldHostileCasting", false)]
        public int worldHostileCasting;
        [Prototype("DiplomaticContact", false)]
        public int diplomaticContact;
        [Prototype("ReactionTooNegativeDiplomacy", false)]
        public FInt reactionTooNegativeDiplomacy;
        [Prototype("ReactionTooPositiveDiplomacy", false)]
        public FInt reactionTooPositiveDiplomacy;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Personality(Enum e)
        {
            return DataBase.Get<Personality>(e, false);
        }

        public static explicit operator Personality(string e)
        {
            return DataBase.Get<Personality>(e, true);
        }

        public void Set_triggerFactorTags(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.triggerFactorTags = new CountedTag[list.Count];
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
                        Debug.LogError("triggerFactorTags of type CountedTag received invalid type from array! " + text1);
                    }
                    this.triggerFactorTags[i] = list[i] as CountedTag;
                }
            }
        }
    }
}

