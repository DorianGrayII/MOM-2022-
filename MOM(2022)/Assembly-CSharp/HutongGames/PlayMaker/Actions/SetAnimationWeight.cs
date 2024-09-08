using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [Tooltip("Sets the Blend Weight of an Animation. Check Every Frame to update the weight continuously, e.g., if you're manipulating a variable that controls the weight.")]
    public class SetAnimationWeight : BaseAnimationAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animation))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Animation)]
        public FsmString animName;

        public FsmFloat weight = 1f;

        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.weight = 1f;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetAnimationWeight((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetAnimationWeight((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
        }

        private void DoSetAnimationWeight(GameObject go)
        {
            if (base.UpdateCache(go))
            {
                AnimationState animationState = base.animation[this.animName.Value];
                if (animationState == null)
                {
                    base.LogWarning("Missing animation: " + this.animName.Value);
                }
                else
                {
                    animationState.weight = this.weight.Value;
                }
            }
        }
    }
}
