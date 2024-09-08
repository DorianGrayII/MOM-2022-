using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
    public class SetAnimatorBody : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The gameObject target of the ik goal")]
        public FsmGameObject target;

        [Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
        public FsmVector3 position;

        [Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
        public FsmQuaternion rotation;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private Animator _animator;

        private Transform _transform;

        public override void Reset()
        {
            this.gameObject = null;
            this.target = null;
            this.position = new FsmVector3
            {
                UseVariable = true
            };
            this.rotation = new FsmQuaternion
            {
                UseVariable = true
            };
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleAnimatorIK = true;
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
            GameObject value = this.target.Value;
            if (value != null)
            {
                this._transform = value.transform;
            }
        }

        public override void DoAnimatorIK(int layerIndex)
        {
            this.DoSetBody();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetBody()
        {
            if (this._animator == null)
            {
                return;
            }
            if (this._transform != null)
            {
                if (this.position.IsNone)
                {
                    this._animator.bodyPosition = this._transform.position;
                }
                else
                {
                    this._animator.bodyPosition = this._transform.position + this.position.Value;
                }
                if (this.rotation.IsNone)
                {
                    this._animator.bodyRotation = this._transform.rotation;
                }
                else
                {
                    this._animator.bodyRotation = this._transform.rotation * this.rotation.Value;
                }
            }
            else
            {
                if (!this.position.IsNone)
                {
                    this._animator.bodyPosition = this.position.Value;
                }
                if (!this.rotation.IsNone)
                {
                    this._animator.bodyRotation = this.rotation.Value;
                }
            }
        }
    }
}
