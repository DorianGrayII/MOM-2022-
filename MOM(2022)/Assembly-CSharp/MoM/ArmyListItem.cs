using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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

        public void Awake()
        {
            this.grid = base.GetComponentInParent<ArmyGrid>();
            if (this.useTooltip)
            {
                this.unitTooltip = base.gameObject.GetOrAddComponent<RolloverUnitTooltip>();
            }
            MouseClickEvent orAddComponent = base.gameObject.GetOrAddComponent<MouseClickEvent>();
            orAddComponent.mouseRightClick = delegate
            {
                if (this.unit is Unit unit4)
                {
                    ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
                    UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Popup, componentInParent);
                    List<Unit> units = null;
                    if (unit4.group != null)
                    {
                        List<Reference<Unit>> units2 = unit4.group.Get().GetUnits();
                        units = new List<Unit>(units2.Count);
                        units2.ForEach(delegate(Reference<Unit> o)
                        {
                            units.Add(o.Get());
                        });
                    }
                    else if ((bool)this.grid)
                    {
                        foreach (object item in this.grid.GetObjectsInGrid())
                        {
                            if (item is Unit)
                            {
                                if (units == null)
                                {
                                    units = new List<Unit>();
                                }
                                units.Add(item as Unit);
                            }
                        }
                    }
                    unitInfo.SetData(units, unit4);
                }
            };
            orAddComponent.mouseDoubleClick = delegate
            {
                TownScreen townScreen = TownScreen.Get();
                if (townScreen != null)
                {
                    Unit unit2 = this.unit as Unit;
                    if (unit2?.group != null)
                    {
                        UIManager.Close(townScreen);
                        FSMSelectionManager.Get().Select(unit2.group.Get(), focus: true);
                    }
                }
                ArmyManager screen2 = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (screen2 != null)
                {
                    Unit unit3 = this.unit as Unit;
                    if (unit3?.group != null)
                    {
                        UIManager.Close(screen2);
                        FSMSelectionManager.Get().Select(unit3.group.Get(), focus: true);
                    }
                }
            };
            orAddComponent.mouseLeftClick = delegate
            {
                ArmyManager screen = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (screen != null && FSMMapGame.Get().IsCasting())
                {
                    FSMMapGame.Get().SetChosenTarget(this.unit.GetPosition());
                    Unit unit = this.unit as Unit;
                    if (unit?.group != null)
                    {
                        FSMSelectionManager.Get().Select(unit.group.Get(), focus: true);
                        UIManager.Close(screen);
                    }
                }
            };
            if (!this.grid)
            {
                return;
            }
            this.toggle = base.GetComponent<Toggle>();
            if ((bool)this.toggle)
            {
                this.toggle.onValueChanged.RemoveAllListeners();
                this.toggle.onValueChanged.AddListener(delegate(bool b)
                {
                    this.grid.UpdateToggleState(this.unit, b);
                });
            }
        }

        private void UpdateVisuals()
        {
            string graphic = this.unit.dbSource.Get().GetDescriptionInfo().graphic;
            this.portrait.texture = AssetManager.Get<Texture2D>(graphic);
            if (this.greyScaleWhenNoMP)
            {
                this.portrait.material = ((this.unit.Mp > 0) ? null : UIReferences.GetGrayscale());
            }
            if ((bool)this.unitTooltip)
            {
                this.unitTooltip.sourceAsUnit = this.Unit;
            }
            if ((bool)this.hp)
            {
                this.hp.value = this.unit.GetTotalHpPercent();
                this.hp.gameObject.SetActive(this.unit.GetTotalHpPercent() != 1f && !(this.unit.GetTotalHpPercent() <= 0f));
            }
            if ((bool)this.experience)
            {
                Tag uType = ((this.unit.dbSource.Get() is Hero) ? ((Tag)TAG.HERO_CLASS) : ((Tag)TAG.NORMAL_CLASS));
                UnitLvl unitLvl = DataBase.GetType<UnitLvl>().Find((UnitLvl o) => o.unitClass == uType && o.level == this.unit.GetLevel());
                if (unitLvl != null)
                {
                    this.experience.gameObject.SetActive(value: true);
                    this.experience.texture = unitLvl.GetDescriptionInfo().GetTexture();
                }
                else
                {
                    this.experience.gameObject.SetActive(value: false);
                }
            }
            if (this.race != null)
            {
                if (this.unit.GetAttFinal((Tag)TAG.REANIMATED) > 0)
                {
                    this.race.gameObject.SetActive(value: true);
                    this.race.texture = ((Race)RACE.REALM_DEATH).GetDescriptionInfo().GetTexture();
                }
                else if (this.unit.GetAttFinal((Tag)TAG.FANTASTIC_CLASS) > 0 || this.unit.GetAttFinal((Tag)TAG.HERO_CLASS) > 0)
                {
                    this.race.gameObject.SetActive(value: true);
                    this.race.texture = this.unit.race.Get().GetDescriptionInfo().GetTexture();
                }
                else
                {
                    this.race.gameObject.SetActive(value: false);
                }
            }
            if (this.goodEnchantment != null && this.badEnchantment != null && this.goodBadEnchantment != null)
            {
                List<EnchantmentInstance> enchantmentsWithRemotes = this.unit.GetEnchantmentManager().GetEnchantmentsWithRemotes();
                bool flag = false;
                bool flag2 = false;
                foreach (EnchantmentInstance item in enchantmentsWithRemotes)
                {
                    if (item.GetEnchantmentType() == EEnchantmentCategory.Positive)
                    {
                        flag = true;
                    }
                    else if (item.GetEnchantmentType() == EEnchantmentCategory.Negative)
                    {
                        flag2 = true;
                    }
                }
                this.goodEnchantment.SetActive(value: false);
                this.badEnchantment.SetActive(value: false);
                this.goodBadEnchantment.SetActive(value: false);
                if (flag && flag2)
                {
                    this.goodBadEnchantment.SetActive(value: true);
                }
                else if (flag)
                {
                    this.goodEnchantment.SetActive(value: true);
                }
                else if (flag2)
                {
                    this.badEnchantment.SetActive(value: true);
                }
            }
            if ((bool)this.toggle)
            {
                this.toggle.isOn = this.grid.IsSelected(this.unit);
            }
        }
    }
}
