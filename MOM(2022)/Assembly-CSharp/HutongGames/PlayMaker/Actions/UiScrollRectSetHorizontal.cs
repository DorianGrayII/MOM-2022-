using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the UI ScrollRect horizontal flag")]
    public class UiScrollRectSetHorizontal : ComponentAction<ScrollRect>
    {
        [RequiredField]
        [CheckForComponent(typeof(ScrollRect))]
        [Tooltip("The GameObject with the UI ScrollRect component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The horizontal flag")]
        public FsmBool horizontal;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private ScrollRect scrollRect;

        private bool originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.horizontal = null;
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
                this.scrollRect.horizontal = this.horizontal.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.scrollRect == null) && this.resetOnExit.Value)
            {
                this.scrollRect.horizontal = this.originalValue;
            }
        }
    }
}
