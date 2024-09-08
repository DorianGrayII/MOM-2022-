using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the value of a UI Slider component.")]
    public class UiSliderSetValue : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The value of the UI Slider component.")]
        public FsmFloat value;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Slider slider;

        private float originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.value = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
            }
            this.originalValue = this.slider.value;
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
            if (this.slider != null)
            {
                this.slider.value = this.value.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.slider == null) && this.resetOnExit.Value)
            {
                this.slider.value = this.originalValue;
            }
        }
    }
}
