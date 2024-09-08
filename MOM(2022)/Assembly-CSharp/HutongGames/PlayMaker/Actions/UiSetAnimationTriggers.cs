namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the Animation Triggers of a UI Selectable component. Modifications will not be visible if transition is not Animation")]
    public class UiSetAnimationTriggers : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The normal trigger value. Leave as None for no effect")]
        public FsmString normalTrigger;
        [HutongGames.PlayMaker.Tooltip("The highlighted trigger value. Leave as None for no effect")]
        public FsmString highlightedTrigger;
        [HutongGames.PlayMaker.Tooltip("The pressed trigger value. Leave as None for no effect")]
        public FsmString pressedTrigger;
        [HutongGames.PlayMaker.Tooltip("The disabled trigger value. Leave as None for no effect")]
        public FsmString disabledTrigger;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Selectable selectable;
        private AnimationTriggers _animationTriggers;
        private AnimationTriggers originalAnimationTriggers;

        private void DoSetValue()
        {
            if (this.selectable != null)
            {
                this._animationTriggers = this.selectable.animationTriggers;
                if (!this.normalTrigger.IsNone)
                {
                    this._animationTriggers.normalTrigger = this.normalTrigger.Value;
                }
                if (!this.highlightedTrigger.IsNone)
                {
                    this._animationTriggers.highlightedTrigger = this.highlightedTrigger.Value;
                }
                if (!this.pressedTrigger.IsNone)
                {
                    this._animationTriggers.pressedTrigger = this.pressedTrigger.Value;
                }
                if (!this.disabledTrigger.IsNone)
                {
                    this._animationTriggers.disabledTrigger = this.disabledTrigger.Value;
                }
                this.selectable.animationTriggers = this._animationTriggers;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this.originalAnimationTriggers = this.selectable.animationTriggers;
            }
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this.selectable.animationTriggers = this.originalAnimationTriggers;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmString text1 = new FsmString();
            text1.UseVariable = true;
            this.normalTrigger = text1;
            FsmString text2 = new FsmString();
            text2.UseVariable = true;
            this.highlightedTrigger = text2;
            FsmString text3 = new FsmString();
            text3.UseVariable = true;
            this.pressedTrigger = text3;
            FsmString text4 = new FsmString();
            text4.UseVariable = true;
            this.disabledTrigger = text4;
            this.resetOnExit = null;
        }
    }
}

