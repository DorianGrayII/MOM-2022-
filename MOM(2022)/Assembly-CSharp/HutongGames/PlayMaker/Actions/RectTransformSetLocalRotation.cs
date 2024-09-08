namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Set the local rotation of this RectTransform.")]
    public class RectTransformSetLocalRotation : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The rotation. Set to none for no effect")]
        public FsmVector3 rotation;
        [HutongGames.PlayMaker.Tooltip("The x component of the rotation. Set to none for no effect")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component of the rotation. Set to none for no effect")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("The z component of the rotation. Set to none for no effect")]
        public FsmFloat z;
        private RectTransform _rt;

        private void DoSetValues()
        {
            if (this._rt != null)
            {
                Vector3 eulerAngles = this._rt.eulerAngles;
                if (!this.rotation.IsNone)
                {
                    eulerAngles = this.rotation.get_Value();
                }
                if (!this.x.IsNone)
                {
                    eulerAngles.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    eulerAngles.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    eulerAngles.z = this.z.Value;
                }
                this._rt.eulerAngles = eulerAngles;
            }
        }

        public override void OnActionUpdate()
        {
            this.DoSetValues();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
            }
            this.DoSetValues();
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

