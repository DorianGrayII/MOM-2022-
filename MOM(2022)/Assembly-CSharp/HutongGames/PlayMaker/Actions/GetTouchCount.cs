using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Device)]
    [Tooltip("Gets the number of Touches.")]
    public class GetTouchCount : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the current number of touches in an Int Variable.")]
        public FsmInt storeCount;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.storeCount = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetTouchCount();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetTouchCount();
        }

        private void DoGetTouchCount()
        {
            this.storeCount.Value = Input.touchCount;
        }
    }
}
