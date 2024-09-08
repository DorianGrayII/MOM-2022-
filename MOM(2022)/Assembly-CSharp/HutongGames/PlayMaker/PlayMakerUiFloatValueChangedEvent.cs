namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

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
                    this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
                }
                if (this.scrollbar == null)
                {
                    this.scrollbar = base.GetComponent<Scrollbar>();
                }
                if (this.scrollbar != null)
                {
                    this.scrollbar.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.slider != null)
            {
                this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnValueChanged));
            }
            if (this.scrollbar != null)
            {
                this.scrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.OnValueChanged));
            }
        }

        private void OnValueChanged(float value)
        {
            Fsm.EventData.FloatData = value;
            base.SendEvent(FsmEvent.UiFloatValueChanged);
        }
    }
}

