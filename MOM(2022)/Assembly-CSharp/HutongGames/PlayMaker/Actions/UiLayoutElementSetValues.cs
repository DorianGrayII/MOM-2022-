using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets various properties of a UI Layout Element component.")]
    public class UiLayoutElementSetValues : ComponentAction<LayoutElement>
    {
        [RequiredField]
        [CheckForComponent(typeof(LayoutElement))]
        [Tooltip("The GameObject with the UI LayoutElement component.")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Values")]
        [Tooltip("The minimum width this layout element should have.")]
        public FsmFloat minWidth;

        [Tooltip("The minimum height this layout element should have.")]
        public FsmFloat minHeight;

        [Tooltip("The preferred width this layout element should have before additional available width is allocated.")]
        public FsmFloat preferredWidth;

        [Tooltip("The preferred height this layout element should have before additional available height is allocated.")]
        public FsmFloat preferredHeight;

        [Tooltip("The relative amount of additional available width this layout element should fill out relative to its siblings.")]
        public FsmFloat flexibleWidth;

        [Tooltip("The relative amount of additional available height this layout element should fill out relative to its siblings.")]
        public FsmFloat flexibleHeight;

        [ActionSection("Options")]
        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private LayoutElement layoutElement;

        public override void Reset()
        {
            this.gameObject = null;
            this.minWidth = new FsmFloat
            {
                UseVariable = true
            };
            this.minHeight = new FsmFloat
            {
                UseVariable = true
            };
            this.preferredWidth = new FsmFloat
            {
                UseVariable = true
            };
            this.preferredHeight = new FsmFloat
            {
                UseVariable = true
            };
            this.flexibleWidth = new FsmFloat
            {
                UseVariable = true
            };
            this.flexibleHeight = new FsmFloat
            {
                UseVariable = true
            };
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.layoutElement = base.cachedComponent;
            }
            this.DoSetValues();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValues();
        }

        private void DoSetValues()
        {
            if (!(this.layoutElement == null))
            {
                if (!this.minWidth.IsNone)
                {
                    this.layoutElement.minWidth = this.minWidth.Value;
                }
                if (!this.minHeight.IsNone)
                {
                    this.layoutElement.minHeight = this.minHeight.Value;
                }
                if (!this.preferredWidth.IsNone)
                {
                    this.layoutElement.preferredWidth = this.preferredWidth.Value;
                }
                if (!this.preferredHeight.IsNone)
                {
                    this.layoutElement.preferredHeight = this.preferredHeight.Value;
                }
                if (!this.flexibleWidth.IsNone)
                {
                    this.layoutElement.flexibleWidth = this.flexibleWidth.Value;
                }
                if (!this.flexibleHeight.IsNone)
                {
                    this.layoutElement.flexibleHeight = this.flexibleHeight.Value;
                }
            }
        }
    }
}
