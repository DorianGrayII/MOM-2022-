using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when Called by the EventSystem when the object associated with this EventTrigger is updated.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnUpdateSelectedEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnUpdateSelected is called")]
        public FsmEvent onUpdateSelectedEvent;

        public override void Reset()
        {
            base.Reset();
            this.onUpdateSelectedEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.UpdateSelected, OnUpdateSelectedDelegate);
        }

        private void OnUpdateSelectedDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onUpdateSelectedEvent);
        }
    }
}
