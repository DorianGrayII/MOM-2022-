namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Sets the Speed of an Animation. Check Every Frame to update the animation time continuously, e.g., if you're manipulating a variable that controls animation speed.")]
    public class SetAnimationSpeed : BaseAnimationAction
    {
        [RequiredField, CheckForComponent(typeof(Animation))]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Animation)]
        public FsmString animName;
        public FsmFloat speed = 1f;
        public bool everyFrame;

        private void DoSetAnimationSpeed(GameObject go)
        {
            if (base.UpdateCache(go))
            {
                AnimationState state = base.animation[this.animName.Value];
                if (state == null)
                {
                    base.LogWarning("Missing animation: " + this.animName.Value);
                }
                else
                {
                    state.speed = this.speed.Value;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetAnimationSpeed((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetAnimationSpeed((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.speed = 1f;
            this.everyFrame = false;
        }
    }
}

