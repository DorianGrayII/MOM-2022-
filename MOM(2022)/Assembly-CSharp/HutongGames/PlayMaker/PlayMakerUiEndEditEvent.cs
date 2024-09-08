using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI End Edit Event")]
    public class PlayMakerUiEndEditEvent : PlayMakerUiEventBase
    {
        public InputField inputField;

        protected override void Initialize()
        {
            if (!base.initialized)
            {
                base.initialized = true;
                if (this.inputField == null)
                {
                    this.inputField = base.GetComponent<InputField>();
                }
                if (this.inputField != null)
                {
                    this.inputField.onEndEdit.AddListener(DoOnEndEdit);
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.inputField != null)
            {
                this.inputField.onEndEdit.RemoveListener(DoOnEndEdit);
            }
        }

        private void DoOnEndEdit(string value)
        {
            Fsm.EventData.StringData = value;
            base.SendEvent(FsmEvent.UiEndEdit);
        }
    }
}
