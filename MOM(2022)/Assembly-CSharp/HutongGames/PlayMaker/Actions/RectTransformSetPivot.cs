namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The normalized position in this RectTransform that it rotates around.")]
    public class RectTransformSetPivot : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Vector2 pivot. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 pivot;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides pivot x value if set. Set to none for no effect")]
        public FsmFloat x;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides pivot y value if set. Set to none for no effect")]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoSetPivotPosition()
        {
            Vector2 pivot = this._rt.pivot;
            if (!this.pivot.IsNone)
            {
                pivot = this.pivot.get_Value();
            }
            if (!this.x.IsNone)
            {
                pivot.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                pivot.y = this.y.Value;
            }
            this._rt.pivot = pivot;
        }

        public override void OnActionUpdate()
        {
            this.DoSetPivotPosition();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetPivotPosition();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.pivot = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }
    }
}

