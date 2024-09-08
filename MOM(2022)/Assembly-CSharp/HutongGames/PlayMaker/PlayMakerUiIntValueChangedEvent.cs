using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Int Value Changed Event")]
    public class PlayMakerUiIntValueChangedEvent : PlayMakerUiEventBase
    {
        public Dropdown dropdown;

        protected override void Initialize()
        {
            if (!base.initialized)
            {
                base.initialized = true;
                if (this.dropdown == null)
                {
                    this.dropdown = base.GetComponent<Dropdown>();
                }
                if (this.dropdown != null)
                {
                    this.dropdown.onValueChanged.AddListener(OnValueChanged);
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.dropdown != null)
            {
                this.dropdown.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(int value)
        {
            Fsm.EventData.IntData = value;
            base.SendEvent(FsmEvent.UiIntValueChanged);
        }
    }
}
