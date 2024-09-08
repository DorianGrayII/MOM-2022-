using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the Color Block of a UI Selectable component. Modifications will not be visible if transition is not ColorTint")]
    public class UiSetColorBlock : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The fade duration value. Leave as None for no effect")]
        public FsmFloat fadeDuration;

        [Tooltip("The color multiplier value. Leave as None for no effect")]
        public FsmFloat colorMultiplier;

        [Tooltip("The normal color value. Leave as None for no effect")]
        public FsmColor normalColor;

        [Tooltip("The pressed color value. Leave as None for no effect")]
        public FsmColor pressedColor;

        [Tooltip("The highlighted color value. Leave as None for no effect")]
        public FsmColor highlightedColor;

        [Tooltip("The disabled color value. Leave as None for no effect")]
        public FsmColor disabledColor;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;

        private Selectable selectable;

        private ColorBlock _colorBlock;

        private ColorBlock originalColorBlock;

        public override void Reset()
        {
            this.gameObject = null;
            this.fadeDuration = new FsmFloat
            {
                UseVariable = true
            };
            this.colorMultiplier = new FsmFloat
            {
                UseVariable = true
            };
            this.normalColor = new FsmColor
            {
                UseVariable = true
            };
            this.highlightedColor = new FsmColor
            {
                UseVariable = true
            };
            this.pressedColor = new FsmColor
            {
                UseVariable = true
            };
            this.disabledColor = new FsmColor
            {
                UseVariable = true
            };
            this.resetOnExit = null;
            this.everyFrame = false;
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
                this.originalColorBlock = this.selectable.colors;
            }
            this.DoSetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        private void DoSetValue()
        {
            if (!(this.selectable == null))
            {
                this._colorBlock = this.selectable.colors;
                if (!this.colorMultiplier.IsNone)
                {
                    this._colorBlock.colorMultiplier = this.colorMultiplier.Value;
                }
                if (!this.fadeDuration.IsNone)
                {
                    this._colorBlock.fadeDuration = this.fadeDuration.Value;
                }
                if (!this.normalColor.IsNone)
                {
                    this._colorBlock.normalColor = this.normalColor.Value;
                }
                if (!this.pressedColor.IsNone)
                {
                    this._colorBlock.pressedColor = this.pressedColor.Value;
                }
                if (!this.highlightedColor.IsNone)
                {
                    this._colorBlock.highlightedColor = this.highlightedColor.Value;
                }
                if (!this.disabledColor.IsNone)
                {
                    this._colorBlock.disabledColor = this.disabledColor.Value;
                }
                this.selectable.colors = this._colorBlock;
            }
        }

        public override void OnExit()
        {
            if (!(this.selectable == null) && this.resetOnExit.Value)
            {
                this.selectable.colors = this.originalColorBlock;
            }
        }
    }
}
