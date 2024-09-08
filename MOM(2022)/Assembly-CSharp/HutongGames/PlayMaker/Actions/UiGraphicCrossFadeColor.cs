using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Tweens the color of the CanvasRenderer color associated with this Graphic.")]
    public class UiGraphicCrossFadeColor : ComponentAction<Graphic>
    {
        [RequiredField]
        [CheckForComponent(typeof(Graphic))]
        [Tooltip("The GameObject with a UI component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Color target of the UI component. Leave to none and set the individual color values, for example to affect just the alpha channel")]
        public FsmColor color;

        [Tooltip("The red channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat red;

        [Tooltip("The green channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat green;

        [Tooltip("The blue channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat blue;

        [Tooltip("The alpha channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat alpha;

        [Tooltip("The duration of the tween")]
        public FsmFloat duration;

        [Tooltip("Should ignore Time.scale?")]
        public FsmBool ignoreTimeScale;

        [Tooltip("Should also Tween the alpha channel?")]
        public FsmBool useAlpha;

        private Graphic uiComponent;

        public override void Reset()
        {
            this.gameObject = null;
            this.color = null;
            this.red = new FsmFloat
            {
                UseVariable = true
            };
            this.green = new FsmFloat
            {
                UseVariable = true
            };
            this.blue = new FsmFloat
            {
                UseVariable = true
            };
            this.alpha = new FsmFloat
            {
                UseVariable = true
            };
            this.useAlpha = null;
            this.duration = 1f;
            this.ignoreTimeScale = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiComponent = base.cachedComponent;
            }
            Color value = this.uiComponent.color;
            if (!this.color.IsNone)
            {
                value = this.color.Value;
            }
            if (!this.red.IsNone)
            {
                value.r = this.red.Value;
            }
            if (!this.green.IsNone)
            {
                value.g = this.green.Value;
            }
            if (!this.blue.IsNone)
            {
                value.b = this.blue.Value;
            }
            if (!this.alpha.IsNone)
            {
                value.a = this.alpha.Value;
            }
            this.uiComponent.CrossFadeColor(value, this.duration.Value, this.ignoreTimeScale.Value, this.useAlpha.Value);
            base.Finish();
        }
    }
}
