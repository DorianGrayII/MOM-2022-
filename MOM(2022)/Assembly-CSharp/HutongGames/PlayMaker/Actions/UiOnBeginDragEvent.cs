using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when user starts to drag a GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnBeginDragEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnBeginDrag is called")]
        public FsmEvent onBeginDragEvent;

        public override void Reset()
        {
            base.Reset();
            this.onBeginDragEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.BeginDrag, OnBeginDragDelegate);
        }

        private void OnBeginDragDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onBeginDragEvent);
        }
    }
}
