namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when OnCancel is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnCancelEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnCancelEvent is called")]
        public FsmEvent onCancelEvent;

        private void OnCancelDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onCancelEvent);
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Cancel, new UnityAction<BaseEventData>(this.OnCancelDelegate));
        }

        public override void Reset()
        {
            base.gameObject = null;
            this.onCancelEvent = null;
        }
    }
}

