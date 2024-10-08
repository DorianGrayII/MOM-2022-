using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnPointerDown is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnPointerDownEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when PointerDown is called")]
        public FsmEvent onPointerDownEvent;

        public override void Reset()
        {
            base.Reset();
            this.onPointerDownEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.PointerDown, OnPointerDownDelegate);
        }

        private void OnPointerDownDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onPointerDownEvent);
        }
    }
}
