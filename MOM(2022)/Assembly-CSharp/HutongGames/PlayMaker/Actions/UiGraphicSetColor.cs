namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Set Graphic Color. E.g. to set Sprite Color.")]
    public class UiGraphicSetColor : ComponentAction<Graphic>
    {
        [RequiredField, CheckForComponent(typeof(Graphic)), HutongGames.PlayMaker.Tooltip("The GameObject with a UI component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Color of the UI component. Leave to none and set the individual color values, for example to affect just the alpha channel")]
        public FsmColor color;
        [HutongGames.PlayMaker.Tooltip("The red channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat red;
        [HutongGames.PlayMaker.Tooltip("The green channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat green;
        [HutongGames.PlayMaker.Tooltip("The blue channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat blue;
        [HutongGames.PlayMaker.Tooltip("The alpha channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat alpha;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private Graphic uiComponent;
        private Color originalColor;

        private void DoSetColorValue()
        {
            if (this.uiComponent != null)
            {
                Color color = this.uiComponent.color;
                if (!this.color.IsNone)
                {
                    color = this.color.get_Value();
                }
                if (!this.red.IsNone)
                {
                    color.r = this.red.Value;
                }
                if (!this.green.IsNone)
                {
                    color.g = this.green.Value;
                }
                if (!this.blue.IsNone)
                {
                    color.b = this.blue.Value;
                }
                if (!this.alpha.IsNone)
                {
                    color.a = this.alpha.Value;
                }
                this.uiComponent.color = color;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiComponent = base.cachedComponent;
            }
            this.originalColor = this.uiComponent.color;
            this.DoSetColorValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnExit()
        {
            if ((this.uiComponent != null) && this.resetOnExit.Value)
            {
                this.uiComponent.color = this.originalColor;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetColorValue();
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
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

