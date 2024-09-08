using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
    public class GetAnimatorRoot : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The avatar body mass center")]
        public FsmVector3 rootPosition;

        [UIHint(UIHint.Variable)]
        [Tooltip("The avatar body mass center")]
        public FsmQuaternion rootRotation;

        [Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
        public FsmGameObject bodyGameObject;

        private Animator _animator;

        private Transform _transform;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.rootPosition = null;
            this.rootRotation = null;
            this.bodyGameObject = null;
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
            this.DoGetBodyPosition();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetBodyPosition();
        }

        private void DoGetBodyPosition()
        {
            if (!(this._animator == null))
            {
                this.rootPosition.Value = this._animator.rootPosition;
                this.rootRotation.Value = this._animator.rootRotation;
                if (this._transform != null)
                {
                    this._transform.position = this._animator.rootPosition;
                    this._transform.rotation = this._animator.rootRotation;
                }
            }
        }
    }
}
