using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when Called by the EventSystem when a drag has been found, but before it is valid to begin the drag.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnInitializePotentialDragEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when OnInitializePotentialDrag is called")]
        public FsmEvent onInitializePotentialDragEvent;

        public override void Reset()
        {
            base.Reset();
            this.onInitializePotentialDragEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.InitializePotentialDrag, OnInitializePotentialDragDelegate);
        }

        private void OnInitializePotentialDragDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onInitializePotentialDragEvent);
        }
    }
}
