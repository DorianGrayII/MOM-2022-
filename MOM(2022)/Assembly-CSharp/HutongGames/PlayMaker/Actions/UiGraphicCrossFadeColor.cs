namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Tweens the color of the CanvasRenderer color associated with this Graphic.")]
    public class UiGraphicCrossFadeColor : ComponentAction<Graphic>
    {
        [RequiredField, CheckForComponent(typeof(Graphic)), HutongGames.PlayMaker.Tooltip("The GameObject with a UI component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Color target of the UI component. Leave to none and set the individual color values, for example to affect just the alpha channel")]
        public FsmColor color;
        [HutongGames.PlayMaker.Tooltip("The red channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat red;
        [HutongGames.PlayMaker.Tooltip("The green channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat green;
        [HutongGames.PlayMaker.Tooltip("The blue channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat blue;
        [HutongGames.PlayMaker.Tooltip("The alpha channel Color target of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat alpha;
        [HutongGames.PlayMaker.Tooltip("The duration of the tween")]
        public FsmFloat duration;
        [HutongGames.PlayMaker.Tooltip("Should ignore Time.scale?")]
        public FsmBool ignoreTimeScale;
        [HutongGames.PlayMaker.Tooltip("Should also Tween the alpha channel?")]
        public FsmBool useAlpha;
        private Graphic uiComponent;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiComponent = base.cachedComponent;
            }
            Color targetColor = this.uiComponent.color;
            if (!this.color.IsNone)
            {
                targetColor = this.color.get_Value();
            }
            if (!this.red.IsNone)
            {
                targetColor.r = this.red.Value;
            }
            if (!this.green.IsNone)
            {
                targetColor.g = this.green.Value;
            }
            if (!this.blue.IsNone)
            {
                targetColor.b = this.blue.Value;
            }
            if (!this.alpha.IsNone)
            {
                targetColor.a = this.alpha.Value;
            }
            this.uiComponent.CrossFadeColor(targetColor, this.duration.Value, this.ignoreTimeScale.Value, this.useAlpha.Value);
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.color = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.red = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.green = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.blue = num3;
            FsmFloat num4 = new FsmFloat();
            num4.UseVariable = true;
            this.alpha = num4;
            this.useAlpha = null;
            this.duration = 1f;
            this.ignoreTimeScale = null;
        }
    }
}

