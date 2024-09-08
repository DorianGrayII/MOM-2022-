namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Get the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
    public class RectTransformGetSizeDelta : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The sizeDelta"), UIHint(UIHint.Variable)]
        public FsmVector2 sizeDelta;
        [HutongGames.PlayMaker.Tooltip("The x component of the sizeDelta, the width."), UIHint(UIHint.Variable)]
        public FsmFloat width;
        [HutongGames.PlayMaker.Tooltip("The y component of the sizeDelta, the height"), UIHint(UIHint.Variable)]
        public FsmFloat height;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (!this.sizeDelta.IsNone)
            {
                this.sizeDelta.set_Value(this._rt.sizeDelta);
            }
            if (!this.width.IsNone)
            {
                this.width.Value = this._rt.sizeDelta.x;
            }
            if (!this.height.IsNone)
            {
                this.height.Value = this._rt.sizeDelta.y;
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
            this.sizeDelta = null;
            this.width = null;
            this.height = null;
        }
    }
}

