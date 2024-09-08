namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when Called by the EventSystem when a Select event occurs.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnSelectEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnSelect is called")]
        public FsmEvent onSelectEvent;

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Select, new UnityAction<BaseEventData>(this.OnSelectDelegate));
        }

        private void OnSelectDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onSelectEvent);
        }

        public override void Reset()
        {
            base.Reset();
            this.onSelectEvent = null;
        }
    }
}

