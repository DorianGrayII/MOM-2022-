namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [AddComponentMenu("PlayMaker/UI/UI End Edit Event")]
    public class PlayMakerUiEndEditEvent : PlayMakerUiEventBase
    {
        public InputField inputField;

        private void DoOnEndEdit(string value)
        {
            Fsm.EventData.StringData = value;
            base.SendEvent(FsmEvent.UiEndEdit);
        }

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
                    this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.DoOnEndEdit));
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.inputField != null)
            {
                this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.DoOnEndEdit));
            }
        }
    }
}

