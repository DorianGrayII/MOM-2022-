using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [Tooltip("Removes a mixing transform previously added with Add Mixing Transform. If transform has been added as recursive, then it will be removed as recursive. Once you remove all mixing transforms added to animation state all curves become animated again.")]
    public class RemoveMixingTransform : BaseAnimationAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animation))]
        [Tooltip("The GameObject playing the animation.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The name of the animation.")]
        public FsmString animationName;

        [RequiredField]
        [Tooltip("The mixing transform to remove. E.g., root/upper_body/left_shoulder")]
        public FsmString transfrom;

        public override void Reset()
        {
            this.gameObject = null;
            this.animationName = "";
        }

        public override void OnEnter()
        {
            this.DoRemoveMixingTransform();
            base.Finish();
        }

        private void DoRemoveMixingTransform()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                AnimationState animationState = base.animation[this.animationName.Value];
                if (!(animationState == null))
                {
                    Transform mix = ownerDefaultTarget.transform.Find(this.transfrom.Value);
                    animationState.AddMixingTransform(mix);
                }
            }
        }
    }
}
