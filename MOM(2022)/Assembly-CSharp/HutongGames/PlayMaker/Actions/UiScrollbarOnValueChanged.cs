using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Catches UI Scrollbar onValueChanged event. Store the new value and/or send events. Event float data will contain the new Scrollbar value")]
    public class UiScrollbarOnValueChanged : ComponentAction<Scrollbar>
    {
        [RequiredField]
        [CheckForComponent(typeof(Scrollbar))]
        [Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Send this event when the UI Scrollbar value changes.")]
        public FsmEvent sendEvent;

        [Tooltip("Store new value in float variable.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat value;

        private Scrollbar scrollbar;

        public override void Reset()
        {
            this.gameObject = null;
            this.eventTarget = FsmEventTarget.Self;
            this.sendEvent = null;
            this.value = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.scrollbar = base.cachedComponent;
                if (this.scrollbar != null)
                {
                    this.scrollbar.onValueChanged.AddListener(DoOnValueChanged);
                }
            }
        }

        public override void OnExit()
        {
            if (this.scrollbar != null)
            {
                this.scrollbar.onValueChanged.RemoveListener(DoOnValueChanged);
            }
        }

        public void DoOnValueChanged(float _value)
        {
            this.value.Value = _value;
            Fsm.EventData.FloatData = _value;
            base.SendEvent(this.eventTarget, this.sendEvent);
        }
    }
}
