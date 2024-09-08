using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the position and rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).\nThe position and rotation are only valid when a frame has being evaluated after the SetTarget call")]
    public class GetAnimatorTarget : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The target position")]
        public FsmVector3 targetPosition;

        [UIHint(UIHint.Variable)]
        [Tooltip("The target rotation")]
        public FsmQuaternion targetRotation;

        [Tooltip("If set, apply the position and rotation to this gameObject")]
        public FsmGameObject targetGameObject;

        private Animator _animator;

        private Transform _transform;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.targetPosition = null;
            this.targetRotation = null;
            this.targetGameObject = null;
            base.everyFrame = false;
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
            GameObject value = this.targetGameObject.Value;
            if (value != null)
            {
                this._transform = value.transform;
            }
            this.DoGetTarget();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetTarget();
        }

        private void DoGetTarget()
        {
            if (!(this._animator == null))
            {
                this.targetPosition.Value = this._animator.targetPosition;
                this.targetRotation.Value = this._animator.targetRotation;
                if (this._transform != null)
                {
                    this._transform.position = this._animator.targetPosition;
                    this._transform.rotation = this._animator.targetRotation;
                }
            }
        }
    }
}
