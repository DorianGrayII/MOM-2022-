using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("ENCHANTMENT", "ENCH")]
    public class Enchantment : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "ENCH";

        [Prototype("HideEnch", false)]
        public bool hideEnch;

        [Prototype("AllowDispel", false)]
        public bool allowDispel;

        [Prototype("MindControl", false)]
        public bool mindControl;

        [Prototype("RemoveWhenParentIDChange", false)]
        public bool removeWhenParentIDChange;

        [Prototype("NonCombatDisplay", false)]
        public bool nonCombatDisplay;

        [Prototype("WizardTowerBonus", false)]
        public bool wizardTowerBonus;

        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;

        [Prototype("EnchantmentScript", false)]
        public EnchantmentScript[] scripts;

        [Prototype("EnchantmentApplicationScript", false)]
        public EnchantmentScript applicationScript;

        [Prototype("EnchantmentRemovalScript", false)]
        public EnchantmentScript removalScript;

        [Prototype("EnchantmentRequirementScript", false)]
        public EnchantmentScript requirementScript;

        [Prototype("UpkeepCost", false)]
        public int upkeepCost;

        [Prototype("LifeTime", false)]
        public int lifeTime;

        [Prototype("EnchLastingEffect", false)]
        public string enchLastingEffect;

        [Prototype("WorldEnchantment", false)]
        public bool worldEnchantment;

        [Prototype("EnchCategory", false)]
        public EEnchantmentCategory enchCategory;

        [Prototype("OnJoinWithUnit", false)]
        public string onJoinWithUnit;

        [Prototype("OnLeaveFromUnit", false)]
        public string onLeaveFromUnit;

        [Prototype("OnRemoteTriggerFilter", false)]
        public string onRemoteTriggerFilter;

        [Prototype("Realm", true)]
        public ERealm realm;

        [Prototype("Dlc", false)]
        public string dlc;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Enchantment(Enum e)
        {
            return DataBase.Get<Enchantment>(e);
        }

        public static explicit operator Enchantment(string e)
        {
            return DataBase.Get<Enchantment>(e, reportMissing: true);
        }

        public void Set_scripts(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.scripts = new EnchantmentScript[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is EnchantmentScript))
                {
                    Debug.LogError("scripts of type EnchantmentScript received invalid type from array! " + list[i]);
                }
                this.scripts[i] = list[i] as EnchantmentScript;
            }
        }
    }
}
