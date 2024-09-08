// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupGeneral
using System.Collections;
using DBUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupGeneral : ScreenBase
{
    public Button button1;

    public Button button2;

    public Button button3;

    public TextMeshProUGUI tfHeader;

    public TextMeshProUGUI tfMessage;

    public GameObjectEnabler<PlayerWizard.Familiar> familiar;

    private Callback confirm;

    private Callback cancel;

    private Callback third;

    private object popupData;

    public static void OpenPopup(ScreenBase parent, string header, string message, string confirmMessage, Callback confirm = null, string cancelMessage = null, Callback cancel = null, string thirdMessage = null, Callback third = null, object data = null)
    {
        PopupGeneral popupGeneral = UIManager.Open<PopupGeneral>(UIManager.Layer.Popup, parent);
        popupGeneral.cancel = cancel;
        popupGeneral.confirm = confirm;
        popupGeneral.third = third;
        popupGeneral.tfHeader.text = Localization.Get(header, true);
        popupGeneral.tfMessage.text = Localization.Get(message, true);
        popupGeneral.popupData = data;
        if (Battle.Get() != null)
        {
            Battle.Get().SetMessageAttentionTo(popupGeneral);
        }
        if (cancelMessage == null)
        {
            popupGeneral.button2.gameObject.SetActive(value: false);
        }
        else
        {
            popupGeneral.button2.GetComponentInChildren<TextMeshProUGUI>().text = Localization.Get(cancelMessage, true);
        }
        if (confirmMessage == null)
        {
            popupGeneral.button1.gameObject.SetActive(value: false);
        }
        else
        {
            popupGeneral.button1.GetComponentInChildren<TextMeshProUGUI>().text = Localization.Get(confirmMessage, true);
        }
        if (thirdMessage == null)
        {
            popupGeneral.button3.gameObject.SetActive(value: false);
        }
        else
        {
            popupGeneral.button3.GetComponentInChildren<TextMeshProUGUI>().text = Localization.Get(thirdMessage, true, popupGeneral);
        }
        if (confirmMessage != null && cancelMessage == null && thirdMessage == null)
        {
            UIKeyboardClick uIKeyboardClick = popupGeneral.button1.gameObject.AddComponent<UIKeyboardClick>();
            uIKeyboardClick.button = KeyCode.Escape;
            uIKeyboardClick.UpdateByRemap();
        }
        popupGeneral.familiar.Clear();
        if (GameManager.GetHumanWizard() != null)
        {
            popupGeneral.familiar.Set(GameManager.GetHumanWizard().familiar);
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.button2)
        {
            UIManager.Close(this);
            if (this.cancel != null)
            {
                this.cancel(this.popupData);
            }
        }
        else if (s == this.button1)
        {
            UIManager.Close(this);
            if (this.confirm != null)
            {
                this.confirm(this.popupData);
            }
        }
        else if (s == this.button3)
        {
            UIManager.Close(this);
            if (this.third != null)
            {
                this.third(this.popupData);
            }
        }
    }

    public override IEnumerator PreClose()
    {
        yield return base.PreClose();
        if (Battle.Get() != null)
        {
            Battle.Get().SetMessageAttentionTo(null);
        }
    }
}
