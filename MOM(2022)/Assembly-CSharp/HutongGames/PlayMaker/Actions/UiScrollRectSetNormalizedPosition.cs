using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("The normalized scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.")]
    public class UiScrollRectSetNormalizedPosition : ComponentAction<ScrollRect>
    {
        [RequiredField]
        [CheckForComponent(typeof(ScrollRect))]
        [Tooltip("The GameObject with the UI ScrollRect component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
        public FsmVector2 normalizedPosition;

        [Tooltip("The horizontal position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
        [HasFloatSlider(0f, 1f)]
        public FsmFloat horizontalPosition;

        [Tooltip("The vertical position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
        [HasFloatSlider(0f, 1f)]
        public FsmFloat verticalPosition;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private ScrollRect scrollRect;

        private Vector2 originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.normalizedPosition = null;
            this.horizontalPosition = new FsmFloat
            {
                UseVariable = true
            };
            this.verticalPosition = new FsmFloat
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
                this.scrollRect = base.cachedComponent;
            }
            this.originalValue = this.scrollRect.normalizedPosition;
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
            if (!(this.scrollRect == null))
            {
                Vector2 value = this.scrollRect.normalizedPosition;
                if (!this.normalizedPosition.IsNone)
                {
                    value = this.normalizedPosition.Value;
                }
                if (!this.horizontalPosition.IsNone)
                {
                    value.x = this.horizontalPosition.Value;
                }
                if (!this.verticalPosition.IsNone)
                {
                    value.y = this.verticalPosition.Value;
                }
                this.scrollRect.normalizedPosition = value;
            }
        }

        public override void OnExit()
        {
            if (!(this.scrollRect == null) && this.resetOnExit.Value)
            {
                this.scrollRect.normalizedPosition = this.originalValue;
            }
        }
    }
}
