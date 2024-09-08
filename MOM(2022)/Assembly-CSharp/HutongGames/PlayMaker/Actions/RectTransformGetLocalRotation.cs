using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Gets the local rotation of this RectTransform.")]
    public class RectTransformGetLocalRotation : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The rotation")]
        public FsmVector3 rotation;

        [Tooltip("The x component of the rotation")]
        public FsmFloat x;

        [Tooltip("The y component of the rotation")]
        public FsmFloat y;

        [Tooltip("The z component of the rotation")]
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
            this.DoGetValues();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetValues();
        }

        private void DoGetValues()
        {
            if (!(this._rt == null))
            {
                if (!this.rotation.IsNone)
                {
                    this.rotation.Value = this._rt.eulerAngles;
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
    }
}
