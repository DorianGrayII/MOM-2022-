using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine.UI;

namespace MOM
{
    public class PopupName : ScreenBase
    {
        public TMP_InputField inputField;

        public Button buttonConfirm;

        public Button buttonCancel;

        private Callback confirm;

        private Callback cancel;

        public static void OpenPopup(string defaultValue, Callback confirm, Callback cancel = null, ScreenBase parent = null)
        {
            PopupName popupName = UIManager.Open<PopupName>(UIManager.Layer.Popup, parent);
            popupName.cancel = cancel;
            popupName.confirm = confirm;
            popupName.inputField.text = Localization.Get(defaultValue, true);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.buttonCancel)
            {
                UIManager.Close(this);
                if (this.cancel != null)
                {
                    this.cancel(null);
                }
            }
            else if (s == this.buttonConfirm)
            {
                string text = this.inputField.text;
                UIManager.Close(this);
                if (this.confirm != null)
                {
                    this.confirm(text);
                }
            }
        }
    }
}
