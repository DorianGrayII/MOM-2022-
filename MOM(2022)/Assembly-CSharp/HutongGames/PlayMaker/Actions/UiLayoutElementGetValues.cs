using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets various properties of a UI Layout Element component.")]
    public class UiLayoutElementGetValues : ComponentAction<LayoutElement>
    {
        [RequiredField]
        [CheckForComponent(typeof(LayoutElement))]
        [Tooltip("The GameObject with the UI LayoutElement component.")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Values")]
        [Tooltip("Is this element use Layout constraints")]
        [UIHint(UIHint.Variable)]
        public FsmBool ignoreLayout;

        [Tooltip("The minimum width enabled state")]
        [UIHint(UIHint.Variable)]
        public FsmBool minWidthEnabled;

        [Tooltip("The minimum width this layout element should have.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat minWidth;

        [Tooltip("The minimum height enabled state")]
        [UIHint(UIHint.Variable)]
        public FsmBool minHeightEnabled;

        [Tooltip("The minimum height this layout element should have.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat minHeight;

        [Tooltip("The preferred width enabled state")]
        [UIHint(UIHint.Variable)]
        public FsmBool preferredWidthEnabled;

        [Tooltip("The preferred width this layout element should have before additional available width is allocated.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat preferredWidth;

        [Tooltip("The preferred height enabled state")]
        [UIHint(UIHint.Variable)]
        public FsmBool preferredHeightEnabled;

        [Tooltip("The preferred height this layout element should have before additional available height is allocated.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat preferredHeight;

        [Tooltip("The flexible width enabled state")]
        [UIHint(UIHint.Variable)]
        public FsmBool flexibleWidthEnabled;

        [Tooltip("The relative amount of additional available width this layout element should fill out relative to its siblings.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat flexibleWidth;

        [Tooltip("The flexible height enabled state")]
        [UIHint(UIHint.Variable)]
        public FsmBool flexibleHeightEnabled;

        [Tooltip("The relative amount of additional available height this layout element should fill out relative to its siblings.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat flexibleHeight;

        [ActionSection("Options")]
        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private LayoutElement layoutElement;

        public override void Reset()
        {
            this.gameObject = null;
            this.ignoreLayout = null;
            this.minWidthEnabled = null;
            this.minHeightEnabled = null;
            this.preferredWidthEnabled = null;
            this.preferredHeightEnabled = null;
            this.flexibleWidthEnabled = null;
            this.flexibleHeightEnabled = null;
            this.minWidth = null;
            this.minHeight = null;
            this.preferredWidth = null;
            this.preferredHeight = null;
            this.flexibleWidth = null;
            this.flexibleHeight = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.layoutElement = base.cachedComponent;
            }
            this.DoGetValues();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValues();
        }

        private void DoGetValues()
        {
            if (!(this.layoutElement == null))
            {
                if (!this.ignoreLayout.IsNone)
                {
                    this.ignoreLayout.Value = this.layoutElement.ignoreLayout;
                }
                if (!this.minWidthEnabled.IsNone)
                {
                    this.minWidthEnabled.Value = this.layoutElement.minWidth != 0f;
                }
                if (!this.minWidth.IsNone)
                {
                    this.minWidth.Value = this.layoutElement.minWidth;
                }
                if (!this.minHeightEnabled.IsNone)
                {
                    this.minHeightEnabled.Value = this.layoutElement.minHeight != 0f;
                }
                if (!this.minHeight.IsNone)
                {
                    this.minHeight.Value = this.layoutElement.minHeight;
                }
                if (!this.preferredWidthEnabled.IsNone)
                {
                    this.preferredWidthEnabled.Value = this.layoutElement.preferredWidth != 0f;
                }
                if (!this.preferredWidth.IsNone)
                {
                    this.preferredWidth.Value = this.layoutElement.preferredWidth;
                }
                if (!this.preferredHeightEnabled.IsNone)
                {
                    this.preferredHeightEnabled.Value = this.layoutElement.preferredHeight != 0f;
                }
                if (!this.preferredHeight.IsNone)
                {
                    this.preferredHeight.Value = this.layoutElement.preferredHeight;
                }
                if (!this.flexibleWidthEnabled.IsNone)
                {
                    this.flexibleWidthEnabled.Value = this.layoutElement.flexibleWidth != 0f;
                }
                if (!this.flexibleWidth.IsNone)
                {
                    this.flexibleWidth.Value = this.layoutElement.flexibleWidth;
                }
                if (!this.flexibleHeightEnabled.IsNone)
                {
                    this.flexibleHeightEnabled.Value = this.layoutElement.flexibleHeight != 0f;
                }
                if (!this.flexibleHeight.IsNone)
                {
                    this.flexibleHeight.Value = this.layoutElement.flexibleHeight;
                }
            }
        }
    }
}
