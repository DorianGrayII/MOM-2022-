using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
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
                    this.toggle.onValueChanged.AddListener(OnValueChanged);
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.toggle != null)
            {
                this.toggle.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(bool value)
        {
            Fsm.EventData.BoolData = value;
            base.SendEvent(FsmEvent.UiBoolValueChanged);
        }
    }
}
