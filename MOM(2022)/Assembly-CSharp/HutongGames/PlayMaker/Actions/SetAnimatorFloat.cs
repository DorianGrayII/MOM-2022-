namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the value of a float parameter")]
    public class SetAnimatorFloat : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.AnimatorFloat), HutongGames.PlayMaker.Tooltip("The animator parameter")]
        public FsmString parameter;
        [HutongGames.PlayMaker.Tooltip("The float value to assign to the animator parameter")]
        public FsmFloat Value;
        [HutongGames.PlayMaker.Tooltip("Optional: The time allowed to parameter to reach the value. Requires everyFrame Checked on")]
        public FsmFloat dampTime;
        private Animator _animator;
        private int _paramID;

        public override void OnActionUpdate()
        {
            this.SetParameter();
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
                    this._paramID = Animator.StringToHash(this.parameter.Value);
                    this.SetParameter();
                    if (!base.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.parameter = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.dampTime = num1;
            this.Value = null;
        }

        private void SetParameter()
        {
            if (this._animator != null)
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

