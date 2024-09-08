namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The calculated rectangle in the local space of the Transform.")]
    public class RectTransformGetRect : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The rect")]
        public FsmRect rect;
        [UIHint(UIHint.Variable)]
        public FsmFloat x;
        [UIHint(UIHint.Variable)]
        public FsmFloat y;
        [UIHint(UIHint.Variable)]
        public FsmFloat width;
        [UIHint(UIHint.Variable)]
        public FsmFloat height;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (!this.rect.IsNone)
            {
                this.rect.set_Value(this._rt.rect);
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.rect.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.rect.y;
            }
            if (!this.width.IsNone)
            {
                this.width.Value = this._rt.rect.width;
            }
            if (!this.height.IsNone)
            {
                this.height.Value = this._rt.rect.height;
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetValues();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoGetValues();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.rect = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.width = num3;
            FsmFloat num4 = new FsmFloat();
            num4.UseVariable = true;
            this.height = num4;
        }
    }
}

