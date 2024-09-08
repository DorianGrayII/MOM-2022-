using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("SPELL", "")]
    public class Spell : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("ChangeableCost", false)]
        public ChangeableCost changeableCost;

        [Prototype("ResearchExclusion", false)]
        public bool researchExclusion;

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("WorldCost", false)]
        public int worldCost;

        [Prototype("BattleCost", false)]
        public int battleCost;

        [Prototype("UnitBattleCost", false)]
        public int unitBattleCost;

        [Prototype("CastEffect", false)]
        public string castEffect;

        [Prototype("BattleCastEffect", false)]
        public string battleCastEffect;

        [Prototype("ResearchCost", true)]
        public int researchCost;

        [Prototype("UpkeepCost", false)]
        public int upkeepCost;

        [Prototype("Realm", true)]
        public ERealm realm;

        [Prototype("Rarity", true)]
        public ERarity rarity;

        [Prototype("TargetType", true)]
        public TargetType targetType;

        [Prototype("TargetingScript", false)]
        public string targetingScript;

        [Prototype("BattleScript", false)]
        public string battleScript;

        [Prototype("WorldScript", false)]
        public string worldScript;

        [Prototype("AIBattleEvaluationScript", false)]
        public string aiBattleEvaluationScript;

        [Prototype("AIWorldEvaluationScript", false)]
        public string aiWorldEvaluationScript;

        [Prototype("StringData", false)]
        public string[] stringData;

        [Prototype("FIntData", false)]
        public FInt[] fIntData;

        [Prototype("EnchantmentData", false)]
        public Enchantment[] enchantmentData;

        [Prototype("BuildingData", false)]
        public Building[] buildingData;

        [Prototype("DamagePool", false)]
        public ESkillDamagePool damagePool;

        [Prototype("BattleAttackEffect", false)]
        public ESkillBattleAttackEffect battleAttackEffect;

        [Prototype("AdditionalGraphic", false)]
        public string additionalGraphic;

        [Prototype("AudioEffect", false)]
        public string audioEffect;

        [Prototype("HealingSpell", false)]
        public bool healingSpell;

        [Prototype("DispelingSpell", false)]
        public bool dispelingSpell;

        [Prototype("SummonFantasticUnitSpell", false)]
        public bool summonFantasticUnitSpell;

        [Prototype("ShowWizardsEnchantments", false)]
        public bool showWizardsEnchantments;

        [Prototype("TreasureExclude", false)]
        public bool treasureExclude;

        [Prototype("InvalidTarget", false)]
        public string invalidTarget;

        [Prototype("Dlc", false)]
        public string dlc;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Spell(Enum e)
        {
            return DataBase.Get<Spell>(e);
        }

        public static explicit operator Spell(string e)
        {
            return DataBase.Get<Spell>(e, reportMissing: true);
        }

        public void Set_stringData(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.stringData = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is string))
                {
                    Debug.LogError("stringData of type string received invalid type from array! " + list[i]);
                }
                this.stringData[i] = list[i] as string;
            }
        }

        public void Set_fIntData(List<object> list)
        {
            if (list != null && list.Count != 0)
            {
                this.fIntData = new FInt[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    this.fIntData[i] = (FInt)list[i];
                }
            }
        }

        public void Set_enchantmentData(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.enchantmentData = new Enchantment[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Enchantment))
                {
                    Debug.LogError("enchantmentData of type Enchantment received invalid type from array! " + list[i]);
                }
                this.enchantmentData[i] = list[i] as Enchantment;
            }
        }

        public void Set_buildingData(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.buildingData = new Building[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Building))
                {
                    Debug.LogError("buildingData of type Building received invalid type from array! " + list[i]);
                }
                this.buildingData[i] = list[i] as Building;
            }
        }
    }
}
