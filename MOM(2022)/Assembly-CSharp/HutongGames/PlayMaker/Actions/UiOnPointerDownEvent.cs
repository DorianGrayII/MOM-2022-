namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when OnPointerDown is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnPointerDownEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when PointerDown is called")]
        public FsmEvent onPointerDownEvent;

        public override void OnEnter()
        {
            base.Init(EventTriggerType.PointerDown, new UnityAction<BaseEventData>(this.OnPointerDownDelegate));
        }

        private void OnPointerDownDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onPointerDownEvent);
        }

        public override void Reset()
        {
            base.Reset();
            this.onPointerDownEvent = null;
        }
    }
}

