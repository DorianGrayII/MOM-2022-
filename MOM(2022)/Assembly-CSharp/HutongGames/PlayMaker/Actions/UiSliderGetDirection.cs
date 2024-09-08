using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the direction of a UI Slider component.")]
    public class UiSliderGetDirection : ComponentAction<Slider>
    {
        [RequiredField]
        [CheckForComponent(typeof(Slider))]
        [Tooltip("The GameObject with the UI Slider component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The direction of the UI Slider.")]
        [ObjectType(typeof(Slider.Direction))]
        public FsmEnum direction;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Slider slider;

        public override void Reset()
        {
            this.gameObject = null;
            this.direction = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.slider = base.cachedComponent;
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
            if (this.slider != null)
            {
                this.direction.Value = this.slider.direction;
            }
        }
    }
}
