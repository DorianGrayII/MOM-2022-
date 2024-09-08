using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
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
                    this.scrollRect.onValueChanged.AddListener(OnValueChanged);
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.scrollRect != null)
            {
                this.scrollRect.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(Vector2 value)
        {
            Fsm.EventData.Vector2Data = value;
            base.SendEvent(FsmEvent.UiVector2ValueChanged);
        }
    }
}
