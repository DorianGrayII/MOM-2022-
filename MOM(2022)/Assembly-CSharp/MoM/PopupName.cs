namespace MOM
{
    using DBUtils;
    using MHUtils.UI;
    using System;
    using System.Runtime.InteropServices;
    using TMPro;
    using UnityEngine.UI;

    public class PopupName : ScreenBase
    {
        public TMP_InputField inputField;
        public Button buttonConfirm;
        public Button buttonCancel;
        private Callback confirm;
        private Callback cancel;

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

        public static void OpenPopup(string defaultValue, Callback confirm, Callback cancel, ScreenBase parent)
        {
            PopupName local1 = UIManager.Open<PopupName>(UIManager.Layer.Popup, parent);
            local1.cancel = cancel;
            local1.confirm = confirm;
            local1.inputField.text = Localization.Get(defaultValue, true, Array.Empty<object>());
        }
    }
}

