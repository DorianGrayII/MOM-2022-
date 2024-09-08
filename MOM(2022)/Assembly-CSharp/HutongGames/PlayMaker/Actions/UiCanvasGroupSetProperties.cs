using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets properties of a UI CanvasGroup component.")]
    public class UiCanvasGroupSetProperties : ComponentAction<CanvasGroup>
    {
        [RequiredField]
        [CheckForComponent(typeof(CanvasGroup))]
        [Tooltip("The GameObject with the UI CanvasGroup component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Canvas group alpha. Ranges from 0.0 to 1.0.")]
        [HasFloatSlider(0f, 1f)]
        public FsmFloat alpha;

        [Tooltip("Is the group interactable (are the elements beneath the group enabled). Leave as None for no effect")]
        public FsmBool interactable;

        [Tooltip("Does this group block raycasting (allow collision). Leave as None for no effect")]
        public FsmBool blocksRaycasts;

        [Tooltip("Should the group ignore parent groups? Leave as None for no effect")]
        public FsmBool ignoreParentGroup;

        [Tooltip("Reset when exiting this state. Leave as None for no effect")]
        public FsmBool resetOnExit;

        public bool everyFrame;

        private CanvasGroup component;

        private float originalAlpha;

        private bool originalInteractable;

        private bool originalBlocksRaycasts;

        private bool originalIgnoreParentGroup;

        public override void Reset()
        {
            this.gameObject = null;
            this.alpha = new FsmFloat
            {
                UseVariable = true
            };
            this.interactable = new FsmBool
            {
                UseVariable = true
            };
            this.blocksRaycasts = new FsmBool
            {
                UseVariable = true
            };
            this.ignoreParentGroup = new FsmBool
            {
                UseVariable = true
            };
            this.resetOnExit = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.component = base.cachedComponent;
                if (this.component != null)
                {
                    this.originalAlpha = this.component.alpha;
                    this.originalInteractable = this.component.interactable;
                    this.originalBlocksRaycasts = this.component.blocksRaycasts;
                    this.originalIgnoreParentGroup = this.component.ignoreParentGroups;
                }
            }
            this.DoAction();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoAction();
        }

        private void DoAction()
        {
            if (!(this.component == null))
            {
                if (!this.alpha.IsNone)
                {
                    this.component.alpha = this.alpha.Value;
                }
                if (!this.interactable.IsNone)
                {
                    this.component.interactable = this.interactable.Value;
                }
                if (!this.blocksRaycasts.IsNone)
                {
                    this.component.blocksRaycasts = this.blocksRaycasts.Value;
                }
                if (!this.ignoreParentGroup.IsNone)
                {
                    this.component.ignoreParentGroups = this.ignoreParentGroup.Value;
                }
            }
        }

        public override void OnExit()
        {
            if (!(this.component == null) && this.resetOnExit.Value)
            {
                if (!this.alpha.IsNone)
                {
                    this.component.alpha = this.originalAlpha;
                }
                if (!this.interactable.IsNone)
                {
                    this.component.interactable = this.originalInteractable;
                }
                if (!this.blocksRaycasts.IsNone)
                {
                    this.component.blocksRaycasts = this.originalBlocksRaycasts;
                }
                if (!this.ignoreParentGroup.IsNone)
                {
                    this.component.ignoreParentGroups = this.originalIgnoreParentGroup;
                }
            }
        }
    }
}
