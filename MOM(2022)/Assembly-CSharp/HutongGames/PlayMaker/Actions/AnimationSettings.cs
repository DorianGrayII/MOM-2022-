namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Set the Wrap Mode, Blend Mode, Layer and Speed of an Animation.\nNOTE: Settings are applied once, on entering the state, NOT continuously. To dynamically control an animation's settings, use Set Animation Speed etc.")]
    public class AnimationSettings : BaseAnimationAction
    {
        [RequiredField, CheckForComponent(typeof(Animation)), HutongGames.PlayMaker.Tooltip("A GameObject with an Animation Component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Animation), HutongGames.PlayMaker.Tooltip("The name of the animation.")]
        public FsmString animName;
        [HutongGames.PlayMaker.Tooltip("The behavior of the animation when it wraps.")]
        public WrapMode wrapMode;
        [HutongGames.PlayMaker.Tooltip("How the animation is blended with other animations on the Game Object.")]
        public AnimationBlendMode blendMode;
        [HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("The speed of the animation. 1 = normal; 2 = double speed...")]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("The animation layer")]
        public FsmInt layer;

        private void DoAnimationSettings()
        {
            if (!string.IsNullOrEmpty(this.animName.Value))
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (base.UpdateCache(ownerDefaultTarget))
                {
                    AnimationState state = base.animation[this.animName.Value];
                    if (state == null)
                    {
                        base.LogWarning("Missing animation: " + this.animName.Value);
                    }
                    else
                    {
                        state.wrapMode = this.wrapMode;
                        state.blendMode = this.blendMode;
                        if (!this.layer.IsNone)
                        {
                            state.layer = this.layer.Value;
                        }
                        if (!this.speed.IsNone)
                        {
                            state.speed = this.speed.Value;
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoAnimationSettings();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.wrapMode = WrapMode.Loop;
            this.blendMode = AnimationBlendMode.Blend;
            this.speed = 1f;
            this.layer = 0;
        }
    }
}

