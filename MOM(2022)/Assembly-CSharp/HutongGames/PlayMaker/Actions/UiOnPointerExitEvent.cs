using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sends event when OnPointerExit is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
    public class UiOnPointerExitEvent : EventTriggerActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Event sent when PointerExit is called")]
        public FsmEvent onPointerExitEvent;

        public override void Reset()
        {
            base.Reset();
            this.onPointerExitEvent = null;
        }

        public override void OnEnter()
        {
            base.Init(EventTriggerType.PointerExit, OnPointerExitDelegate);
        }

        private void OnPointerExitDelegate(BaseEventData data)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
            base.SendEvent(base.eventTarget, this.onPointerExitEvent);
        }
    }
}
