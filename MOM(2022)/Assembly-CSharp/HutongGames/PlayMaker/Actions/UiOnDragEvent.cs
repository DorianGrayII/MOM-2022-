namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when OnDrag is called on the GameObject. Warning this event is sent every frame while dragging.\n Use GetLastPointerDataInfo action to get info from the event.")]
    public class UiOnDragEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnDrag is called")]
        public FsmEvent onDragEvent;

        private void OnDragDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onDragEvent);
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Drag, new UnityAction<BaseEventData>(this.OnDragDelegate));
        }

        public override void Reset()
        {
            base.Reset();
            this.onDragEvent = null;
        }
    }
}

