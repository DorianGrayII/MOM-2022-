namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("The normalized scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.")]
    public class UiScrollRectSetNormalizedPosition : ComponentAction<ScrollRect>
    {
        [RequiredField, CheckForComponent(typeof(ScrollRect)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI ScrollRect component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
        public FsmVector2 normalizedPosition;
        [HutongGames.PlayMaker.Tooltip("The horizontal position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0."), HasFloatSlider(0f, 1f)]
        public FsmFloat horizontalPosition;
        [HutongGames.PlayMaker.Tooltip("The vertical position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0."), HasFloatSlider(0f, 1f)]
        public FsmFloat verticalPosition;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private ScrollRect scrollRect;
        private Vector2 originalValue;

        private void DoSetValue()
        {
            if (this.scrollRect != null)
            {
                Vector2 normalizedPosition = this.scrollRect.normalizedPosition;
                if (!this.normalizedPosition.IsNone)
                {
                    normalizedPosition = this.normalizedPosition.get_Value();
                }
                if (!this.horizontalPosition.IsNone)
                {
                    normalizedPosition.x = this.horizontalPosition.Value;
                }
                if (!this.verticalPosition.IsNone)
                {
                    normalizedPosition.y = this.verticalPosition.Value;
                }
                this.scrollRect.normalizedPosition = normalizedPosition;
            }
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

        public override void OnExit()
        {
            if ((this.scrollRect != null) && this.resetOnExit.Value)
            {
                this.scrollRect.normalizedPosition = this.originalValue;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.normalizedPosition = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.horizontalPosition = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.verticalPosition = num2;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

