namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Get the normalized position in this RectTransform that it rotates around.")]
    public class RectTransformGetPivot : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The pivot"), UIHint(UIHint.Variable)]
        public FsmVector2 pivot;
        [HutongGames.PlayMaker.Tooltip("The x component of the pivot"), UIHint(UIHint.Variable)]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the pivot"), UIHint(UIHint.Variable)]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (!this.pivot.IsNone)
            {
                this.pivot.set_Value(this._rt.pivot);
            }
            if (!this.x.IsNone)
            {
                this.x.Value = this._rt.pivot.x;
            }
            if (!this.y.IsNone)
            {
                this.y.Value = this._rt.pivot.y;
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
            this.pivot = null;
            this.x = null;
            this.y = null;
        }
    }
}

