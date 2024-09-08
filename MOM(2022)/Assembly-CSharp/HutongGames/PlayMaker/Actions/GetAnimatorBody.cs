using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the avatar body mass center position and rotation. Optionally accepts a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
    public class GetAnimatorBody : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The avatar body mass center")]
        public FsmVector3 bodyPosition;

        [UIHint(UIHint.Variable)]
        [Tooltip("The avatar body mass center")]
        public FsmQuaternion bodyRotation;

        [Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
        public FsmGameObject bodyGameObject;

        private Animator _animator;

        private Transform _transform;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.bodyPosition = null;
            this.bodyRotation = null;
            this.bodyGameObject = null;
            base.everyFrame = false;
            base.everyFrameOption = AnimatorFrameUpdateSelector.OnAnimatorIK;
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
            GameObject value = this.bodyGameObject.Value;
            if (value != null)
            {
                this._transform = value.transform;
            }
            if (base.everyFrameOption != AnimatorFrameUpdateSelector.OnAnimatorIK)
            {
                base.everyFrameOption = AnimatorFrameUpdateSelector.OnAnimatorIK;
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetBodyPosition();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoGetBodyPosition()
        {
            if (!(this._animator == null))
            {
                this.bodyPosition.Value = this._animator.bodyPosition;
                this.bodyRotation.Value = this._animator.bodyRotation;
                if (this._transform != null)
                {
                    this._transform.position = this._animator.bodyPosition;
                    this._transform.rotation = this._animator.bodyRotation;
                }
            }
        }

        public override string ErrorCheck()
        {
            if (base.everyFrameOption != AnimatorFrameUpdateSelector.OnAnimatorIK)
            {
                return "Getting Body Position should only be done in OnAnimatorIK";
            }
            return string.Empty;
        }
    }
}
