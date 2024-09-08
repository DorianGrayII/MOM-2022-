namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sends an event when a UI Button is clicked.")]
    public class UiButtonOnClickEvent : ComponentAction<Button>
    {
        [RequiredField, CheckForComponent(typeof(Button)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Button component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when Clicked.")]
        public FsmEvent sendEvent;
        private Button button;

        public void DoOnClick()
        {
            base.SendEvent(this.eventTarget, this.sendEvent);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!base.UpdateCache(ownerDefaultTarget))
            {
                base.LogError("Missing GameObject ");
            }
            else
            {
                if (this.button != null)
                {
                    this.button.onClick.RemoveListener(new UnityAction(this.DoOnClick));
                }
                this.button = base.cachedComponent;
                if (this.button != null)
                {
                    this.button.onClick.AddListener(new UnityAction(this.DoOnClick));
                }
                else
                {
                    base.LogError("Missing UI.Button on " + ownerDefaultTarget.name);
                }
            }
        }

        public override void OnExit()
        {
            if (this.button != null)
            {
                this.button.onClick.RemoveListener(new UnityAction(this.DoOnClick));
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.sendEvent = null;
        }
    }
}

