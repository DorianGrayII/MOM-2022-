using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the value of a bool parameter")]
    public class GetAnimatorBool : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorBool)]
        [Tooltip("The animator parameter")]
        public FsmString parameter;

        [ActionSection("Results")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The bool value of the animator parameter")]
        public FsmBool result;

        private Animator _animator;

        private int _paramID;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.parameter = null;
            this.result = null;
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
            this.GetParameter();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.GetParameter();
        }

        private void GetParameter()
        {
            if (this._animator != null)
            {
                this.result.Value = this._animator.GetBool(this._paramID);
            }
        }
    }
}
