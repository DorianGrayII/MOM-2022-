namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Gets the number of Touches.")]
    public class GetTouchCount : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the current number of touches in an Int Variable.")]
        public FsmInt storeCount;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetTouchCount()
        {
            this.storeCount.Value = Input.touchCount;
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

        public override void Reset()
        {
            this.storeCount = null;
            this.everyFrame = false;
        }
    }
}

