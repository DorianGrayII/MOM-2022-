using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Set Graphic Color. E.g. to set Sprite Color.")]
    public class UiGraphicSetColor : ComponentAction<Graphic>
    {
        [RequiredField]
        [CheckForComponent(typeof(Graphic))]
        [Tooltip("The GameObject with a UI component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Color of the UI component. Leave to none and set the individual color values, for example to affect just the alpha channel")]
        public FsmColor color;

        [Tooltip("The red channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat red;

        [Tooltip("The green channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat green;

        [Tooltip("The blue channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat blue;

        [Tooltip("The alpha channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
        public FsmFloat alpha;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;

        private Graphic uiComponent;

        private Color originalColor;

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
            this.resetOnExit = null;
            this.everyFrame = false;
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

        public override void OnUpdate()
        {
            this.DoSetColorValue();
        }

        private void DoSetColorValue()
        {
            if (!(this.uiComponent == null))
            {
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
                this.uiComponent.color = value;
            }
        }

        public override void OnExit()
        {
            if (!(this.uiComponent == null) && this.resetOnExit.Value)
            {
                this.uiComponent.color = this.originalColor;
            }
        }
    }
}
