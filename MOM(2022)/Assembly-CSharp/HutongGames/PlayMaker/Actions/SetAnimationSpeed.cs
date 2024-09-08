using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [Tooltip("Sets the Speed of an Animation. Check Every Frame to update the animation time continuously, e.g., if you're manipulating a variable that controls animation speed.")]
    public class SetAnimationSpeed : BaseAnimationAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animation))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Animation)]
        public FsmString animName;

        public FsmFloat speed = 1f;

        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.speed = 1f;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetAnimationSpeed((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetAnimationSpeed((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
        }

        private void DoSetAnimationSpeed(GameObject go)
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
                    animationState.speed = this.speed.Value;
                }
            }
        }
    }
}
