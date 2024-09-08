namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Get the offset of the upper right corner of the rectangle relative to the upper right anchor")]
    public class RectTransformGetOffsetMax : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The offsetMax"), UIHint(UIHint.Variable)]
        public FsmVector2 offsetMax;
        [HutongGames.PlayMaker.Tooltip("The x component of the offsetMax"), UIHint(UIHint.Variable)]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the offsetMax"), UIHint(UIHint.Variable)]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (!this.offsetMax.IsNone)
            {
                this.offsetMax.set_Value(this._rt.offsetMax);
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.offsetMax.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.offsetMax.y;
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
            this.offsetMax = null;
            this.x = null;
            this.y = null;
        }
    }
}

