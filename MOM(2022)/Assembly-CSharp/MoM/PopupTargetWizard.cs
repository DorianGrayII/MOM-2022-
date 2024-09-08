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
    public class PopupTargetWizard : ScreenBase
    {
        private static PopupTargetWizard instance;

        public Button btCancel;

        public GridItemManager wizardsGrid;

        public TextMeshProUGUI labelManaDrained;

        public TextMeshProUGUI labelCastingSkillLost;

        public TextMeshProUGUI labelHeading;

        public TextMeshProUGUI labelMana;

        public GameObject goMana;

        private Spell spell;

        private Callback cancel;

        private Callback confirm;

        private bool useSpellBlastTooltip;

        public static void OpenPopup(ScreenBase parent, Spell spell, List<PlayerWizard> wizards, Callback confirm = null, Callback cancel = null, bool useSpellBlastTooltip = false)
        {
            PopupTargetWizard.instance = UIManager.Open<PopupTargetWizard>(UIManager.Layer.Popup, parent);
            PopupTargetWizard.instance.cancel = cancel;
            PopupTargetWizard.instance.confirm = confirm;
            PopupTargetWizard.instance.spell = spell;
            PopupTargetWizard.instance.useSpellBlastTooltip = useSpellBlastTooltip;
            PopupTargetWizard.instance.labelHeading.text = global::DBUtils.Localization.Get("UI_SELECT_TARGET_FOR", true) + spell.GetDescriptionInfo().GetLocalizedName();
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            PopupTargetWizard.instance.labelMana.text = humanWizard.GetMana() + "(" + humanWizard.CalculateManaIncome(includeUpkeep: true).SInt() + ")";
            PopupTargetWizard.instance.wizardsGrid.CustomDynamicItem(PopupTargetWizard.instance.WizardItem);
            PopupTargetWizard.instance.wizardsGrid.UpdateGrid(wizards);
        }

        private void WizardItem(GameObject itemSource, object source, object data, int index)
        {
            PlayerWizard playerWizard = source as PlayerWizard;
            TargetWizardListItem component = itemSource.GetComponent<TargetWizardListItem>();
            component.bannerBlue.SetActive(playerWizard.color == PlayerWizard.Color.Blue);
            component.bannerGreen.SetActive(playerWizard.color == PlayerWizard.Color.Green);
            component.bannerPurple.SetActive(playerWizard.color == PlayerWizard.Color.Purple);
            component.bannerRed.SetActive(playerWizard.color == PlayerWizard.Color.Red);
            component.bannerYellow.SetActive(playerWizard.color == PlayerWizard.Color.Yellow);
            component.portrait.texture = playerWizard.Graphic;
            component.labelAttitude.text = playerWizard.name;
            Button component2 = itemSource.GetComponent<Button>();
            if (component2 != null)
            {
                component2.onClick.RemoveAllListeners();
                component2.onClick.AddListener(delegate
                {
                    UIManager.Close(this);
                    if (this.confirm != null)
                    {
                        this.confirm(source);
                    }
                });
            }
            if (this.useSpellBlastTooltip)
            {
                itemSource.GetOrAddComponent<RolloverObject>().source = playerWizard;
            }
            if (this.spell != null)
            {
                bool flag = true;
                if (!string.IsNullOrEmpty(this.spell.targetingScript))
                {
                    PlayerWizard humanWizard = GameManager.GetHumanWizard();
                    flag = (bool)ScriptLibrary.Call(this.spell.targetingScript, new SpellCastData(humanWizard, null), playerWizard, this.spell);
                }
                component.portrait.material = (flag ? null : UIReferences.GetGrayscale());
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btCancel)
            {
                UIManager.Close(this);
                if (this.cancel != null)
                {
                    this.cancel(null);
                }
            }
        }

        public static bool IsOpen()
        {
            return PopupTargetWizard.instance != null;
        }

        public override IEnumerator Closing()
        {
            PopupTargetWizard.instance = null;
            yield return base.Closing();
        }

        public static void ShowDrainEffect(Spell spell, PlayerWizard wizard, int drainValue, bool mana, Callback confirm = null, Callback cancel = null)
        {
            List<PlayerWizard> items = new List<PlayerWizard> { wizard };
            PopupTargetWizard.instance = UIManager.Open<PopupTargetWizard>(UIManager.Layer.Popup);
            PopupTargetWizard.instance.cancel = cancel;
            PopupTargetWizard.instance.confirm = confirm;
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            PopupTargetWizard.instance.labelMana.text = humanWizard.GetMana() + "(" + humanWizard.CalculateManaIncome(includeUpkeep: true).SInt() + ")";
            PopupTargetWizard.instance.labelHeading.text = global::DBUtils.Localization.Get("UI_SELECT_TARGET_FOR", true) + spell.GetDescriptionInfo().GetLocalizedName();
            PopupTargetWizard.instance.wizardsGrid.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                PlayerWizard playerWizard = source as PlayerWizard;
                TargetWizardListItem component = itemSource.GetComponent<TargetWizardListItem>();
                component.bannerBlue.SetActive(playerWizard.color == PlayerWizard.Color.Blue);
                component.bannerGreen.SetActive(playerWizard.color == PlayerWizard.Color.Green);
                component.bannerPurple.SetActive(playerWizard.color == PlayerWizard.Color.Purple);
                component.bannerRed.SetActive(playerWizard.color == PlayerWizard.Color.Red);
                component.bannerYellow.SetActive(playerWizard.color == PlayerWizard.Color.Yellow);
                component.portrait.texture = playerWizard.Graphic;
                component.labelAttitude.text = playerWizard.name;
            });
            PopupTargetWizard.instance.wizardsGrid.UpdateGrid(items);
            if (mana)
            {
                PopupTargetWizard.instance.labelManaDrained.gameObject.SetActive(value: true);
                PopupTargetWizard.instance.labelManaDrained.text = wizard.GetName() + " UI_LOSES " + drainValue + " UI_POINTS_OF_MANA";
            }
            else
            {
                PopupTargetWizard.instance.labelCastingSkillLost.gameObject.SetActive(value: true);
                PopupTargetWizard.instance.labelCastingSkillLost.text = wizard.GetName() + " UI_LOSES " + drainValue + " UI_POINTS_OF_CASTING_SKILL";
            }
            PopupTargetWizard.instance.btCancel.gameObject.SetActive(value: false);
            PopupTargetWizard.instance.StartCoroutine(PopupTargetWizard.instance.FadeOut());
        }

        private IEnumerator FadeOut()
        {
            CanvasGroup cg = PopupTargetWizard.instance.GetComponent<CanvasGroup>();
            yield return new WaitForSeconds(3f);
            for (float ft = 1f; ft >= 0f; ft -= 0.1f)
            {
                cg.alpha = ft;
                yield return null;
            }
            UIManager.Close(this);
        }
    }
}
