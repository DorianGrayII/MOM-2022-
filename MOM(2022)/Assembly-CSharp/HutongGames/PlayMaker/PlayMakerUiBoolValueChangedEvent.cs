namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [AddComponentMenu("PlayMaker/UI/UI Bool Value Changed Event")]
    public class PlayMakerUiBoolValueChangedEvent : PlayMakerUiEventBase
    {
        public Toggle toggle;

        protected override void Initialize()
        {
            if (!base.initialized)
            {
                base.initialized = true;
                if (this.toggle == null)
                {
                    this.toggle = base.GetComponent<Toggle>();
                }
                if (this.toggle != null)
                {
                    this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.toggle != null)
            {
                this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
            }
        }

        private void OnValueChanged(bool value)
        {
            Fsm.EventData.BoolData = value;
            base.SendEvent(FsmEvent.UiBoolValueChanged);
        }
    }
}

