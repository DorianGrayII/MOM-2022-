namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Get the normalized position in the parent RectTransform that the lower left corner is anchored to.")]
    public class RectTransformGetAnchorMin : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The anchorMin"), UIHint(UIHint.Variable)]
        public FsmVector2 anchorMin;
        [HutongGames.PlayMaker.Tooltip("The x component of the anchorMin"), UIHint(UIHint.Variable)]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the anchorMin"), UIHint(UIHint.Variable)]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (!this.anchorMin.IsNone)
            {
                this.anchorMin.set_Value(this._rt.anchorMin);
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.anchorMin.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.anchorMin.y;
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
            this.anchorMin = null;
            this.x = null;
            this.y = null;
        }
    }
}

