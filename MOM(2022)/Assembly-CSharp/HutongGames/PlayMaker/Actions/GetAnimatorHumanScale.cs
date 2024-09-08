namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).\n The scale is relative to Unity's Default Avatar")]
    public class GetAnimatorHumanScale : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Result"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("the scale of the current Avatar")]
        public FsmFloat humanScale;
        private Animator _animator;

        private void DoGetHumanScale()
        {
            if (this._animator != null)
            {
                this.humanScale.Value = this._animator.humanScale;
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
                    this.DoGetHumanScale();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.humanScale = null;
        }
    }
}

