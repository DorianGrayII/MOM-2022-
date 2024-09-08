namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the transition type of a UI Selectable component.")]
    public class UiTransitionGetType : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The transition value")]
        public FsmString transition;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is ColorTint")]
        public FsmEvent colorTintEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is SpriteSwap")]
        public FsmEvent spriteSwapEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is Animation")]
        public FsmEvent animationEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is none")]
        public FsmEvent noTransitionEvent;
        private Selectable selectable;
        private Selectable.Transition originalTransition;

        private void DoGetValue()
        {
            if (this.selectable != null)
            {
                this.transition.Value = this.selectable.transition.ToString();
                if (this.selectable.transition == Selectable.Transition.None)
                {
                    base.Fsm.Event(this.noTransitionEvent);
                }
                else if (this.selectable.transition == Selectable.Transition.ColorTint)
                {
                    base.Fsm.Event(this.colorTintEvent);
                }
                else if (this.selectable.transition == Selectable.Transition.SpriteSwap)
                {
                    base.Fsm.Event(this.spriteSwapEvent);
                }
                else if (this.selectable.transition == Selectable.Transition.Animation)
                {
                    base.Fsm.Event(this.animationEvent);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            this.DoGetValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.transition = null;
            this.colorTintEvent = null;
            this.spriteSwapEvent = null;
            this.animationEvent = null;
            this.noTransitionEvent = null;
        }
    }
}

