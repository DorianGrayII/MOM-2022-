namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the Color Block of a UI Selectable component. Modifications will not be visible if transition is not ColorTint")]
    public class UiSetColorBlock : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The fade duration value. Leave as None for no effect")]
        public FsmFloat fadeDuration;
        [HutongGames.PlayMaker.Tooltip("The color multiplier value. Leave as None for no effect")]
        public FsmFloat colorMultiplier;
        [HutongGames.PlayMaker.Tooltip("The normal color value. Leave as None for no effect")]
        public FsmColor normalColor;
        [HutongGames.PlayMaker.Tooltip("The pressed color value. Leave as None for no effect")]
        public FsmColor pressedColor;
        [HutongGames.PlayMaker.Tooltip("The highlighted color value. Leave as None for no effect")]
        public FsmColor highlightedColor;
        [HutongGames.PlayMaker.Tooltip("The disabled color value. Leave as None for no effect")]
        public FsmColor disabledColor;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private Selectable selectable;
        private ColorBlock _colorBlock;
        private ColorBlock originalColorBlock;

        private void DoSetValue()
        {
            if (this.selectable != null)
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
                    this._colorBlock.normalColor = this.normalColor.get_Value();
                }
                if (!this.pressedColor.IsNone)
                {
                    this._colorBlock.pressedColor = this.pressedColor.get_Value();
                }
                if (!this.highlightedColor.IsNone)
                {
                    this._colorBlock.highlightedColor = this.highlightedColor.get_Value();
                }
                if (!this.disabledColor.IsNone)
                {
                    this._colorBlock.disabledColor = this.disabledColor.get_Value();
                }
                this.selectable.colors = this._colorBlock;
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
                this.originalColorBlock = this.selectable.colors;
            }
            this.DoSetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnExit()
        {
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this.selectable.colors = this.originalColorBlock;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.fadeDuration = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.colorMultiplier = num2;
            FsmColor color1 = new FsmColor();
            color1.UseVariable = true;
            this.normalColor = color1;
            FsmColor color2 = new FsmColor();
            color2.UseVariable = true;
            this.highlightedColor = color2;
            FsmColor color3 = new FsmColor();
            color3.UseVariable = true;
            this.pressedColor = color3;
            FsmColor color4 = new FsmColor();
            color4.UseVariable = true;
            this.disabledColor = color4;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

