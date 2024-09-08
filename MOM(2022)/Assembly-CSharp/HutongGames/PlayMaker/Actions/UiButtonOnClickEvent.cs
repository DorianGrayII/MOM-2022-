using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends an event when a UI Button is clicked.")]
    public class UiButtonOnClickEvent : ComponentAction<Button>
    {
        [RequiredField]
        [CheckForComponent(typeof(Button))]
        [Tooltip("The GameObject with the UI Button component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when Clicked.")]
        public FsmEvent sendEvent;

        private Button button;

        public override void Reset()
        {
            this.gameObject = null;
            this.sendEvent = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                if (this.button != null)
                {
                    this.button.onClick.RemoveListener(DoOnClick);
                }
                this.button = base.cachedComponent;
                if (this.button != null)
                {
                    this.button.onClick.AddListener(DoOnClick);
                }
                else
                {
                    base.LogError("Missing UI.Button on " + ownerDefaultTarget.name);
                }
            }
            else
            {
                base.LogError("Missing GameObject ");
            }
        }

        public override void OnExit()
        {
            if (this.button != null)
            {
                this.button.onClick.RemoveListener(DoOnClick);
            }
        }

        public void DoOnClick()
        {
            base.SendEvent(this.eventTarget, this.sendEvent);
        }
    }
}
