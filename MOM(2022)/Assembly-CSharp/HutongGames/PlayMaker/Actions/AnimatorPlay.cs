namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Plays a state. This could be used to synchronize your animation with audio or synchronize an Animator over the network.")]
    public class AnimatorPlay : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the state that will be played.")]
        public FsmString stateName;
        [HutongGames.PlayMaker.Tooltip("The layer where the state is.")]
        public FsmInt layer;
        [HutongGames.PlayMaker.Tooltip("The normalized time at which the state will play")]
        public FsmFloat normalizedTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when using normalizedTime to manually control the animation.")]
        public bool everyFrame;
        private Animator _animator;

        private void DoAnimatorPlay()
        {
            if (this._animator != null)
            {
                int layer = this.layer.IsNone ? -1 : this.layer.Value;
                this._animator.Play(this.stateName.Value, layer, this.normalizedTime.IsNone ? float.NegativeInfinity : this.normalizedTime.Value);
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
                this.DoAnimatorPlay();
                if (!this.everyFrame)
                {
                    base.Finish();
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoAnimatorPlay();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.stateName = null;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.layer = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.normalizedTime = num2;
            this.everyFrame = false;
        }
    }
}

