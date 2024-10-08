using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the fractional size of the handle of a UI Scrollbar component. Ranges from 0.0 to 1.0.")]
    public class UiScrollbarSetSize : ComponentAction<Scrollbar>
    {
        [RequiredField]
        [CheckForComponent(typeof(Scrollbar))]
        [Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The fractional size of the handle for the UI Scrollbar. Ranges from 0.0 to 1.0.")]
        [HasFloatSlider(0f, 1f)]
        public FsmFloat value;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Scrollbar scrollbar;

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
                this.scrollbar = base.cachedComponent;
            }
            if (this.resetOnExit.Value)
            {
                this.originalValue = this.scrollbar.size;
            }
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
            if (this.scrollbar != null)
            {
                this.scrollbar.size = this.value.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.scrollbar == null) && this.resetOnExit.Value)
            {
                this.scrollbar.size = this.originalValue;
            }
        }
    }
}
