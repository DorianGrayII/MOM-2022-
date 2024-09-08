using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the UI ScrollRect vertical flag")]
    public class UiScrollRectSetVertical : ComponentAction<ScrollRect>
    {
        [RequiredField]
        [CheckForComponent(typeof(ScrollRect))]
        [Tooltip("The GameObject with the UI ScrollRect component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The vertical flag")]
        public FsmBool vertical;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private ScrollRect scrollRect;

        private bool originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.vertical = null;
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
            this.originalValue = this.scrollRect.vertical;
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
            if (this.scrollRect != null)
            {
                this.scrollRect.vertical = this.vertical.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.scrollRect == null) && this.resetOnExit.Value)
            {
                this.scrollRect.vertical = this.originalValue;
            }
        }
    }
}
