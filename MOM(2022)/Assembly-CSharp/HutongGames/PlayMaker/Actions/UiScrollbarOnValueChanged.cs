namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Catches UI Scrollbar onValueChanged event. Store the new value and/or send events. Event float data will contain the new Scrollbar value")]
    public class UiScrollbarOnValueChanged : ComponentAction<Scrollbar>
    {
        [RequiredField, CheckForComponent(typeof(Scrollbar)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [HutongGames.PlayMaker.Tooltip("Send this event when the UI Scrollbar value changes.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("Store new value in float variable."), UIHint(UIHint.Variable)]
        public FsmFloat value;
        private Scrollbar scrollbar;

        public void DoOnValueChanged(float _value)
        {
            this.value.Value = _value;
            Fsm.EventData.FloatData = _value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.scrollbar = base.cachedComponent;
                if (this.scrollbar != null)
                {
                    this.scrollbar.onValueChanged.AddListener(new UnityAction<float>(this.DoOnValueChanged));
                }
            }
        }

        public override void OnExit()
        {
            if (this.scrollbar != null)
            {
                this.scrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.DoOnValueChanged));
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
            this.value = null;
        }
    }
}

