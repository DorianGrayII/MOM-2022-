using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the Animation Triggers of a UI Selectable component. Modifications will not be visible if transition is not Animation")]
    public class UiSetAnimationTriggers : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The normal trigger value. Leave as None for no effect")]
        public FsmString normalTrigger;

        [Tooltip("The highlighted trigger value. Leave as None for no effect")]
        public FsmString highlightedTrigger;

        [Tooltip("The pressed trigger value. Leave as None for no effect")]
        public FsmString pressedTrigger;

        [Tooltip("The disabled trigger value. Leave as None for no effect")]
        public FsmString disabledTrigger;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Selectable selectable;

        private AnimationTriggers _animationTriggers;

        private AnimationTriggers originalAnimationTriggers;

        public override void Reset()
        {
            this.gameObject = null;
            this.normalTrigger = new FsmString
            {
                UseVariable = true
            };
            this.highlightedTrigger = new FsmString
            {
                UseVariable = true
            };
            this.pressedTrigger = new FsmString
            {
                UseVariable = true
            };
            this.disabledTrigger = new FsmString
            {
                UseVariable = true
            };
            this.resetOnExit = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            if (this.selectable != null && this.resetOnExit.Value)
            {
                this.originalAnimationTriggers = this.selectable.animationTriggers;
            }
            this.DoSetValue();
            base.Finish();
        }

        private void DoSetValue()
        {
            if (!(this.selectable == null))
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

        public override void OnExit()
        {
            if (!(this.selectable == null) && this.resetOnExit.Value)
            {
                this.selectable.animationTriggers = this.originalAnimationTriggers;
            }
        }
    }
}
