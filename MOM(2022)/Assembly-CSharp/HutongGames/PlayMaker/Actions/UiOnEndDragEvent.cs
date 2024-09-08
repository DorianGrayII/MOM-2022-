using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event Called by the EventSystem once dragging ends.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnEndDragEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnEndDrag is called")]
        public FsmEvent onEndDragEvent;

        public override void Reset()
        {
            base.Reset();
            this.onEndDragEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.EndDrag, OnEndDragDelegate);
        }

        private void OnEndDragDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onEndDragEvent);
        }
    }
}
