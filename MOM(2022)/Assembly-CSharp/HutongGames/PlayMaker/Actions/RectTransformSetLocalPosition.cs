using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("RectTransform")]
    [Tooltip("Set the local position of this RectTransform.")]
    public class RectTransformSetLocalPosition : BaseUpdateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(RectTransform))]
        [Tooltip("The GameObject target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The position. Set to none for no effect")]
        public FsmVector2 position2d;

        [Tooltip("Or the 3d position. Set to none for no effect")]
        public FsmVector3 position;

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
            this.position2d = new FsmVector2
            {
                UseVariable = true
            };
            this.position = new FsmVector3
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
                Vector3 localPosition = this._rt.localPosition;
                if (!this.position.IsNone)
                {
                    localPosition = this.position.Value;
                }
                if (!this.position2d.IsNone)
                {
                    localPosition.x = this.position2d.Value.x;
                    localPosition.y = this.position2d.Value.y;
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
    }
}
