namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
    public class GetAnimatorDelta : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The avatar delta position for the last evaluated frame")]
        public FsmVector3 deltaPosition;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The avatar delta position for the last evaluated frame")]
        public FsmQuaternion deltaRotation;
        private Animator _animator;

        private void DoGetDeltaPosition()
        {
            if (this._animator != null)
            {
                this.deltaPosition.set_Value(this._animator.deltaPosition);
                this.deltaRotation.set_Value(this._animator.deltaRotation);
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetDeltaPosition();
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if (this._animator == null)
                {
                    base.Finish();
                }
                else
                {
                    this.DoGetDeltaPosition();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.deltaPosition = null;
            this.deltaRotation = null;
        }
    }
}

