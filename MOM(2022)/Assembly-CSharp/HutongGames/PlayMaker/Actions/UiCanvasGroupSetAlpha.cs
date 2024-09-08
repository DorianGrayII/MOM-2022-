namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Set Group Alpha.")]
    public class UiCanvasGroupSetAlpha : ComponentAction<CanvasGroup>
    {
        [RequiredField, CheckForComponent(typeof(CanvasGroup)), HutongGames.PlayMaker.Tooltip("The GameObject with a UI CanvasGroup component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The alpha of the UI component.")]
        public FsmFloat alpha;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private CanvasGroup component;
        private float originalValue;

        private void DoSetValue()
        {
            if (this.component != null)
            {
                this.component.alpha = this.alpha.Value;
            }
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

        public override void OnExit()
        {
            if ((this.component != null) && this.resetOnExit.Value)
            {
                this.component.alpha = this.originalValue;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.alpha = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

