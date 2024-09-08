using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets the value of a float parameter")]
    public class SetAnimatorFloat : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorFloat)]
        [Tooltip("The animator parameter")]
        public FsmString parameter;

        [Tooltip("The float value to assign to the animator parameter")]
        public FsmFloat Value;

        [Tooltip("Optional: The time allowed to parameter to reach the value. Requires everyFrame Checked on")]
        public FsmFloat dampTime;

        private Animator _animator;

        private int _paramID;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.parameter = null;
            this.dampTime = new FsmFloat
            {
                UseVariable = true
            };
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
            if (!(this._animator == null))
            {
                if (this.dampTime.Value > 0f)
                {
                    this._animator.SetFloat(this._paramID, this.Value.Value, this.dampTime.Value, Time.deltaTime);
                }
                else
                {
                    this._animator.SetFloat(this._paramID, this.Value.Value);
                }
            }
        }
    }
}
