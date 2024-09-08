using System.Collections;
using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupWizardDefeated : ScreenBase
    {
        public Button btOkay;

        public GameObject wizardCurrentGemGreen;

        public GameObject wizardCurrentGemBlue;

        public GameObject wizardCurrentGemRed;

        public GameObject wizardCurrentGemPurple;

        public GameObject wizardCurrentGemYellow;

        public TextMeshProUGUI labelWizardDefeated;

        public TextMeshProUGUI labelWizardName;

        public RawImage wizardImage;

        private static PopupWizardDefeated instance;

        public static PopupWizardDefeated OpenPopup(ScreenBase parent, PlayerWizard banishedWizard)
        {
            PlayerWizard wizard = GameManager.GetWizard(banishedWizard.banishedBy);
            string text = banishedWizard.GetName() + " " + Localization.Get("UI_HAS_BEEN_DEFEATED_BY", true) + " " + ((wizard != null) ? wizard.GetName() : Localization.Get("UI_NEUTRAL_ARMIES", true));
            PopupWizardDefeated.instance = UIManager.Open<PopupWizardDefeated>(UIManager.Layer.Popup, parent);
            PopupWizardDefeated.instance.labelWizardDefeated.text = text;
            PopupWizardDefeated.instance.labelWizardName.text = banishedWizard.GetName();
            PopupWizardDefeated.instance.wizardCurrentGemGreen.SetActive(banishedWizard.color == PlayerWizard.Color.Green);
            PopupWizardDefeated.instance.wizardCurrentGemBlue.SetActive(banishedWizard.color == PlayerWizard.Color.Blue);
            PopupWizardDefeated.instance.wizardCurrentGemRed.SetActive(banishedWizard.color == PlayerWizard.Color.Red);
            PopupWizardDefeated.instance.wizardCurrentGemPurple.SetActive(banishedWizard.color == PlayerWizard.Color.Purple);
            PopupWizardDefeated.instance.wizardCurrentGemYellow.SetActive(banishedWizard.color == PlayerWizard.Color.Yellow);
            PopupWizardDefeated.instance.wizardImage.texture = banishedWizard.Graphic;
            AudioLibrary.RequestSFX("EnemyDefeated");
            return PopupWizardDefeated.instance;
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btOkay)
            {
                UIManager.Close(this);
            }
        }

        public static bool IsOpen()
        {
            return PopupWizardDefeated.instance != null;
        }

        public override IEnumerator Closing()
        {
            PopupWizardDefeated.instance = null;
            yield return base.Closing();
        }
    }
}
