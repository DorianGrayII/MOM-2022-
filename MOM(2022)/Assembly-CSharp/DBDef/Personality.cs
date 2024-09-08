using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
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
            return DataBase.Get<Personality>(e);
        }

        public static explicit operator Personality(string e)
        {
            return DataBase.Get<Personality>(e, reportMissing: true);
        }

        public void Set_triggerFactorTags(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.triggerFactorTags = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("triggerFactorTags of type CountedTag received invalid type from array! " + list[i]);
                }
                this.triggerFactorTags[i] = list[i] as CountedTag;
            }
        }
    }
}
