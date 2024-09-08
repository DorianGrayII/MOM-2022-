namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [AddComponentMenu("PlayMaker/UI/UI Click Event")]
    public class PlayMakerUiClickEvent : PlayMakerUiEventBase
    {
        public Button button;

        private void DoOnClick()
        {
            base.SendEvent(FsmEvent.UiClick);
        }

        protected override void Initialize()
        {
            if (!base.initialized)
            {
                base.initialized = true;
                if (this.button == null)
                {
                    this.button = base.GetComponent<Button>();
                }
                if (this.button != null)
                {
                    this.button.onClick.AddListener(new UnityAction(this.DoOnClick));
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.button != null)
            {
                this.button.onClick.RemoveListener(new UnityAction(this.DoOnClick));
            }
        }
    }
}

