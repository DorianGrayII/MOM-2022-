using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the direction of a UI Slider component.")]
    public class UiSliderSetDirection : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The direction of the UI Slider component.")]
        [ObjectType(typeof(Slider.Direction))]
        public FsmEnum direction;

        [Tooltip("Include the  RectLayouts. Leave to none for no effect")]
        public FsmBool includeRectLayouts;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Slider slider;

        private Slider.Direction originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = Slider.Direction.LeftToRight;
            this.includeRectLayouts = new FsmBool
            {
                UseVariable = true
            };
            this.resetOnExit = null;
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

        private void DoSetValue()
        {
            if (!(this.slider == null))
            {
                if (this.includeRectLayouts.IsNone)
                {
                    this.slider.direction = (Slider.Direction)(object)this.direction.Value;
                }
                else
                {
                    this.slider.SetDirection((Slider.Direction)(object)this.direction.Value, this.includeRectLayouts.Value);
                }
            }
        }

        public override void OnExit()
        {
            if (!(this.slider == null) && this.resetOnExit.Value)
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
    }
}
