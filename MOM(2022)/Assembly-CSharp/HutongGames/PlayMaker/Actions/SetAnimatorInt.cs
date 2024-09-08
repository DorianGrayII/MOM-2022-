namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the value of a int parameter")]
    public class SetAnimatorInt : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.AnimatorInt), HutongGames.PlayMaker.Tooltip("The animator parameter")]
        public FsmString parameter;
        [HutongGames.PlayMaker.Tooltip("The Int value to assign to the animator parameter")]
        public FsmInt Value;
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
            this.Value = null;
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

