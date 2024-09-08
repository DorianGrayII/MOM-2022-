namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("The normalized position in the parent RectTransform that the lower left corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
    public class RectTransformSetAnchorMin : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Vector2 anchor. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 anchorMin;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
        public FsmFloat x;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoSetAnchorMin()
        {
            Vector2 anchorMin = this._rt.anchorMin;
            if (!this.anchorMin.IsNone)
            {
                anchorMin = this.anchorMin.get_Value();
            }
            if (!this.x.IsNone)
            {
                anchorMin.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                anchorMin.y = this.y.Value;
            }
            this._rt.anchorMin = anchorMin;
        }

        public override void OnActionUpdate()
        {
            this.DoSetAnchorMin();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetAnchorMin();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.anchorMin = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }
    }
}

