using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets the value of a int parameter")]
    public class SetAnimatorInt : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorInt)]
        [Tooltip("The animator parameter")]
        public FsmString parameter;

        [Tooltip("The Int value to assign to the animator parameter")]
        public FsmInt Value;

        private Animator _animator;

        private int _paramID;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.parameter = null;
            this.Value = null;
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
            this._paramID = Animator.StringToHash(this.parameter.Value);
            this.SetParameter();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.SetParameter();
        }

        private void SetParameter()
        {
            if (this._animator != null)
            {
                this._animator.SetInteger(this._paramID, this.Value.Value);
            }
        }
    }
}
