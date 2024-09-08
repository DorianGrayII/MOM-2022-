using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnPointerUp is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnPointerUpEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when PointerUp is called")]
        public FsmEvent onPointerUpEvent;

        public override void Reset()
        {
            base.Reset();
            this.onPointerUpEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.PointerUp, OnPointerUpDelegate);
        }

        private void OnPointerUpDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onPointerUpEvent);
        }
    }
}
