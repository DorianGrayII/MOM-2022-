namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
    public class RectTransformSetAnchorMax : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Vector2 anchor. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMax;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat x;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoSetAnchorMax()
        {
            Vector2 anchorMax = this._rt.anchorMax;
            if (!this.anchorMax.IsNone)
            {
                anchorMax = this.anchorMax.get_Value();
            }
            if (!this.x.IsNone)
            {
                anchorMax.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                anchorMax.y = this.y.Value;
            }
            this._rt.anchorMax = anchorMax;
        }

        public override void OnActionUpdate()
        {
            this.DoSetAnchorMax();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetAnchorMax();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.anchorMax = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }
    }
}

