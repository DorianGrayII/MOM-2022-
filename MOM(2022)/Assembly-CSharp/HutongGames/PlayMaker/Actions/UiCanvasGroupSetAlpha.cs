using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Set Group Alpha.")]
    public class UiCanvasGroupSetAlpha : ComponentAction<CanvasGroup>
    {
        [RequiredField]
        [CheckForComponent(typeof(CanvasGroup))]
        [Tooltip("The GameObject with a UI CanvasGroup component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The alpha of the UI component.")]
        public FsmFloat alpha;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;

        private CanvasGroup component;

        private float originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.alpha = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.component = base.cachedComponent;
            }
            this.originalValue = this.component.alpha;
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
            if (this.component != null)
            {
                this.component.alpha = this.alpha.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.component == null) && this.resetOnExit.Value)
            {
                this.component.alpha = this.originalValue;
            }
        }
    }
}
