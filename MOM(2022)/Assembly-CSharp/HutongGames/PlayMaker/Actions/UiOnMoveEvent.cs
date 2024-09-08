namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when OnMoveEvent is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnMoveEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnMoveEvent is called")]
        public FsmEvent onMoveEvent;

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Move, new UnityAction<BaseEventData>(this.OnMoveDelegate));
        }

        private void OnMoveDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onMoveEvent);
        }

        public override void Reset()
        {
            base.Reset();
            this.onMoveEvent = null;
        }
    }
}

