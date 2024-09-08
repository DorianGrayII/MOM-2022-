namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when Called by the EventSystem when the object associated with this EventTrigger is updated.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnUpdateSelectedEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnUpdateSelected is called")]
        public FsmEvent onUpdateSelectedEvent;

        public override void OnEnter()
        {
            base.Init(EventTriggerType.UpdateSelected, new UnityAction<BaseEventData>(this.OnUpdateSelectedDelegate));
        }

        private void OnUpdateSelectedDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onUpdateSelectedEvent);
        }

        public override void Reset()
        {
            base.Reset();
            this.onUpdateSelectedEvent = null;
        }
    }
}

