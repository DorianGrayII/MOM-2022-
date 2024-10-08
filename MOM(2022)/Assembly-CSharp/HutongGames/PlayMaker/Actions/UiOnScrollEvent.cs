using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnScroll is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnScrollEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnScroll is called")]
        public FsmEvent onScrollEvent;

        public override void Reset()
        {
            base.Reset();
            this.onScrollEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Scroll, OnScrollDelegate);
        }

        private void OnScrollDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onScrollEvent);
        }
    }
}
