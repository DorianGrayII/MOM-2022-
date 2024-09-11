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
            int iMinSpellCost = 0;
            int iMaxSpellCost = 0;
            int iMinSpellSkillCost = 0;
            int iMaxSpellSkillCost = 0;
            float fSpellStepCost = 1f;
            float fSpellStepSCost = 1f;
            int num7 = 0;
            int iSpellCost = 0;
            int num9 = 0;
            bool active = false;
            if (selectedSpell.worldScript != null && Battle.Get() == null)
            {
                iSpellCost = selectedSpell.worldCost;
                num9 = (iSpellCost * selectedSpell.changeableCost.maxMultipier - iSpellCost) / selectedSpell.changeableCost.costPerPoint;
                iMinSpellCost = selectedSpell.GetWorldCastingCost(PopupSpellCost.instance.caster);
                float num10 = (float)iMinSpellCost / (float)iSpellCost;
                fSpellStepCost = (float)selectedSpell.changeableCost.costPerPoint * num10;
                num7 = num9;
                iMaxSpellCost = iMinSpellCost + Mathf.CeilToInt(fSpellStepCost * (float)num7);
            }
            else if (selectedSpell.battleScript != null && Battle.Get() != null)
            {
                if (caster is PlayerWizard)
                {
                    BattlePlayer obj = ((battle.attacker.GetID() == caster.GetWizardOwner().GetID()) ? battle.attacker : battle.defender);
                    int mana = obj.mana;
                    int castingSkill = obj.castingSkill;
                    iSpellCost = selectedSpell.battleCost;
                    num9 = (iSpellCost * selectedSpell.changeableCost.maxMultipier - iSpellCost) / selectedSpell.changeableCost.costPerPoint;
                    iMinSpellSkillCost = selectedSpell.GetBattleCastingCost(PopupSpellCost.instance.caster);
                    float num11 = (float)iMinSpellSkillCost / (float)iSpellCost;
                    fSpellStepSCost = (float)selectedSpell.changeableCost.costPerPoint * num11;
                    iMinSpellCost = selectedSpell.GetBattleCastingCostByDistance(PopupSpellCost.instance.caster);
                    float num12 = (float)iMinSpellCost / (float)iSpellCost;
                    fSpellStepCost = (float)selectedSpell.changeableCost.costPerPoint * num12;
                    num7 = Mathf.Min(num9, (int)((float)(mana - iMinSpellCost) / fSpellStepCost), (int)((float)(castingSkill - iMinSpellSkillCost) / fSpellStepSCost));
                    iMaxSpellSkillCost = iMinSpellSkillCost + Mathf.CeilToInt(fSpellStepSCost * (float)num7);
                    iMaxSpellCost = iMinSpellCost + Mathf.CeilToInt(fSpellStepCost * (float)num7);
                    active = true;
                }
                else
                {
                    int mana2 = caster.GetMana();
                    iSpellCost = selectedSpell.GetBattleCastingCost(caster);
                    num9 = (iSpellCost * selectedSpell.changeableCost.maxMultipier - iSpellCost) / selectedSpell.changeableCost.costPerPoint;
                    iMinSpellCost = selectedSpell.GetBattleCastingCostByDistance(PopupSpellCost.instance.caster);
                    float num13 = (float)iMinSpellCost / (float)iSpellCost;
                    fSpellStepCost = (float)selectedSpell.changeableCost.costPerPoint * num13;
                    num7 = Mathf.Min(num9, (int)((float)(mana2 - iMinSpellCost) / fSpellStepCost));
                    iMaxSpellCost = iMinSpellCost + Mathf.CeilToInt(fSpellStepCost * (float)num7);
                }
            }
            else
            {
                Debug.LogError("PopupSpellCost have error because attempt of use spell " + selectedSpell.dbName.ToString());
            }
            PopupSpellCost.instance.minCost = iMinSpellCost;
            PopupSpellCost.instance.maxCost = iMaxSpellCost;
            PopupSpellCost.instance.stepCost = fSpellStepCost;
            PopupSpellCost.instance.minSCost = iMinSpellSkillCost;
            PopupSpellCost.instance.maxSCost = iMaxSpellSkillCost;
            PopupSpellCost.instance.sStepCost = fSpellStepSCost;
            int iMaxSliderValue = Mathf.Min(num7, iMaxSpellCost - iMinSpellCost);
            if (iMaxSliderValue == 0)
            {
                PopupSpellCost.instance.stepCapScalar = 0f;
            }
            else
            {
                PopupSpellCost.instance.stepCapScalar = (float)(iMaxSpellCost - iMinSpellCost) / ((float)iMaxSliderValue * fSpellStepCost);
            }
            PopupSpellCost.instance.sliderMana.minValue = 0f;
            PopupSpellCost.instance.sliderMana.value = 0f;
            PopupSpellCost.instance.sliderMana.maxValue = iMaxSliderValue;
            PopupSpellCost.instance.sliderMana.wholeNumbers = true;
            PopupSpellCost.instance.labelMinSkillCost.text = iMinSpellSkillCost.ToString();
            PopupSpellCost.instance.labelMaxSkillCost.text = iMaxSpellSkillCost.ToString();
            PopupSpellCost.instance.labelCurrentCastingSkillCost.text = iMinSpellSkillCost.ToString();
            PopupSpellCost.instance.labelMinManaCost.text = iMinSpellCost.ToString();
            PopupSpellCost.instance.labelMaxManaCost.text = iMaxSpellCost.ToString();
            PopupSpellCost.instance.labelCurrentManaCost.text = iMinSpellCost.ToString();
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
