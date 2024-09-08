using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class Magic : ScreenBase
    {
        public Button buttonClose;

        public Button buttonTransmute;

        public Button buttonCasting;

        public Button buttonResearch;

        public Button btTransmuteGoldToMana;

        public Button btTransmuteManaToGold;

        public Toggle tgActiveEnchantments;

        public Toggle tgWorldEnchantments;

        public Toggle tgWizardEnchantments;

        public TextMeshProUGUI tfCastingSkill;

        public TextMeshProUGUI tfCastingSkillProgress;

        public TextMeshProUGUI tfCastingSkillIncrease;

        public TextMeshProUGUI tfManaReserve;

        public TextMeshProUGUI tfResearchTotal;

        public TextMeshProUGUI tfManaPerTurn;

        public TextMeshProUGUI tfCastingSpellName;

        public TextMeshProUGUI tfCastingTime;

        public TextMeshProUGUI tfResearchName;

        public TextMeshProUGUI tfResearchTime;

        public TextMeshProUGUI tfSummonTownName;

        public TextMeshProUGUI tfMana;

        public TextMeshProUGUI tfResearch;

        public TextMeshProUGUI tfSkill;

        public TextMeshProUGUI tfPower;

        public TextMeshProUGUI tfAlchemyRatio;

        public TextMeshProUGUI tfCurrentGold;

        public TextMeshProUGUI tfCurrentMana;

        public TextMeshProUGUI tfActiveEnchantments;

        public TextMeshProUGUI tfWorldEnchantments;

        public TextMeshProUGUI tfWizardEnchantments;

        public TMP_InputField inputGold;

        public TMP_InputField inputMana;

        public RawImage imageCasting;

        public RawImage imageResearch;

        public GridItemManager gridActiveEnchantments;

        public GridItemManager gridWorldEnchantments;

        public GridItemManager gridWizardEnchantments;

        public GameObject goActiveEnchantments;

        public GameObject goWorldEnchantments;

        public GameObject goWizardEnchantments;

        public Slider sliderMana;

        public Slider sliderResearch;

        public Slider sliderSkill;

        public Slider sliderSkillProgress;

        public Slider sliderSkillProgressAfterThisTurn;

        public Image imageCastingAfterThisTurn;

        public Image imageCastingDone;

        public Image imageResearchAfterThisTurn;

        public Image imageResearchDone;

        public Toggle tgLockMana;

        public Toggle tgLockResearch;

        public Toggle tgLockSkill;

        public RolloverObject rolloverPower;

        public RolloverObject rolloverResearch;

        private StatDetails powerDetails = new StatDetails();

        private StatDetails researchDetails = new StatDetails();

        private Callback confirm;

        private Callback cancel;

        private Callback third;

        private MagicAndResearch mr;

        private PlayerWizard w;

        private List<EnchantmentInstance> enchantments = new List<EnchantmentInstance>();

        public override IEnumerator PreStart()
        {
            this.rolloverPower.source = this.powerDetails;
            this.rolloverResearch.source = this.researchDetails;
            this.w = GameManager.GetHumanWizard();
            this.mr = this.w.GetMagicAndResearch();
            this.Initialization();
            this.UpdateScreen();
            yield return base.PreStart();
            this.inputGold.onEndEdit.AddListener(AlchemyUpdateGold);
            this.inputMana.onEndEdit.AddListener(AlchemyUpdateMana);
            this.AlchemyGoldManaSwitch(this.GoldToMana());
            AudioLibrary.RequestSFX("OpenMagicScreen");
        }

        public void Initialization()
        {
            this.tgLockMana.onValueChanged.AddListener(delegate(bool b)
            {
                this.mr.SetManaLock(b);
            });
            this.tgLockResearch.onValueChanged.AddListener(delegate(bool b)
            {
                this.mr.SetResearchLock(b);
            });
            this.tgLockSkill.onValueChanged.AddListener(delegate(bool b)
            {
                this.mr.SetSkillLock(b);
            });
            this.tgActiveEnchantments.onValueChanged.AddListener(delegate
            {
                this.UpdateActiveEnchantments();
            });
            this.tgWizardEnchantments.onValueChanged.AddListener(delegate
            {
                this.UpdateWizardEnchantments();
            });
            this.tgWorldEnchantments.onValueChanged.AddListener(delegate
            {
                this.UpdateWorldEnchantments();
            });
            this.btTransmuteGoldToMana.onClick.AddListener(delegate
            {
                this.AlchemyGoldManaSwitch(b: false);
            });
            this.btTransmuteManaToGold.onClick.AddListener(delegate
            {
                this.AlchemyGoldManaSwitch(b: true);
            });
            this.buttonTransmute.onClick.AddListener(delegate
            {
                this.Transmute();
            });
            this.buttonClose.onClick.AddListener(delegate
            {
                HUD hUD = HUD.Get();
                if ((bool)hUD)
                {
                    hUD.UpdateHUD();
                }
                TownScreen townScreen = TownScreen.Get();
                if ((bool)townScreen)
                {
                    townScreen.UpdateTopPanel();
                }
            });
            this.gridActiveEnchantments.CustomDynamicItem(EnchantmentItem, UpdateActiveEnchantments);
            this.gridWizardEnchantments.CustomDynamicItem(EnchantmentItem, UpdateWizardEnchantments);
            this.gridWorldEnchantments.CustomDynamicItem(EnchantmentItem, UpdateWorldEnchantments);
            PlayerWizard w = GameManager.GetHumanWizard();
            List<EnchantmentInstance> list = GameManager.Get().GetEnchantments().FindAll((EnchantmentInstance o) => w.GetDiscoveredWizards().Contains(o.owner.GetEntity() as PlayerWizard) || o.owner.GetEntity() == w);
            this.tfActiveEnchantments.text = global::DBUtils.Localization.Get("UI_YOU_CONTROL", true) + " (" + EnchantmentRegister.GetByWizard(w).Count + ")";
            this.tfWorldEnchantments.text = global::DBUtils.Localization.Get("UI_ON_WORLD", true) + " (" + list.Count + ")";
            this.tfWizardEnchantments.text = global::DBUtils.Localization.Get("UI_ON_YOU", true) + " (" + w.GetEnchantments().Count + ")";
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.buttonCasting)
            {
                UIManager.Close(this);
                UIManager.Open<CastSpells>(UIManager.Layer.Standard);
            }
            if (s == this.buttonResearch)
            {
                UIManager.Close(this);
                UIManager.Open<ResearchSpells>(UIManager.Layer.Standard);
            }
            if (s.name == "ButtonClose")
            {
                UIManager.Close(this);
            }
        }

        private void EnchantmentItem(GameObject itemSource, object source, object data, int index)
        {
            EnchantmentInstance e = source as EnchantmentInstance;
            if (e == null)
            {
                return;
            }
            MagicEnchantmentListItem component = itemSource.GetComponent<MagicEnchantmentListItem>();
            if (component.goUpkeep != null)
            {
                if (e.owner != null && e.owner.ID == GameManager.GetHumanWizard().GetID())
                {
                    int enchantmentCost = e.GetEnchantmentCost(GameManager.GetWizard(e.owner.ID));
                    if (enchantmentCost > 0)
                    {
                        component.labelUpkeep.text = enchantmentCost + "/" + global::DBUtils.Localization.Get("UI_TURN", true);
                        component.goUpkeep.SetActive(value: true);
                    }
                    else
                    {
                        component.goUpkeep.SetActive(value: false);
                    }
                }
                else
                {
                    component.goUpkeep.SetActive(value: false);
                }
            }
            component.enchantment.Set(e);
            if ((bool)component.enchantmentTarget)
            {
                component.enchantmentTarget.Set(e);
            }
            if (!component.btCancelEnchantment)
            {
                return;
            }
            component.btCancelEnchantment.gameObject.SetActive(e.owner == GameManager.GetHumanWizard() && e.source.Get().allowDispel);
            component.btCancelEnchantment.onClick.RemoveAllListeners();
            component.btCancelEnchantment.onClick.AddListener(delegate
            {
                if (e.manager == null)
                {
                    Debug.LogError("missing manager");
                }
                else
                {
                    e.manager.owner.RemoveEnchantment(e);
                    this.UpdateScreen();
                }
            });
        }

        private void UpdateActiveEnchantments()
        {
            this.goActiveEnchantments.SetActive(this.tgActiveEnchantments.isOn);
            this.enchantments.Clear();
            this.enchantments.AddRange(EnchantmentRegister.GetByWizard(this.w));
            this.tfActiveEnchantments.text = global::DBUtils.Localization.Get("UI_YOU_CONTROL", true) + " (" + this.enchantments.Count + ")";
            if (this.tgActiveEnchantments.isOn)
            {
                this.gridActiveEnchantments.UpdateGrid(this.enchantments);
            }
        }

        private void UpdateWizardEnchantments()
        {
            this.goWizardEnchantments.SetActive(this.tgWizardEnchantments.isOn);
            this.enchantments.Clear();
            this.enchantments.AddRange(this.w.GetEnchantments().FindAll((EnchantmentInstance o) => !o.source.Get().hideEnch));
            this.tfWizardEnchantments.text = global::DBUtils.Localization.Get("UI_ON_YOU", true) + " (" + this.enchantments.Count + ")";
            if (this.tgWizardEnchantments.isOn)
            {
                this.gridWizardEnchantments.UpdateGrid(this.enchantments);
            }
        }

        private void UpdateWorldEnchantments()
        {
            this.goWorldEnchantments.SetActive(this.tgWorldEnchantments.isOn);
            this.enchantments.Clear();
            this.enchantments.AddRange(GameManager.Get().GetEnchantments());
            this.tfWorldEnchantments.text = global::DBUtils.Localization.Get("UI_ON_WORLD", true) + " (" + this.enchantments.Count + ")";
            if (this.tgWorldEnchantments.isOn)
            {
                this.gridWorldEnchantments.UpdateGrid(this.enchantments);
            }
        }

        public void UpdateScreen()
        {
            this.UpdateSliders();
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            MagicAndResearch magicAndResearch = humanWizard.GetMagicAndResearch();
            this.UpdateActiveEnchantments();
            this.UpdateWorldEnchantments();
            this.UpdateWizardEnchantments();
            this.tfCastingSkill.text = humanWizard.GetTotalCastingSkill().ToString();
            this.tfCastingSkillProgress.text = humanWizard.castingSkillDevelopment + "/" + humanWizard.GetNextLevelCastingSkill();
            this.tfCastingSkillIncrease.text = "+" + magicAndResearch.GetSkillIncome() + "/";
            this.sliderSkillProgress.value = this.CastingSkillProgressPercent();
            this.sliderSkillProgressAfterThisTurn.value = this.CastingSkillProgressPercent() + magicAndResearch.GetSkillIncome();
            this.tfManaReserve.text = humanWizard.GetMana().ToString();
            this.researchDetails.Clear();
            this.tfResearchTotal.text = humanWizard.CalculateResearchIncome(this.researchDetails) + "/";
            int num = humanWizard.CalculateManaIncome(includeUpkeep: true);
            if (num >= 0)
            {
                this.tfManaPerTurn.text = "+" + num + "/";
            }
            else
            {
                this.tfManaPerTurn.text = num + "/";
            }
            this.tfCurrentGold.text = "/" + humanWizard.money;
            this.tfCurrentMana.text = "/" + humanWizard.mana;
            if (magicAndResearch.curentlyCastSpell != null)
            {
                Spell curentlyCastSpell = magicAndResearch.curentlyCastSpell;
                magicAndResearch.GetCastingProgress(out var curentStatus, out var nextTurnStatus, out var turnsLeft);
                this.tfCastingSpellName.text = curentlyCastSpell.GetDescriptionInfo().GetLocalizedName();
                this.imageCastingAfterThisTurn.fillAmount = nextTurnStatus;
                this.imageCastingDone.fillAmount = curentStatus;
                this.tfCastingTime.text = turnsLeft.ToString();
                this.imageCasting.texture = curentlyCastSpell.GetDescriptionInfo().GetTexture();
            }
            else
            {
                this.tfCastingSpellName.text = "---";
                this.imageCastingAfterThisTurn.fillAmount = 0f;
                this.imageCastingDone.fillAmount = 0f;
                this.tfCastingTime.text = "-";
                this.imageCasting.texture = UIReferences.GetTransparent();
            }
            this.buttonResearch.interactable = FSMCoreGame.Get().CanResearch();
            if (magicAndResearch.curentlyResearched != null)
            {
                Spell spell = magicAndResearch.curentlyResearched.Get();
                magicAndResearch.GetResearchProgress(out var curentStatus2, out var nextTurnStatus2, out var turnsLeft2);
                this.tfResearchName.text = spell.GetDescriptionInfo().GetLocalizedName();
                this.imageResearchAfterThisTurn.fillAmount = nextTurnStatus2;
                this.imageResearchDone.fillAmount = curentStatus2;
                if (turnsLeft2 > 99)
                {
                    this.tfResearchTime.text = "99+";
                }
                else if (GameManager.GetHumanWizard().CalculateResearchIncome() == 0)
                {
                    this.tfResearchTime.text = "∞";
                }
                else if (turnsLeft2 < 0)
                {
                    this.tfResearchTime.text = "1";
                }
                else
                {
                    this.tfResearchTime.text = turnsLeft2.ToString();
                }
                this.imageResearch.texture = spell.GetDescriptionInfo().GetTexture();
            }
            else
            {
                this.tfResearchName.text = "---";
                this.imageResearchAfterThisTurn.fillAmount = 0f;
                this.imageResearchDone.fillAmount = 0f;
                this.tfResearchTime.text = "-";
                this.imageResearch.texture = UIReferences.GetTransparent();
            }
            if (humanWizard.summoningCircle != null)
            {
                TownLocation townLocation = humanWizard.summoningCircle.Get();
                this.tfSummonTownName.text = townLocation.name;
            }
            else
            {
                this.tfSummonTownName.text = global::DBUtils.Localization.Get("UI_NONE", true);
            }
        }

        public void UpdateSliders()
        {
            this.sliderMana.onValueChanged.RemoveAllListeners();
            this.sliderMana.value = this.mr.manaShare;
            this.sliderResearch.onValueChanged.RemoveAllListeners();
            this.sliderResearch.value = this.mr.researchShare;
            this.sliderSkill.onValueChanged.RemoveAllListeners();
            this.sliderSkill.value = this.mr.skillShare;
            this.sliderMana.onValueChanged.AddListener(delegate(float a)
            {
                this.mr.SetManaShare(a);
                this.UpdateScreen();
            });
            this.sliderResearch.onValueChanged.AddListener(delegate(float a)
            {
                this.mr.SetResearchShare(a);
                this.UpdateScreen();
            });
            this.sliderSkill.onValueChanged.AddListener(delegate(float a)
            {
                this.mr.SetSkillShare(a);
                this.UpdateScreen();
            });
            Multitype<int, int, int> powerSplit = this.mr.GetPowerSplit();
            this.tfResearch.text = powerSplit.t0.ToString();
            this.tfMana.text = powerSplit.t1.ToString();
            this.tfSkill.text = powerSplit.t2.ToString();
            this.powerDetails.Clear();
            this.tfPower.text = this.mr.GetTotalPower(this.powerDetails).ToString();
            if ((bool)HUD.Get())
            {
                HUD.Get().UpdateCastingButton();
                HUD.Get().UpdateResearchButton();
            }
        }

        private void Transmute()
        {
            if (this.GoldToMana())
            {
                int result = 0;
                if (int.TryParse(this.inputGold.text, out result))
                {
                    this.w.mana += result / this.GetAlchemyRatio();
                    this.w.money -= result;
                    this.inputMana.text = "0";
                    this.inputGold.text = "0";
                    this.UpdateScreen();
                }
                else
                {
                    this.inputGold.text = "0";
                    this.inputMana.text = "0";
                }
            }
            else
            {
                int result2 = 0;
                if (int.TryParse(this.inputMana.text, out result2))
                {
                    this.w.money += result2 / this.GetAlchemyRatio();
                    this.w.mana -= result2;
                    this.inputMana.text = "0";
                    this.inputGold.text = "0";
                    this.UpdateScreen();
                }
                else
                {
                    this.inputMana.text = "0";
                    this.inputGold.text = "0";
                }
            }
            HUD.Get()?.UpdateHUD();
            TownScreen.Get()?.UpdateTopPanel();
            TownScreen.Get()?.UpdateBuyButton();
        }

        private void AlchemyGoldManaSwitch(bool b)
        {
            int alchemyRatio = this.GetAlchemyRatio();
            if (b)
            {
                this.tfAlchemyRatio.text = alchemyRatio + ":1";
            }
            else
            {
                this.tfAlchemyRatio.text = "1:" + alchemyRatio;
            }
            this.inputMana.text = "0";
            this.inputGold.text = "0";
        }

        private int GetAlchemyRatio()
        {
            return this.w.alchemyRatio;
        }

        private void AlchemyUpdateGold(string s)
        {
            int alchemyRatio = this.GetAlchemyRatio();
            int num = 0;
            int result = 0;
            if (int.TryParse(s, out result))
            {
                if (!this.GoldToMana())
                {
                    num = result * alchemyRatio;
                    if (num > this.w.mana)
                    {
                        num = this.ClampByRatio(this.w.mana);
                    }
                    else if (num < 0)
                    {
                        num = 0;
                    }
                    this.inputGold.text = (num / alchemyRatio).ToString();
                    this.inputMana.text = num.ToString();
                }
                else
                {
                    result = this.ClampByRatio(result);
                    if (result > this.w.money)
                    {
                        result = this.ClampByRatio(this.w.money);
                    }
                    else if (result < 0)
                    {
                        result = 0;
                    }
                    this.inputGold.text = result.ToString();
                    this.inputMana.text = (result / alchemyRatio).ToString();
                }
            }
            else
            {
                this.inputMana.text = "0";
                this.inputGold.text = "0";
            }
        }

        private void AlchemyUpdateMana(string s)
        {
            int alchemyRatio = this.GetAlchemyRatio();
            int result = 0;
            int num = 0;
            if (int.TryParse(s, out result))
            {
                if (this.GoldToMana())
                {
                    num = result * alchemyRatio;
                    if (num > this.w.money)
                    {
                        num = this.ClampByRatio(this.w.money);
                    }
                    else if (num < 0)
                    {
                        num = 0;
                    }
                    this.inputGold.text = num.ToString();
                    this.inputMana.text = (num / alchemyRatio).ToString();
                }
                else
                {
                    result = this.ClampByRatio(result);
                    if (result > this.w.mana)
                    {
                        result = this.ClampByRatio(this.w.mana);
                    }
                    else if (result < 0)
                    {
                        result = 0;
                    }
                    this.inputGold.text = (result / alchemyRatio).ToString();
                    this.inputMana.text = result.ToString();
                }
            }
            else
            {
                this.inputMana.text = "0";
                this.inputGold.text = "0";
            }
        }

        private bool GoldToMana()
        {
            return this.btTransmuteGoldToMana.gameObject.activeSelf;
        }

        private int ClampByRatio(int value)
        {
            int alchemyRatio = this.GetAlchemyRatio();
            return value / alchemyRatio * alchemyRatio;
        }

        private int CastingSkillProgressPercent()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            humanWizard.GetMagicAndResearch();
            float num = humanWizard.castingSkillDevelopment;
            float num2 = humanWizard.GetNextLevelCastingSkill();
            return (int)(num / num2 * 100f);
        }
    }
}
