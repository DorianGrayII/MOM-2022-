namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
    public class RectTransformSetAnchorMinAndMax : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Vector2 anchor max. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMax;
        [HutongGames.PlayMaker.Tooltip("The Vector2 anchor min. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMin;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat xMax;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
        public FsmFloat yMax;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
        public FsmFloat xMin;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
        public FsmFloat yMin;
        private RectTransform _rt;

        private void DoSetAnchorMax()
        {
            Vector2 anchorMax = this._rt.anchorMax;
            Vector2 anchorMin = this._rt.anchorMin;
            if (!this.anchorMax.IsNone)
            {
                anchorMax = this.anchorMax.get_Value();
                anchorMin = this.anchorMin.get_Value();
            }
            if (!this.xMax.IsNone)
            {
                anchorMax.x = this.xMax.Value;
            }
            if (!this.yMax.IsNone)
            {
                anchorMax.y = this.yMax.Value;
            }
            if (!this.xMin.IsNone)
            {
                anchorMin.x = this.xMin.Value;
            }
            if (!this.yMin.IsNone)
            {
                anchorMin.y = this.yMin.Value;
            }
            this._rt.anchorMax = anchorMax;
            this._rt.anchorMin = anchorMin;
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
            this.anchorMin = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.xMax = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.yMax = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.xMin = num3;
            FsmFloat num4 = new FsmFloat();
            num4.UseVariable = true;
            this.yMin = num4;
        }
    }
}

