using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the current transition information on a specified layer. Only valid when during a transition.")]
    public class GetAnimatorCurrentTransitionInfo : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The unique name of the Transition")]
        public FsmString name;

        [UIHint(UIHint.Variable)]
        [Tooltip("The unique name of the Transition")]
        public FsmInt nameHash;

        [UIHint(UIHint.Variable)]
        [Tooltip("The user-specified name of the Transition")]
        public FsmInt userNameHash;

        [UIHint(UIHint.Variable)]
        [Tooltip("Normalized time of the Transition")]
        public FsmFloat normalizedTime;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.layerIndex = null;
            this.name = null;
            this.nameHash = null;
            this.userNameHash = null;
            this.normalizedTime = null;
            base.everyFrame = false;
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
            this.GetTransitionInfo();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.GetTransitionInfo();
        }

        private void GetTransitionInfo()
        {
            if (this._animator != null)
            {
                AnimatorTransitionInfo animatorTransitionInfo = this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value);
                if (!this.name.IsNone)
                {
                    this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
                }
                if (!this.nameHash.IsNone)
                {
                    this.nameHash.Value = animatorTransitionInfo.nameHash;
                }
                if (!this.userNameHash.IsNone)
                {
                    this.userNameHash.Value = animatorTransitionInfo.userNameHash;
                }
                if (!this.normalizedTime.IsNone)
                {
                    this.normalizedTime.Value = animatorTransitionInfo.normalizedTime;
                }
            }
        }
    }
}
