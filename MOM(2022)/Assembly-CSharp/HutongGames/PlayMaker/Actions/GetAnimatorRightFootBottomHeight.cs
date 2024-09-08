namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Get the right foot bottom height.")]
    public class GetAnimatorRightFootBottomHeight : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Result"), RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The right foot bottom height.")]
        public FsmFloat rightFootHeight;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame during LateUpdate. Useful when value is subject to change over time.")]
        public bool everyFrame;
        private Animator _animator;

        private void _getRightFootBottonHeight()
        {
            if (this._animator != null)
            {
                this.rightFootHeight.Value = this._animator.rightFeetBottomHeight;
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
                    this._getRightFootBottonHeight();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnLateUpdate()
        {
            this._getRightFootBottonHeight();
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.rightFootHeight = null;
            this.everyFrame = false;
        }
    }
}

