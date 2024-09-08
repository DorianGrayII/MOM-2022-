namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Enables/Disables an Animation on a GameObject.\nAnimation time is paused while disabled. Animation must also have a non zero weight to play.")]
    public class EnableAnimation : BaseAnimationAction
    {
        [RequiredField, CheckForComponent(typeof(Animation)), HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Animation), HutongGames.PlayMaker.Tooltip("The name of the animation to enable/disable.")]
        public FsmString animName;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
        public FsmBool enable;
        [HutongGames.PlayMaker.Tooltip("Reset the initial enabled state when exiting the state.")]
        public FsmBool resetOnExit;
        private AnimationState anim;

        private void DoEnableAnimation(GameObject go)
        {
            if (base.UpdateCache(go))
            {
                this.anim = base.animation[this.animName.Value];
                if (this.anim != null)
                {
                    this.anim.enabled = this.enable.Value;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoEnableAnimation(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        public override void OnExit()
        {
            if (this.resetOnExit.Value && (this.anim != null))
            {
                this.anim.enabled = !this.enable.Value;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.enable = true;
            this.resetOnExit = false;
        }
    }
}

