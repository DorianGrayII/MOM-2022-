namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), Tooltip("Sends event when OnSubmit is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnSubmitEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable), Tooltip("Event sent when OnSubmitEvent is called")]
        public FsmEvent onSubmitEvent;

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Submit, new UnityAction<BaseEventData>(this.OnSubmitDelegate));
        }

        private void OnSubmitDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData) data;
            base.SendEvent(base.eventTarget, this.onSubmitEvent);
        }

        public override void Reset()
        {
            base.Reset();
            this.onSubmitEvent = null;
        }
    }
}

