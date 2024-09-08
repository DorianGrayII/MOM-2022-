using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
    public class GetAnimatorDelta : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The avatar delta position for the last evaluated frame")]
        public FsmVector3 deltaPosition;

        [UIHint(UIHint.Variable)]
        [Tooltip("The avatar delta position for the last evaluated frame")]
        public FsmQuaternion deltaRotation;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.deltaPosition = null;
            this.deltaRotation = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            this._animator = ownerDefaultTarget.GetComponent<Animator>();
            if (this._animator == null)
            {
                base.Finish();
                return;
            }
            this.DoGetDeltaPosition();
            base.Finish();
        }

        public override void OnActionUpdate()
        {
            this.DoGetDeltaPosition();
        }

        private void DoGetDeltaPosition()
        {
            if (!(this._animator == null))
            {
                this.deltaPosition.Value = this._animator.deltaPosition;
                this.deltaRotation.Value = this._animator.deltaRotation;
            }
        }
    }
}
