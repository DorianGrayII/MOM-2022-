using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnDeselect is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnDeselectEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnDeselectEvent is called")]
        public FsmEvent onDeselectEvent;

        public override void Reset()
        {
            base.Reset();
            this.onDeselectEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.Deselect, OnDeselectDelegate);
        }

        private void OnDeselectDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onDeselectEvent);
        }
    }
}
