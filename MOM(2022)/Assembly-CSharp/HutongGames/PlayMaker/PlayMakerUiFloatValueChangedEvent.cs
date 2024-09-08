using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Float Value Changed Event")]
    public class PlayMakerUiFloatValueChangedEvent : PlayMakerUiEventBase
    {
        public Slider slider;

        public Scrollbar scrollbar;

        protected override void Initialize()
        {
            if (!base.initialized)
            {
                base.initialized = true;
                if (this.slider == null)
                {
                    this.slider = base.GetComponent<Slider>();
                }
                if (this.slider != null)
                {
                    this.slider.onValueChanged.AddListener(OnValueChanged);
                }
                if (this.scrollbar == null)
                {
                    this.scrollbar = base.GetComponent<Scrollbar>();
                }
                if (this.scrollbar != null)
                {
                    this.scrollbar.onValueChanged.AddListener(OnValueChanged);
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.slider != null)
            {
                this.slider.onValueChanged.RemoveListener(OnValueChanged);
            }
            if (this.scrollbar != null)
            {
                this.scrollbar.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(float value)
        {
            Fsm.EventData.FloatData = value;
            base.SendEvent(FsmEvent.UiFloatValueChanged);
        }
    }
}
