namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Gets the local rotation of this RectTransform.")]
    public class RectTransformGetLocalRotation : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The rotation")]
        public FsmVector3 rotation;
        [HutongGames.PlayMaker.Tooltip("The x component of the rotation")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the rotation")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("The z component of the rotation")]
        public FsmFloat z;
        private RectTransform _rt;

        private void DoGetValues()
        {
            if (this._rt != null)
            {
                if (!this.rotation.IsNone)
                {
                    this.rotation.set_Value(this._rt.eulerAngles);
                }
                if (!this.x.IsNone)
                {
                    this.x.Value = this._rt.eulerAngles.x;
                }
                if (!this.y.IsNone)
                {
                    this.y.Value = this._rt.eulerAngles.y;
                }
                if (!this.z.IsNone)
                {
                    this.z.Value = this._rt.eulerAngles.z;
                }
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
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.rotation = vector1;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.z = num3;
        }
    }
}

