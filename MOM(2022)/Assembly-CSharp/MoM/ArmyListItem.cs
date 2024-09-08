namespace MOM
{
    using DBDef;
    using DBEnum;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ArmyListItem : MonoBehaviour
    {
        public RawImage portrait;
        public RawImage experience;
        public RawImage race;
        public GameObject goodEnchantment;
        public GameObject badEnchantment;
        public GameObject goodBadEnchantment;
        public GameObject highlight;
        public Slider hp;
        public bool greyScaleWhenNoMP = true;
        public bool useTooltip = true;
        private BaseUnit unit;
        private RolloverUnitTooltip unitTooltip;
        private Toggle toggle;
        private ArmyGrid grid;

        public void Awake()
        {
            this.grid = base.GetComponentInParent<ArmyGrid>();
            if (this.useTooltip)
            {
                this.unitTooltip = GameObjectUtils.GetOrAddComponent<RolloverUnitTooltip>(base.gameObject);
            }
            MouseClickEvent orAddComponent = GameObjectUtils.GetOrAddComponent<MouseClickEvent>(base.gameObject);
            orAddComponent.mouseRightClick = delegate (object d) {
                MOM.Unit unit = this.unit as MOM.Unit;
                if (unit != null)
                {
                    MOM.UnitInfo info = UIManager.Open<MOM.UnitInfo>(UIManager.Layer.Popup, base.GetComponentInParent<ScreenBase>());
                    List<MOM.Unit> units = null;
                    if (unit.group != null)
                    {
                        List<Reference<MOM.Unit>> list = unit.group.Get().GetUnits();
                        units = new List<MOM.Unit>(list.Count);
                        list.ForEach(o => units.Add(o.Get()));
                    }
                    else if (this.grid)
                    {
                        foreach (object obj2 in this.grid.GetObjectsInGrid())
                        {
                            if (obj2 is MOM.Unit)
                            {
                                units = new List<MOM.Unit> {
                                    obj2 as MOM.Unit
                                };
                            }
                        }
                    }
                    info.SetData(units, unit);
                }
            };
            orAddComponent.mouseDoubleClick = delegate (object d) {
                TownScreen screen = TownScreen.Get();
                if (screen != null)
                {
                    MOM.Unit unit = this.unit as MOM.Unit;
                    if (((unit != null) ? unit.group : null) != null)
                    {
                        UIManager.Close(screen);
                        FSMSelectionManager.Get().Select(unit.group.Get(), true);
                    }
                }
                ArmyManager manager = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (manager != null)
                {
                    MOM.Unit unit = this.unit as MOM.Unit;
                    if (((unit != null) ? unit.group : null) != null)
                    {
                        UIManager.Close(manager);
                        FSMSelectionManager.Get().Select(unit.group.Get(), true);
                    }
                }
            };
            orAddComponent.mouseLeftClick = delegate (object d) {
                ArmyManager screen = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if ((screen != null) && FSMMapGame.Get().IsCasting())
                {
                    FSMMapGame.Get().SetChosenTarget(this.unit.GetPosition());
                    MOM.Unit unit = this.unit as MOM.Unit;
                    if (((unit != null) ? unit.group : null) != null)
                    {
                        FSMSelectionManager.Get().Select(unit.group.Get(), true);
                        UIManager.Close(screen);
                    }
                }
            };
            if (this.grid)
            {
                this.toggle = base.GetComponent<Toggle>();
                if (this.toggle)
                {
                    this.toggle.onValueChanged.RemoveAllListeners();
                    this.toggle.onValueChanged.AddListener(b => this.grid.UpdateToggleState(this.unit, b));
                }
            }
        }

        private void UpdateVisuals()
        {
            string graphic = this.unit.dbSource.Get().GetDescriptionInfo().graphic;
            this.portrait.texture = AssetManager.Get<Texture2D>(graphic, true);
            if (this.greyScaleWhenNoMP)
            {
                this.portrait.material = (this.unit.Mp > 0) ? null : UIReferences.GetGrayscale();
            }
            if (this.unitTooltip)
            {
                this.unitTooltip.sourceAsUnit = this.Unit;
            }
            if (this.hp)
            {
                this.hp.value = this.unit.GetTotalHpPercent();
                this.hp.gameObject.SetActive((this.unit.GetTotalHpPercent() != 1f) && (this.unit.GetTotalHpPercent() != 0f));
            }
            if (this.experience)
            {
                Tag uType = (this.unit.dbSource.Get() is Hero) ? ((Tag) TAG.HERO_CLASS) : ((Tag) TAG.NORMAL_CLASS);
                UnitLvl lvl = DataBase.GetType<UnitLvl>().Find(o => ReferenceEquals(o.unitClass, uType) && (o.level == this.unit.GetLevel()));
                if (lvl == null)
                {
                    this.experience.gameObject.SetActive(false);
                }
                else
                {
                    this.experience.gameObject.SetActive(true);
                    this.experience.texture = DescriptionInfoExtension.GetTexture(lvl.GetDescriptionInfo());
                }
            }
            if (this.race != null)
            {
                if (IAttributeableExtension.GetAttFinal(this.unit, (Tag) TAG.REANIMATED) > 0)
                {
                    this.race.gameObject.SetActive(true);
                    this.race.texture = DescriptionInfoExtension.GetTexture(((Race) RACE.REALM_DEATH).GetDescriptionInfo());
                }
                else if ((IAttributeableExtension.GetAttFinal(this.unit, (Tag) TAG.FANTASTIC_CLASS) <= 0) && (IAttributeableExtension.GetAttFinal(this.unit, (Tag) TAG.HERO_CLASS) <= 0))
                {
                    this.race.gameObject.SetActive(false);
                }
                else
                {
                    this.race.gameObject.SetActive(true);
                    this.race.texture = DescriptionInfoExtension.GetTexture(this.unit.race.Get().GetDescriptionInfo());
                }
            }
            if ((this.goodEnchantment != null) && ((this.badEnchantment != null) && (this.goodBadEnchantment != null)))
            {
                bool flag = false;
                bool flag2 = false;
                foreach (EnchantmentInstance instance in this.unit.GetEnchantmentManager().GetEnchantmentsWithRemotes(false))
                {
                    if (instance.GetEnchantmentType() == EEnchantmentCategory.Positive)
                    {
                        flag = true;
                        continue;
                    }
                    if (instance.GetEnchantmentType() == EEnchantmentCategory.Negative)
                    {
                        flag2 = true;
                    }
                }
                this.goodEnchantment.SetActive(false);
                this.badEnchantment.SetActive(false);
                this.goodBadEnchantment.SetActive(false);
                if (flag & flag2)
                {
                    this.goodBadEnchantment.SetActive(true);
                }
                else if (flag)
                {
                    this.goodEnchantment.SetActive(true);
                }
                else if (flag2)
                {
                    this.badEnchantment.SetActive(true);
                }
            }
            if (this.toggle)
            {
                this.toggle.isOn = this.grid.IsSelected(this.unit);
            }
        }

        public BaseUnit Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                this.unit = value;
                this.UpdateVisuals();
            }
        }
    }
}

