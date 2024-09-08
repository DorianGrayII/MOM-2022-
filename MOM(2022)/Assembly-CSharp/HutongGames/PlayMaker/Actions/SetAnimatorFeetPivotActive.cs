namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
    public class SetAnimatorFeetPivotActive : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
        public FsmFloat feetPivotActive;
        private Animator _animator;

        private void DoFeetPivotActive()
        {
            if (this._animator != null)
            {
                this._animator.feetPivotActive = this.feetPivotActive.Value;
            }
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
                    this.DoFeetPivotActive();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.feetPivotActive = null;
        }
    }
}

