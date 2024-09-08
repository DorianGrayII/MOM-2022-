namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event Called by the EventSystem once dragging ends.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnEndDragEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnEndDrag is called")]
        public FsmEvent onEndDragEvent;

        private void OnEndDragDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onEndDragEvent);
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.EndDrag, new UnityAction<BaseEventData>(this.OnEndDragDelegate));
        }

        public override void Reset()
        {
            base.Reset();
            this.onEndDragEvent = null;
        }
    }
}

