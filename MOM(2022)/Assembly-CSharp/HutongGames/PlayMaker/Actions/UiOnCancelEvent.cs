using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnCancel is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnCancelEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnCancelEvent is called")]
        public FsmEvent onCancelEvent;

        public override void Reset()
        {
            base.gameObject = null;
            this.onCancelEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Cancel, OnCancelDelegate);
        }

        private void OnCancelDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onCancelEvent);
        }
    }
}
