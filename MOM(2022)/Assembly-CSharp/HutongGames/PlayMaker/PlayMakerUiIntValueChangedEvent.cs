namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

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
                    this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnValueChanged));
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.dropdown != null)
            {
                this.dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnValueChanged));
            }
        }

        private void OnValueChanged(int value)
        {
            Fsm.EventData.IntData = value;
            base.SendEvent(FsmEvent.UiIntValueChanged);
        }
    }
}

