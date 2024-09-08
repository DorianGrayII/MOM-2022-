namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The offset of the lower left corner of the rectangle relative to the lower left anchor.")]
    public class RectTransformSetOffsetMin : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Vector2 offsetMin. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 offsetMin;
        [HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides offsetMin x value if set. Set to none for no effect")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides offsetMin y value if set. Set to none for no effect")]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoSetOffsetMin()
        {
            Vector2 offsetMin = this._rt.offsetMin;
            if (!this.offsetMin.IsNone)
            {
                offsetMin = this.offsetMin.get_Value();
            }
            if (!this.x.IsNone)
            {
                offsetMin.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                offsetMin.y = this.y.Value;
            }
            this._rt.offsetMin = offsetMin;
        }

        public override void OnActionUpdate()
        {
            this.DoSetOffsetMin();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetOffsetMin();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.offsetMin = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }
    }
}

