using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnPointerClick is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnPointerClickEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when PointerClick is called")]
        public FsmEvent onPointerClickEvent;

        public override void Reset()
        {
            base.Reset();
            this.onPointerClickEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.PointerClick, OnPointerClickDelegate);
        }

        private void OnPointerClickDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onPointerClickEvent);
        }
    }
}
