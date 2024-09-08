using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets pointer data Input Button on the last System event.")]
    public class UiGetLastPointerEventDataInputButton : FsmStateAction
    {
        [Tooltip("Store the Input Button pressed (Left, Right, Middle)")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(PointerEventData.InputButton))]
        public FsmEnum inputButton;

        [Tooltip("Event to send if Left Button clicked.")]
        public FsmEvent leftClick;

        [Tooltip("Event to send if Middle Button clicked.")]
        public FsmEvent middleClick;

        [Tooltip("Event to send if Right Button clicked.")]
        public FsmEvent rightClick;

        public override void Reset()
        {
            this.inputButton = PointerEventData.InputButton.Left;
            this.leftClick = null;
            this.middleClick = null;
            this.rightClick = null;
        }

        public override void OnEnter()
        {
            this.ExecuteAction();
            base.Finish();
        }

        private void ExecuteAction()
        {
            if (UiGetLastPointerDataInfo.lastPointerEventData != null)
            {
                if (!this.inputButton.IsNone)
                {
                    this.inputButton.Value = UiGetLastPointerDataInfo.lastPointerEventData.button;
                }
                if (!string.IsNullOrEmpty(this.leftClick.Name) && UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Left)
                {
                    base.Fsm.Event(this.leftClick);
                }
                else if (!string.IsNullOrEmpty(this.middleClick.Name) && UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Middle)
                {
                    base.Fsm.Event(this.middleClick);
                }
                else if (!string.IsNullOrEmpty(this.rightClick.Name) && UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Right)
                {
                    base.Fsm.Event(this.rightClick);
                }
            }
        }
    }
}
