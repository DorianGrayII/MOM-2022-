namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Blends an Animation towards a Target Weight over a specified Time.\nOptionally sends an Event when finished.")]
    public class BlendAnimation : BaseAnimationAction
    {
        [RequiredField, CheckForComponent(typeof(Animation)), HutongGames.PlayMaker.Tooltip("The GameObject to animate.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Animation), HutongGames.PlayMaker.Tooltip("The name of the animation to blend.")]
        public FsmString animName;
        [RequiredField, HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Target weight to blend to.")]
        public FsmFloat targetWeight;
        [RequiredField, HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("How long should the blend take.")]
        public FsmFloat time;
        [HutongGames.PlayMaker.Tooltip("Event to send when the blend has finished.")]
        public FsmEvent finishEvent;
        private DelayedEvent delayedFinishEvent;

        private void DoBlendAnimation(GameObject go)
        {
            if (go != null)
            {
                Animation component = go.GetComponent<Animation>();
                if (component == null)
                {
                    base.LogWarning("Missing Animation component on GameObject: " + go.name);
                    base.Finish();
                }
                else
                {
                    AnimationState state = component[this.animName.Value];
                    if (state == null)
                    {
                        base.LogWarning("Missing animation: " + this.animName.Value);
                        base.Finish();
                    }
                    else
                    {
                        float fadeLength = this.time.Value;
                        component.Blend(this.animName.Value, this.targetWeight.Value, fadeLength);
                        if (this.finishEvent != null)
                        {
                            this.delayedFinishEvent = base.Fsm.DelayedEvent(this.finishEvent, state.length);
                        }
                        else
                        {
                            base.Finish();
                        }
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoBlendAnimation((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
        }

        public override void OnUpdate()
        {
            if (DelayedEvent.WasSent(this.delayedFinishEvent))
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
            this.targetWeight = 1f;
            this.time = 0.3f;
            this.finishEvent = null;
        }
    }
}

