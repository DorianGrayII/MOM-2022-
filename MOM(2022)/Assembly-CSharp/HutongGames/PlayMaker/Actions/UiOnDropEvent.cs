using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnDrop is called on the GameObject. Warning this event is sent everyframe while dragging.\n Use GetLastPointerDataInfo action to get info from the event.")]
    public class UiOnDropEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnDrop is called")]
        public FsmEvent onDropEvent;

        public override void Reset()
        {
            base.Reset();
            this.onDropEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Drop, OnDropDelegate);
        }

        private void OnDropDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onDropEvent);
        }
    }
}
