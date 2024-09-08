namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the Color Block of a UI Selectable component.")]
    public class UiGetColorBlock : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The fade duration value. Leave as None for no effect"), UIHint(UIHint.Variable)]
        public FsmFloat fadeDuration;
        [HutongGames.PlayMaker.Tooltip("The color multiplier value. Leave as None for no effect"), UIHint(UIHint.Variable)]
        public FsmFloat colorMultiplier;
        [HutongGames.PlayMaker.Tooltip("The normal color value. Leave as None for no effect"), UIHint(UIHint.Variable)]
        public FsmColor normalColor;
        [HutongGames.PlayMaker.Tooltip("The pressed color value. Leave as None for no effect"), UIHint(UIHint.Variable)]
        public FsmColor pressedColor;
        [HutongGames.PlayMaker.Tooltip("The highlighted color value. Leave as None for no effect"), UIHint(UIHint.Variable)]
        public FsmColor highlightedColor;
        [HutongGames.PlayMaker.Tooltip("The disabled color value. Leave as None for no effect"), UIHint(UIHint.Variable)]
        public FsmColor disabledColor;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private Selectable selectable;

        private void DoGetValue()
        {
            if (this.selectable != null)
            {
                if (!this.colorMultiplier.IsNone)
                {
                    this.colorMultiplier.Value = this.selectable.colors.colorMultiplier;
                }
                if (!this.fadeDuration.IsNone)
                {
                    this.fadeDuration.Value = this.selectable.colors.fadeDuration;
                }
                if (!this.normalColor.IsNone)
                {
                    this.normalColor.set_Value(this.selectable.colors.normalColor);
                }
                if (!this.pressedColor.IsNone)
                {
                    this.pressedColor.set_Value(this.selectable.colors.pressedColor);
                }
                if (!this.highlightedColor.IsNone)
                {
                    this.highlightedColor.set_Value(this.selectable.colors.highlightedColor);
                }
                if (!this.disabledColor.IsNone)
                {
                    this.disabledColor.set_Value(this.selectable.colors.disabledColor);
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
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fadeDuration = null;
            this.colorMultiplier = null;
            this.normalColor = null;
            this.highlightedColor = null;
            this.pressedColor = null;
            this.disabledColor = null;
            this.everyFrame = false;
        }
    }
}

