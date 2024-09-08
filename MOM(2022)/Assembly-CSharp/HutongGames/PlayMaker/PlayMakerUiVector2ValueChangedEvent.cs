namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [AddComponentMenu("PlayMaker/UI/UI Vector2 Value Changed Event")]
    public class PlayMakerUiVector2ValueChangedEvent : PlayMakerUiEventBase
    {
        public ScrollRect scrollRect;

        protected override void Initialize()
        {
            if (!base.initialized)
            {
                base.initialized = true;
                if (this.scrollRect == null)
                {
                    this.scrollRect = base.GetComponent<ScrollRect>();
                }
                if (this.scrollRect != null)
                {
                    this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnValueChanged));
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.scrollRect != null)
            {
                this.scrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.OnValueChanged));
            }
        }

        private void OnValueChanged(Vector2 value)
        {
            Fsm.EventData.Vector2Data = value;
            base.SendEvent(FsmEvent.UiVector2ValueChanged);
        }
    }
}

