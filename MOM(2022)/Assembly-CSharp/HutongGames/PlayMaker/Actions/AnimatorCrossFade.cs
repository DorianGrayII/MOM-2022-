using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Create a dynamic transition between the current state and the destination state. Both states have to be on the same layer. Note: You cannot change the current state on a synchronized layer, you need to change it on the referenced layer.")]
    public class AnimatorCrossFade : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The name of the state that will be played.")]
        public FsmString stateName;

        [Tooltip("The duration of the transition. Value is in source state normalized time.")]
        public FsmFloat transitionDuration;

        [Tooltip("Layer index containing the destination state. Leave to none to ignore")]
        public FsmInt layer;

        [Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
        public FsmFloat normalizedTime;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.stateName = null;
            this.transitionDuration = 1f;
            this.layer = new FsmInt
            {
                UseVariable = true
            };
            this.normalizedTime = new FsmFloat
            {
                UseVariable = true
            };
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
            if (this._animator != null)
            {
                int num = (this.layer.IsNone ? (-1) : this.layer.Value);
                float normalizedTimeOffset = (this.normalizedTime.IsNone ? float.NegativeInfinity : this.normalizedTime.Value);
                this._animator.CrossFade(this.stateName.Value, this.transitionDuration.Value, num, normalizedTimeOffset);
            }
            base.Finish();
        }
    }
}
