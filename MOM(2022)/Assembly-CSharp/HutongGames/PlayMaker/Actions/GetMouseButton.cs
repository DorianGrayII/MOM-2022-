using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Input)]
    [Tooltip("Gets the pressed state of the specified Mouse Button and stores it in a Bool Variable. See Unity Input Manager doc.")]
    public class GetMouseButton : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The mouse button to test.")]
        public MouseButton button;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the pressed state in a Bool Variable.")]
        public FsmBool storeResult;

        public override void Reset()
        {
            this.button = MouseButton.Left;
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            this.storeResult.Value = Input.GetMouseButton((int)this.button);
        }

        public override void OnUpdate()
        {
            this.storeResult.Value = Input.GetMouseButton((int)this.button);
        }
    }
}
