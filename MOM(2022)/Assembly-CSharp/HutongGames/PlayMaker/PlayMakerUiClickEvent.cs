using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Click Event")]
    public class PlayMakerUiClickEvent : PlayMakerUiEventBase
    {
        public Button button;

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
                    this.button.onClick.AddListener(DoOnClick);
                }
            }
        }

        protected void OnDisable()
        {
            base.initialized = false;
            if (this.button != null)
            {
                this.button.onClick.RemoveListener(DoOnClick);
            }
        }

        private void DoOnClick()
        {
            base.SendEvent(FsmEvent.UiClick);
        }
    }
}
