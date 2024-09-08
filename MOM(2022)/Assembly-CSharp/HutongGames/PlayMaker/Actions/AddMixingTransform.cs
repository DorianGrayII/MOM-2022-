namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Play an animation on a subset of the hierarchy. E.g., A waving animation on the upper body.")]
    public class AddMixingTransform : BaseAnimationAction
    {
        [RequiredField, CheckForComponent(typeof(Animation)), HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The name of the animation to mix. NOTE: The animation should already be added to the Animation Component on the GameObject.")]
        public FsmString animationName;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The mixing transform. E.g., root/upper_body/left_shoulder")]
        public FsmString transform;
        [HutongGames.PlayMaker.Tooltip("If recursive is true all children of the mix transform will also be animated.")]
        public FsmBool recursive;

        private void DoAddMixingTransform()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                AnimationState state = base.animation[this.animationName.Value];
                if (state != null)
                {
                    state.AddMixingTransform(ownerDefaultTarget.transform.Find(this.transform.Value), this.recursive.Value);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoAddMixingTransform();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.animationName = "";
            this.transform = "";
            this.recursive = true;
        }
    }
}

