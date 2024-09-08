using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Checks if Pointer is over a UI object, optionally takes a pointer ID, otherwise uses the current event.")]
    public class UiIsPointerOverUiObject : FsmStateAction
    {
        [Tooltip("Optional PointerId. Leave to none to use the current event")]
        public FsmInt pointerId;

        [Tooltip("Event to send when the Pointer is over an UI object.")]
        public FsmEvent pointerOverUI;

        [Tooltip("Event to send when the Pointer is NOT over an UI object.")]
        public FsmEvent pointerNotOverUI;

        [UIHint(UIHint.Variable)]
        public FsmBool isPointerOverUI;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.pointerId = new FsmInt
            {
                UseVariable = true
            };
            this.pointerOverUI = null;
            this.pointerNotOverUI = null;
            this.isPointerOverUI = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoCheckPointer();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoCheckPointer();
        }

        private void DoCheckPointer()
        {
            bool flag = false;
            if (this.pointerId.IsNone)
            {
                flag = EventSystem.current.IsPointerOverGameObject();
            }
            else if (EventSystem.current.currentInputModule is PointerInputModule)
            {
                flag = (EventSystem.current.currentInputModule as PointerInputModule).IsPointerOverGameObject(this.pointerId.Value);
            }
            this.isPointerOverUI.Value = flag;
            base.Fsm.Event(flag ? this.pointerOverUI : this.pointerNotOverUI);
        }
    }
}
