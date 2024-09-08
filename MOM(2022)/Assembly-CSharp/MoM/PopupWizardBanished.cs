// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupWizardBanished
using System.Collections;
using DBUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWizardBanished : ScreenBase
{
    public Button btOkay;

    public GameObject wizardCurrentGemGreen;

    public GameObject wizardCurrentGemBlue;

    public GameObject wizardCurrentGemRed;

    public GameObject wizardCurrentGemPurple;

    public GameObject wizardCurrentGemYellow;

    public TextMeshProUGUI labelWizardBanished;

    public TextMeshProUGUI labelWizardName;

    public RawImage wizardImage;

    public GameObjectEnabler<PlayerWizard.Familiar> familiar;

    private static PopupWizardBanished instance;

    public static PopupWizardBanished OpenPopup(ScreenBase parent, PlayerWizard banishedWizard)
    {
        PlayerWizard wizard = GameManager.GetWizard(banishedWizard.banishedBy);
        string text = banishedWizard.GetName() + " " + Localization.Get("UI_HAS_BEEN_BANISHED", true) + " " + ((wizard != null) ? wizard.GetName() : Localization.Get("UI_NEUTRAL_ARMIES", true));
        PopupWizardBanished.instance = UIManager.Open<PopupWizardBanished>(UIManager.Layer.Popup, parent);
        PopupWizardBanished.instance.labelWizardBanished.text = text;
        PopupWizardBanished.instance.labelWizardName.text = banishedWizard.GetName();
        PopupWizardBanished.instance.wizardCurrentGemGreen.SetActive(banishedWizard.color == PlayerWizard.Color.Green);
        PopupWizardBanished.instance.wizardCurrentGemBlue.SetActive(banishedWizard.color == PlayerWizard.Color.Blue);
        PopupWizardBanished.instance.wizardCurrentGemRed.SetActive(banishedWizard.color == PlayerWizard.Color.Red);
        PopupWizardBanished.instance.wizardCurrentGemPurple.SetActive(banishedWizard.color == PlayerWizard.Color.Purple);
        PopupWizardBanished.instance.wizardCurrentGemYellow.SetActive(banishedWizard.color == PlayerWizard.Color.Yellow);
        PopupWizardBanished.instance.wizardImage.texture = banishedWizard.Graphic;
        PopupWizardBanished.instance.familiar.Clear();
        if (GameManager.GetHumanWizard() != null)
        {
            PopupWizardBanished.instance.familiar.Set(GameManager.GetHumanWizard().familiar);
        }
        AudioLibrary.RequestSFX("EnemyBanished");
        return PopupWizardBanished.instance;
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
        return PopupWizardBanished.instance != null;
    }

    public override IEnumerator Closing()
    {
        PopupWizardBanished.instance = null;
        yield return base.Closing();
    }
}
