using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the Color Block of a UI Selectable component.")]
    public class UiGetColorBlock : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The fade duration value. Leave as None for no effect")]
        [UIHint(UIHint.Variable)]
        public FsmFloat fadeDuration;

        [Tooltip("The color multiplier value. Leave as None for no effect")]
        [UIHint(UIHint.Variable)]
        public FsmFloat colorMultiplier;

        [Tooltip("The normal color value. Leave as None for no effect")]
        [UIHint(UIHint.Variable)]
        public FsmColor normalColor;

        [Tooltip("The pressed color value. Leave as None for no effect")]
        [UIHint(UIHint.Variable)]
        public FsmColor pressedColor;

        [Tooltip("The highlighted color value. Leave as None for no effect")]
        [UIHint(UIHint.Variable)]
        public FsmColor highlightedColor;

        [Tooltip("The disabled color value. Leave as None for no effect")]
        [UIHint(UIHint.Variable)]
        public FsmColor disabledColor;

        [Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;

        private Selectable selectable;

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

        private void DoGetValue()
        {
            if (!(this.selectable == null))
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
                    this.normalColor.Value = this.selectable.colors.normalColor;
                }
                if (!this.pressedColor.IsNone)
                {
                    this.pressedColor.Value = this.selectable.colors.pressedColor;
                }
                if (!this.highlightedColor.IsNone)
                {
                    this.highlightedColor.Value = this.selectable.colors.highlightedColor;
                }
                if (!this.disabledColor.IsNone)
                {
                    this.disabledColor.Value = this.selectable.colors.disabledColor;
                }
            }
        }
    }
}
