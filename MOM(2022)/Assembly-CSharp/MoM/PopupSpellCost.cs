using System.Collections;
using DBDef;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupSpellCost : ScreenBase
    {
        private static PopupSpellCost instance;

        public Button btCancel;

        public Button btConfirm;

        public RawImage spellIcon;

        public TextMeshProUGUI labelSpellName;

        public TextMeshProUGUI labelMinManaCost;

        public TextMeshProUGUI labelMaxManaCost;

        public TextMeshProUGUI labelMinSkillCost;

        public TextMeshProUGUI labelMaxSkillCost;

        public TextMeshProUGUI labelCurrentManaCost;

        public TextMeshProUGUI labelCurrentCastingSkillCost;

        public TextMeshProUGUI labelCastTime;

        public GameObject minSkill;

        public GameObject maxSkill;

        public GameObject currentSkill;

        public GameObject castTime;

        public Slider sliderMana;

        public TextMeshProUGUI labelEffect;

        private MagicAndResearch magicAndResearch;

        private Spell selectedSpell;

        private Battle battle;

        private ISpellCaster caster;

        private Button btCloseSpellBook;

        private int minCost;

        private int maxCost;

        private float stepCost = 1f;

        private float stepCapScalar = 1f;

        private int minSCost;

        private int maxSCost;

        private float sStepCost = 1f;

        public static void OpenPopup(ScreenBase parent, MagicAndResearch smFromBook, Battle battle, Spell selectedSpell, ISpellCaster caster, Button btCloseSpellBook)
        {
            PopupSpellCost.instance = UIManager.Open<PopupSpellCost>(UIManager.Layer.Popup, parent);
            DescriptionInfo descriptionInfo = selectedSpell.GetDescriptionInfo();
            PopupSpellCost.instance.magicAndResearch = smFromBook;
            PopupSpellCost.instance.selectedSpell = selectedSpell;
            PopupSpellCost.instance.battle = battle;
            PopupSpellCost.instance.caster = caster;
            PopupSpellCost.instance.btCloseSpellBook = btCloseSpellBook;
            PopupSpellCost.instance.spellIcon.texture = descriptionInfo.GetTexture();
            PopupSpellCost.instance.labelSpellName.text = descriptionInfo.GetLocalizedName();
            int num = 0;
            int num2 = 0;
            float num3 = 1f;
            int num4 = 0;
            int num5 = 0;
            float num6 = 1f;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            bool active = false;
            if (selectedSpell.worldScript != null && Battle.Get() == null)
            {
                num8 = selectedSpell.worldCost;
                num9 = (num8 * selectedSpell.changeableCost.maxMultipier - num8) / selectedSpell.changeableCost.costPerPoint;
                num = selectedSpell.GetWorldCastingCost(PopupSpellCost.instance.caster);
                float num10 = (float)num / (float)num8;
                num3 = (float)selectedSpell.changeableCost.costPerPoint * num10;
                num7 = num9;
                num2 = num + Mathf.CeilToInt(num3 * (float)num7);
            }
            else if (selectedSpell.battleScript != null && Battle.Get() != null)
            {
                if (caster is PlayerWizard)
                {
                    BattlePlayer obj = ((battle.attacker.GetID() == caster.GetWizardOwner().GetID()) ? battle.attacker : battle.defender);
                    int mana = obj.mana;
                    int castingSkill = obj.castingSkill;
                    num8 = selectedSpell.battleCost;
                    num9 = (num8 * selectedSpell.changeableCost.maxMultipier - num8) / selectedSpell.changeableCost.costPerPoint;
                    num4 = selectedSpell.GetBattleCastingCost(PopupSpellCost.instance.caster);
                    float num11 = (float)num4 / (float)num8;
                    num6 = (float)selectedSpell.changeableCost.costPerPoint * num11;
                    num = selectedSpell.GetBattleCastingCostByDistance(PopupSpellCost.instance.caster);
                    float num12 = (float)num / (float)num8;
                    num3 = (float)selectedSpell.changeableCost.costPerPoint * num12;
                    num7 = Mathf.Min(num9, (int)((float)(mana - num) / num3), (int)((float)(castingSkill - num4) / num6));
                    num5 = num4 + Mathf.CeilToInt(num6 * (float)num7);
                    num2 = num + Mathf.CeilToInt(num3 * (float)num7);
                    active = true;
                }
                else
                {
                    int mana2 = caster.GetMana();
                    num8 = selectedSpell.GetBattleCastingCost(caster);
                    num9 = (num8 * selectedSpell.changeableCost.maxMultipier - num8) / selectedSpell.changeableCost.costPerPoint;
                    num = selectedSpell.GetBattleCastingCostByDistance(PopupSpellCost.instance.caster);
                    float num13 = (float)num / (float)num8;
                    num3 = (float)selectedSpell.changeableCost.costPerPoint * num13;
                    num7 = Mathf.Min(num9, (int)((float)(mana2 - num) / num3));
                    num2 = num + Mathf.CeilToInt(num3 * (float)num7);
                }
            }
            else
            {
                Debug.LogError("PopupSpellCost have error because attempt of use spell " + selectedSpell.dbName.ToString());
            }
            PopupSpellCost.instance.minCost = num;
            PopupSpellCost.instance.maxCost = num2;
            PopupSpellCost.instance.stepCost = num3;
            PopupSpellCost.instance.minSCost = num4;
            PopupSpellCost.instance.maxSCost = num5;
            PopupSpellCost.instance.sStepCost = num6;
            int num14 = Mathf.Min(num7, num2 - num);
            if (num14 == 0)
            {
                PopupSpellCost.instance.stepCapScalar = 0f;
            }
            else
            {
                PopupSpellCost.instance.stepCapScalar = (float)(num2 - num) / ((float)num14 * num3);
            }
            PopupSpellCost.instance.sliderMana.minValue = 0f;
            PopupSpellCost.instance.sliderMana.value = 0f;
            PopupSpellCost.instance.sliderMana.maxValue = num14;
            PopupSpellCost.instance.sliderMana.wholeNumbers = true;
            PopupSpellCost.instance.labelMinSkillCost.text = num4.ToString();
            PopupSpellCost.instance.labelMaxSkillCost.text = num5.ToString();
            PopupSpellCost.instance.labelCurrentCastingSkillCost.text = num4.ToString();
            PopupSpellCost.instance.labelMinManaCost.text = num.ToString();
            PopupSpellCost.instance.labelMaxManaCost.text = num2.ToString();
            PopupSpellCost.instance.labelCurrentManaCost.text = num.ToString();
            PopupSpellCost.instance.labelEffect.text = PopupSpellCost.instance.UpdateEffectText();
            PopupSpellCost.instance.minSkill.SetActive(active);
            PopupSpellCost.instance.maxSkill.SetActive(active);
            PopupSpellCost.instance.currentSkill.SetActive(active);
            PopupSpellCost.instance.sliderMana.onValueChanged.RemoveAllListeners();
            PopupSpellCost.instance.sliderMana.onValueChanged.AddListener(delegate
            {
                PopupSpellCost.instance.UpdateEffectText();
            });
        }

        private void StartCastingSpell()
        {
            int num = Mathf.RoundToInt(this.sliderMana.value);
            if (this.selectedSpell.worldScript != null && this.battle == null)
            {
                this.magicAndResearch.extensionItemSpellWorld.extraMana = Mathf.CeilToInt((float)num * Mathf.Max(this.stepCost, 1f));
                this.magicAndResearch.extensionItemSpellWorld.extraPower = Mathf.FloorToInt((float)num * Mathf.Max(1f, 1f / this.stepCost));
            }
            else
            {
                int extraSkill = Mathf.Min(this.minSCost + Mathf.CeilToInt((float)num * this.sStepCost * this.stepCapScalar), PopupSpellCost.instance.maxSCost) - this.minSCost;
                this.magicAndResearch.extensionItemSpellBattle.extraMana = Mathf.CeilToInt((float)num * Mathf.Max(this.stepCost, 1f));
                this.magicAndResearch.extensionItemSpellBattle.extraSkill = extraSkill;
                this.magicAndResearch.extensionItemSpellBattle.extraPower = Mathf.FloorToInt((float)num * Mathf.Max(1f, 1f / this.stepCost));
            }
            if (this.battle == null)
            {
                this.magicAndResearch.curentlyCastSpell = this.selectedSpell;
                this.magicAndResearch.craftItemSpell = null;
                this.btCloseSpellBook.onClick.Invoke();
                HUD.Get()?.UpdateCastingButton();
            }
            else if (this.battle != null && this.caster != null)
            {
                this.battle.activeTurn.StartCasting(this.selectedSpell, this.caster);
                this.btCloseSpellBook.onClick.Invoke();
            }
            else
            {
                Debug.LogError("PopupSpellCost was use in casting spell " + this.selectedSpell.dbName + " but spell is either world or battle type.");
            }
        }

        private string UpdateEffectText()
        {
            int num = Mathf.RoundToInt(this.sliderMana.value);
            this.labelCurrentManaCost.text = (this.minCost + Mathf.CeilToInt((float)num * Mathf.Max(this.stepCost, 1f))).ToString();
            int a = this.minSCost + Mathf.CeilToInt((float)num * this.sStepCost * this.stepCapScalar);
            this.labelCurrentCastingSkillCost.text = Mathf.Min(a, PopupSpellCost.instance.maxSCost).ToString();
            if (PopupSpellCost.instance.selectedSpell.dispelingSpell)
            {
                return this.labelEffect.text = this.selectedSpell.changeableCost.desType.GetLocalizedTargetTypeDescription();
            }
            int num2 = 0;
            if (this.selectedSpell.fIntData != null)
            {
                num2 = this.selectedSpell.fIntData[0].ToInt();
            }
            int num3 = num2 + Mathf.FloorToInt((float)num * Mathf.Max(1f, 1f / this.stepCost));
            return this.labelEffect.text = num3 + " " + this.selectedSpell.changeableCost.desType.GetLocalizedTargetTypeDescription();
        }

        public static bool IsOpen()
        {
            return PopupSpellCost.instance != null;
        }

        public override IEnumerator Closing()
        {
            PopupSpellCost.instance = null;
            yield return base.Closing();
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btCancel)
            {
                this.sliderMana.onValueChanged.RemoveAllListeners();
                UIManager.Close(this);
            }
            if (s == this.btConfirm)
            {
                this.sliderMana.onValueChanged.RemoveAllListeners();
                this.StartCastingSpell();
                UIManager.Close(this);
            }
        }
    }
}
