namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("RectTransform"), HutongGames.PlayMaker.Tooltip("Set the local position of this RectTransform.")]
    public class RectTransformSetLocalPosition : BaseUpdateAction
    {
        [RequiredField, CheckForComponent(typeof(RectTransform)), HutongGames.PlayMaker.Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The position. Set to none for no effect")]
        public FsmVector2 position2d;
        [HutongGames.PlayMaker.Tooltip("Or the 3d position. Set to none for no effect")]
        public FsmVector3 position;
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
                Vector3 localPosition = this._rt.localPosition;
                if (!this.position.IsNone)
                {
                    localPosition = this.position.get_Value();
                }
                if (!this.position2d.IsNone)
                {
                    localPosition.x = this.position2d.get_Value().x;
                    localPosition.y = this.position2d.get_Value().y;
                }
                if (!this.x.IsNone)
                {
                    localPosition.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    localPosition.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    localPosition.z = this.z.Value;
                }
                this._rt.localPosition = localPosition;
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
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.position2d = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.position = vector2;
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

