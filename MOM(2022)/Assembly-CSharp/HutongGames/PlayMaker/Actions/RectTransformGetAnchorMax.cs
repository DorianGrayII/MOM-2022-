namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Get the normalized position in the parent RectTransform that the upper right corner is anchored to.")]
    public class RectTransformGetAnchorMax : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The anchorMax"), UIHint(UIHint.Variable)]
        public FsmVector2 anchorMax;
        [HutongGames.PlayMaker.Tooltip("The x component of the anchorMax"), UIHint(UIHint.Variable)]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the anchorMax"), UIHint(UIHint.Variable)]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (!this.anchorMax.IsNone)
            {
                this.anchorMax.set_Value(this._rt.anchorMax);
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.anchorMax.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.anchorMax.y;
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
            this.anchorMax = null;
            this.x = null;
            this.y = null;
        }
    }
}

