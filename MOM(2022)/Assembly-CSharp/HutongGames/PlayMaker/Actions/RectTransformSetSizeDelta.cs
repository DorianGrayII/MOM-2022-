namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Set the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
    public class RectTransformSetSizeDelta : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("TheVector2 sizeDelta. Set to none for no effect, and/or set individual axis below.")]
        public FsmVector2 sizeDelta;
        [HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides sizeDelta x value if set. Set to none for no effect")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Setting only the x value. Overrides sizeDelta y value if set. Set to none for no effect")]
        public FsmFloat y;
        private RectTransform _rt;

        private void DoSetSizeDelta()
        {
            Vector2 sizeDelta = this._rt.sizeDelta;
            if (!this.sizeDelta.IsNone)
            {
                sizeDelta = this.sizeDelta.get_Value();
            }
            if (!this.x.IsNone)
            {
                sizeDelta.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                sizeDelta.y = this.y.Value;
            }
            this._rt.sizeDelta = sizeDelta;
        }

        public override void OnActionUpdate()
        {
            this.DoSetSizeDelta();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetSizeDelta();
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
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
        }
    }
}

