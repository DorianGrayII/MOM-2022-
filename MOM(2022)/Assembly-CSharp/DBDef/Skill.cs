using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("SKILL", "")]
    public class Skill : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("HideSkill", false)]
        public bool hideSkill;

        [Prototype("Domain", false)]
        public ESkillDomain domain;

        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;

        [Prototype("DescriptionScript", false)]
        public string descriptionScript;

        [Prototype("NonCombatDisplay", false)]
        public bool nonCombatDisplay;

        [Prototype("Script", false)]
        public SkillScript[] script;

        [Prototype("SkillApplicationScript", false)]
        public SkillScript applicationScript;

        [Prototype("SkillRemovalScript", false)]
        public SkillScript removalScript;

        [Prototype("OnJoinWithUnit", false)]
        public string onJoinWithUnit;

        [Prototype("OnLeaveFromUnit", false)]
        public string onLeaveFromUnit;

        [Prototype("VersionSuper", false)]
        public Skill versionSuper;

        [Prototype("RelatedEnchantment", false)]
        public Enchantment[] relatedEnchantment;

        [Prototype("Stackable", false)]
        public bool stackable;

        [Prototype("Dlc", false)]
        public string dlc;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Skill(Enum e)
        {
            return DataBase.Get<Skill>(e);
        }

        public static explicit operator Skill(string e)
        {
            return DataBase.Get<Skill>(e, reportMissing: true);
        }

        public void Set_script(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.script = new SkillScript[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is SkillScript))
                {
                    Debug.LogError("script of type SkillScript received invalid type from array! " + list[i]);
                }
                this.script[i] = list[i] as SkillScript;
            }
        }

        public void Set_relatedEnchantment(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.relatedEnchantment = new Enchantment[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Enchantment))
                {
                    Debug.LogError("relatedEnchantment of type Enchantment received invalid type from array! " + list[i]);
                }
                this.relatedEnchantment[i] = list[i] as Enchantment;
            }
        }
    }
}
