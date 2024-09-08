namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the direction of a UI Slider component.")]
    public class UiSliderSetDirection : ComponentAction<Slider>
    {
        [RequiredField, CheckForComponent(typeof(Slider)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The direction of the UI Slider component."), ObjectType(typeof(Slider.Direction))]
        public FsmEnum direction;
        [HutongGames.PlayMaker.Tooltip("Include the  RectLayouts. Leave to none for no effect")]
        public FsmBool includeRectLayouts;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Slider slider;
        private Slider.Direction originalValue;

        private void DoSetValue()
        {
            if (this.slider != null)
            {
                if (this.includeRectLayouts.IsNone)
                {
                    this.slider.direction = (Slider.Direction) this.direction.Value;
                }
                else
                {
                    this.slider.SetDirection((Slider.Direction) this.direction.Value, this.includeRectLayouts.Value);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.originalValue = this.slider.direction;
            this.DoSetValue();
        }

        public override void OnExit()
        {
            if ((this.slider != null) && this.resetOnExit.Value)
            {
                if (this.includeRectLayouts.IsNone)
                {
                    this.slider.direction = this.originalValue;
                }
                else
                {
                    this.slider.SetDirection(this.originalValue, this.includeRectLayouts.Value);
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = Slider.Direction.LeftToRight;
            FsmBool bool1 = new FsmBool();
            bool1.UseVariable = true;
            this.includeRectLayouts = bool1;
            this.resetOnExit = null;
        }
    }
}

