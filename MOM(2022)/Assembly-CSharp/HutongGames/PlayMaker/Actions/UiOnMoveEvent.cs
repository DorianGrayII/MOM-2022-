using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnMoveEvent is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnMoveEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnMoveEvent is called")]
        public FsmEvent onMoveEvent;

        public override void Reset()
        {
            base.Reset();
            this.onMoveEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Move, OnMoveDelegate);
        }

        private void OnMoveDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onMoveEvent);
        }
    }
}
