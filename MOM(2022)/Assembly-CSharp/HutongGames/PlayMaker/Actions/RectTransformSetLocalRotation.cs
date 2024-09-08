using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Set the local rotation of this RectTransform.")]
    public class RectTransformSetLocalRotation : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The rotation. Set to none for no effect")]
        public FsmVector3 rotation;

        [Tooltip("The x component of the rotation. Set to none for no effect")]
        public FsmFloat x;

        [Tooltip("The y component of the rotation. Set to none for no effect")]
        public FsmFloat y;

        [Tooltip("The z component of the rotation. Set to none for no effect")]
        public FsmFloat z;

        private RectTransform _rt;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.rotation = new FsmVector3
            {
                UseVariable = true
            };
            this.x = new FsmFloat
            {
                UseVariable = true
            };
            this.y = new FsmFloat
            {
                UseVariable = true
            };
            this.z = new FsmFloat
            {
                UseVariable = true
            };
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

        public override void OnActionUpdate()
        {
            this.DoSetValues();
        }

        private void DoSetValues()
        {
            if (!(this._rt == null))
            {
                Vector3 eulerAngles = this._rt.eulerAngles;
                if (!this.rotation.IsNone)
                {
                    eulerAngles = this.rotation.Value;
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
    }
}
