namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when user starts to drag a GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnBeginDragEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnBeginDrag is called")]
        public FsmEvent onBeginDragEvent;

        private void OnBeginDragDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onBeginDragEvent);
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.BeginDrag, new UnityAction<BaseEventData>(this.OnBeginDragDelegate));
        }

        public override void Reset()
        {
            base.Reset();
            this.onBeginDragEvent = null;
        }
    }
}

