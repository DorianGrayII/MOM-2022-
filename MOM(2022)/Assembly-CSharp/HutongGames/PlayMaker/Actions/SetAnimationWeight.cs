namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Sets the Blend Weight of an Animation. Check Every Frame to update the weight continuously, e.g., if you're manipulating a variable that controls the weight.")]
    public class SetAnimationWeight : BaseAnimationAction
    {
        [RequiredField, CheckForComponent(typeof(Animation))]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Animation)]
        public FsmString animName;
        public FsmFloat weight = 1f;
        public bool everyFrame;

        private void DoSetAnimationWeight(GameObject go)
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
                    state.weight = this.weight.Value;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetAnimationWeight((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetAnimationWeight((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.weight = 1f;
            this.everyFrame = false;
        }
    }
}

