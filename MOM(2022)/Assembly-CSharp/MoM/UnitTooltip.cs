using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class UnitTooltip : TooltipBase
    {
        public TextMeshProUGUI labelName;

        public TextMeshProUGUI labelMelee;

        public TextMeshProUGUI labelMeleeChanceToHit;

        public TextMeshProUGUI labelRanged;

        public TextMeshProUGUI labelRangedChanceToHit;

        public TextMeshProUGUI labelAmmo;

        public TextMeshProUGUI labelDefence;

        public TextMeshProUGUI labelResistance;

        public TextMeshProUGUI labelHits;

        public TextMeshProUGUI labelFigures;

        public TextMeshProUGUI labelMP;

        public TextMeshProUGUI labelUpkeepMoney;

        public TextMeshProUGUI labelUpkeepFood;

        public TextMeshProUGUI labelUpkeepMana;

        public Slider sliderHP;

        public GameObject iconMovementWalking;

        public GameObject iconMovementFlying;

        public GameObject iconMovementSwimming;

        public GameObject iconUpkeepMoney;

        public GameObject iconUpkeepFood;

        public GameObject iconUpkeepMana;

        public RawImage icon;

        public Unit localVirtualUnit;

        public GridItemManager unitSkillsGrid;

        public override void Populate(object o)
        {
            BaseUnit baseUnit = o as BaseUnit;
            if (o is Subrace source)
            {
                this.localVirtualUnit = Unit.CreateFrom(source);
                baseUnit = this.localVirtualUnit;
            }
            if (baseUnit == null)
            {
                return;
            }
            this.labelName.text = baseUnit.GetName();
            this.unitSkillsGrid.CustomDynamicItem(SkillItem);
            if (baseUnit is BattleUnit)
            {
                BattleUnit battleUnit = baseUnit as BattleUnit;
                this.labelMelee.text = battleUnit.GetCurentFigure().attack.ToString();
                this.labelMeleeChanceToHit.text = (int)(battleUnit.GetCurentFigure().attackChance * 100f + 0.5f) + "%";
                this.labelDefence.text = battleUnit.GetCurentFigure().defence.ToString();
                this.labelResistance.text = battleUnit.GetCurentFigure().resist.ToString();
                this.labelHits.text = battleUnit.currentFigureHP + "/" + battleUnit.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
                this.labelFigures.text = battleUnit.FigureCount() + "/" + battleUnit.maxCount;
                this.labelMP.text = battleUnit.GetAttributes().GetFinal(TAG.MOVEMENT_POINTS).ToInt()
                    .ToString();
                if (battleUnit.IsRangedUnit())
                {
                    this.labelRanged.text = battleUnit.GetCurentFigure().rangedAttack.ToString();
                    this.labelRangedChanceToHit.text = (int)(battleUnit.GetCurentFigure().rangedAttackChance * 100f + 0.5f) + "%";
                    this.labelAmmo.text = battleUnit.GetCurentFigure().rangedAmmo + "/" + battleUnit.GetAttributes().GetFinal(TAG.AMMUNITION).ToInt();
                }
                else
                {
                    this.labelRanged.text = "0";
                    this.labelRangedChanceToHit.text = "0%";
                    this.labelAmmo.text = "0";
                }
                float totalHpPercent = battleUnit.GetTotalHpPercent();
                this.sliderHP.value = totalHpPercent;
                this.sliderHP.gameObject.SetActive(totalHpPercent != 1f || totalHpPercent <= 0f);
                this.UpdateSkillsGrid(battleUnit);
            }
            else if (baseUnit is Unit)
            {
                Unit unit = baseUnit as Unit;
                this.labelMelee.text = unit.GetAttributes().GetFinal(TAG.MELEE_ATTACK).ToInt()
                    .ToString();
                this.labelMeleeChanceToHit.text = (unit.GetAttributes().GetFinal(TAG.MELEE_ATTACK_CHANCE) * 100).ToInt() + "%";
                this.labelDefence.text = unit.GetAttributes().GetFinal(TAG.DEFENCE).ToInt()
                    .ToString();
                this.labelResistance.text = unit.GetAttributes().GetFinal(TAG.RESIST).ToInt()
                    .ToString();
                this.labelHits.text = unit.currentFigureHP + "/" + unit.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
                this.labelFigures.text = unit.FigureCount() + "/" + unit.MaxCount();
                if (this.localVirtualUnit != null)
                {
                    this.labelMP.text = unit.GetAttributes().GetFinal(TAG.MOVEMENT_POINTS).ToInt()
                        .ToString();
                }
                else
                {
                    this.labelMP.text = unit.Mp.ToString();
                }
                float totalHpPercent2 = unit.GetTotalHpPercent();
                this.sliderHP.value = totalHpPercent2;
                this.sliderHP.gameObject.SetActive(totalHpPercent2 != 1f || totalHpPercent2 <= 0f);
                if (unit.IsRangedUnit())
                {
                    this.labelRanged.text = unit.GetAttributes().GetFinal(TAG.RANGE_ATTACK).ToInt()
                        .ToString();
                    this.labelRangedChanceToHit.text = (unit.GetAttributes().GetFinal(TAG.RANGE_ATTACK_CHANCE) * 100).ToInt() + "%";
                    this.labelAmmo.text = unit.GetAttributes().GetFinal(TAG.AMMUNITION).ToInt()
                        .ToString();
                }
                else
                {
                    this.labelRanged.text = "0";
                    this.labelRangedChanceToHit.text = "0%";
                    this.labelAmmo.text = "0";
                }
                this.UpdateSkillsGrid(unit);
            }
            this.iconMovementWalking.SetActive(baseUnit.GetAttributes().GetFinal(TAG.CAN_WALK) > 0);
            this.iconMovementFlying.SetActive(baseUnit.GetAttributes().GetFinal(TAG.CAN_FLY) > 0 || baseUnit.GetAttributes().GetFinal(TAG.WIND_WALKING) > 0);
            this.iconMovementSwimming.SetActive(baseUnit.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0);
            int num = baseUnit.GetAttributes().GetFinal(TAG.UPKEEP_GOLD).ToInt();
            int num2 = baseUnit.GetAttributes().GetFinal(TAG.UPKEEP_FOOD).ToInt();
            int num3 = baseUnit.GetAttributes().GetFinal(TAG.UPKEEP_MANA).ToInt();
            this.iconUpkeepMoney.SetActive(num > 0);
            this.iconUpkeepFood.SetActive(num2 > 0);
            this.iconUpkeepMana.SetActive(num3 > 0);
            this.labelUpkeepMoney.text = num.ToString();
            this.labelUpkeepFood.text = num2.ToString();
            this.labelUpkeepMana.text = num3.ToString();
            this.icon.texture = AssetManager.Get<Texture2D>(baseUnit.dbSource.Get().GetDescriptionInfo().graphic);
        }

        private void OnDestroy()
        {
            if (this.localVirtualUnit != null)
            {
                this.localVirtualUnit.Destroy();
                this.localVirtualUnit = null;
            }
        }

        private void SkillItem(GameObject itemSource, object source, object data, int index)
        {
            DBReference<Skill> dBReference = source as DBReference<Skill>;
            EnchantmentInstance enchantmentInstance = source as EnchantmentInstance;
            SimpleListItem component = itemSource.GetComponent<SimpleListItem>();
            if (dBReference != null && !dBReference.Get().nonCombatDisplay)
            {
                this.unitSkillsGrid.gameObject.SetActive(value: true);
                component.icon.texture = dBReference.Get().GetDescriptionInfo().GetTexture();
            }
            else if (enchantmentInstance != null && !enchantmentInstance.source.Get().hideEnch && !enchantmentInstance.source.Get().nonCombatDisplay)
            {
                this.unitSkillsGrid.gameObject.SetActive(value: true);
                component.icon.texture = enchantmentInstance.source.Get().GetDescriptionInfo().GetTexture();
            }
            else
            {
                itemSource.gameObject.SetActive(value: false);
            }
        }

        private void UpdateSkillsGrid(BaseUnit bu)
        {
            List<object> list = new List<object>();
            list.AddRange(bu.GetSkills().FindAll((DBReference<Skill> o) => !o.Get().hideSkill));
            list.AddRange(bu.GetEnchantmentManager().GetEnchantmentsWithRemotes().FindAll((EnchantmentInstance o) => !o.source.Get().hideEnch));
            this.unitSkillsGrid.UpdateGrid(list);
        }
    }
}
