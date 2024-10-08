using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [Tooltip("Play an animation on a subset of the hierarchy. E.g., A waving animation on the upper body.")]
    public class AddMixingTransform : BaseAnimationAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animation))]
        [Tooltip("The GameObject playing the animation.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The name of the animation to mix. NOTE: The animation should already be added to the Animation Component on the GameObject.")]
        public FsmString animationName;

        [RequiredField]
        [Tooltip("The mixing transform. E.g., root/upper_body/left_shoulder")]
        public FsmString transform;

        [Tooltip("If recursive is true all children of the mix transform will also be animated.")]
        public FsmBool recursive;

        public override void Reset()
        {
            this.gameObject = null;
            this.animationName = "";
            this.transform = "";
            this.recursive = true;
        }

        public override void OnEnter()
        {
            this.DoAddMixingTransform();
            base.Finish();
        }

        private void DoAddMixingTransform()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                AnimationState animationState = base.animation[this.animationName.Value];
                if (!(animationState == null))
                {
                    Transform mix = ownerDefaultTarget.transform.Find(this.transform.Value);
                    animationState.AddMixingTransform(mix, this.recursive.Value);
                }
            }
        }
    }
}
