namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when OnDrop is called on the GameObject. Warning this event is sent everyframe while dragging.\n Use GetLastPointerDataInfo action to get info from the event.")]
    public class UiOnDropEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnDrop is called")]
        public FsmEvent onDropEvent;

        private void OnDropDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onDropEvent);
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Drop, new UnityAction<BaseEventData>(this.OnDropDelegate));
        }

        public override void Reset()
        {
            base.Reset();
            this.onDropEvent = null;
        }
    }
}

