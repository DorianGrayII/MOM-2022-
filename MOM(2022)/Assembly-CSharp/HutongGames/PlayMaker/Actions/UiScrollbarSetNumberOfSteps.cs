using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the number of distinct scroll positions allowed for a UI Scrollbar component.")]
    public class UiScrollbarSetNumberOfSteps : ComponentAction<Scrollbar>
    {
        [RequiredField]
        [CheckForComponent(typeof(Scrollbar))]
        [Tooltip("The GameObject with the UI Scrollbar component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The number of distinct scroll positions allowed for the UI Scrollbar.")]
        public FsmInt value;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Scrollbar scrollbar;

        private int originalValue;

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
            this.originalValue = this.scrollbar.numberOfSteps;
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
                this.scrollbar.numberOfSteps = this.value.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.scrollbar == null) && this.resetOnExit.Value)
            {
                this.scrollbar.numberOfSteps = this.originalValue;
            }
        }
    }
}
