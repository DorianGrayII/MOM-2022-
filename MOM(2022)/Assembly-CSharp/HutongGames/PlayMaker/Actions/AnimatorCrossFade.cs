namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Create a dynamic transition between the current state and the destination state. Both states have to be on the same layer. Note: You cannot change the current state on a synchronized layer, you need to change it on the referenced layer.")]
    public class AnimatorCrossFade : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the state that will be played.")]
        public FsmString stateName;
        [HutongGames.PlayMaker.Tooltip("The duration of the transition. Value is in source state normalized time.")]
        public FsmFloat transitionDuration;
        [HutongGames.PlayMaker.Tooltip("Layer index containing the destination state. Leave to none to ignore")]
        public FsmInt layer;
        [HutongGames.PlayMaker.Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
        public FsmFloat normalizedTime;
        private Animator _animator;

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
                if (this._animator != null)
                {
                    int layer = this.layer.IsNone ? -1 : this.layer.Value;
                    this._animator.CrossFade(this.stateName.Value, this.transitionDuration.Value, layer, this.normalizedTime.IsNone ? float.NegativeInfinity : this.normalizedTime.Value);
                }
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.stateName = null;
            this.transitionDuration = 1f;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.layer = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.normalizedTime = num2;
        }
    }
}

