namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the UI ScrollRect horizontal flag")]
    public class UiScrollRectSetHorizontal : ComponentAction<ScrollRect>
    {
        [RequiredField, CheckForComponent(typeof(ScrollRect)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI ScrollRect component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The horizontal flag")]
        public FsmBool horizontal;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private ScrollRect scrollRect;
        private bool originalValue;

        private void DoSetValue()
        {
            if (this.scrollRect != null)
            {
                this.scrollRect.horizontal = this.horizontal.Value;
            }
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

        public override void OnExit()
        {
            if ((this.scrollRect != null) && this.resetOnExit.Value)
            {
                this.scrollRect.horizontal = this.originalValue;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.horizontal = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

