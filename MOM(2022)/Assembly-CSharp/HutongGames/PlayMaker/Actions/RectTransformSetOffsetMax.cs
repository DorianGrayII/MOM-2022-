namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("\tThe offset of the upper right corner of the rectangle relative to the upper right anchor.")]
    public class RectTransformSetOffsetMax : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Vector2 offsetMax. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 offsetMax;
        [HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides offsetMax x value if set. Set to none for no effect")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Setting only the y value. Overrides offsetMax y value if set. Set to none for no effect")]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoSetOffsetMax()
        {
            Vector2 offsetMax = this._rt.offsetMax;
            if (!this.offsetMax.IsNone)
            {
                offsetMax = this.offsetMax.get_Value();
            }
            if (!this.x.IsNone)
            {
                offsetMax.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                offsetMax.y = this.y.Value;
            }
            this._rt.offsetMax = offsetMax;
        }

        public override void OnActionUpdate()
        {
            this.DoSetOffsetMax();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetOffsetMax();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.offsetMax = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }
    }
}

