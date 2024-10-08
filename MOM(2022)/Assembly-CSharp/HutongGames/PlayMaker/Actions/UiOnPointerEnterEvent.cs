using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnPointerEnter is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnPointerEnterEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when PointerEnter is called")]
        public FsmEvent onPointerEnterEvent;

        public override void Reset()
        {
            base.Reset();
            this.onPointerEnterEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.PointerEnter, OnPointerEnterDelegate);
        }

        private void OnPointerEnterDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onPointerEnterEvent);
        }
    }
}
