namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets various properties of a UI Layout Element component.")]
    public class UiLayoutElementSetValues : ComponentAction<LayoutElement>
    {
        [RequiredField, CheckForComponent(typeof(LayoutElement)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI LayoutElement component.")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Values"), HutongGames.PlayMaker.Tooltip("The minimum width this layout element should have.")]
        public FsmFloat minWidth;
        [HutongGames.PlayMaker.Tooltip("The minimum height this layout element should have.")]
        public FsmFloat minHeight;
        [HutongGames.PlayMaker.Tooltip("The preferred width this layout element should have before additional available width is allocated.")]
        public FsmFloat preferredWidth;
        [HutongGames.PlayMaker.Tooltip("The preferred height this layout element should have before additional available height is allocated.")]
        public FsmFloat preferredHeight;
        [HutongGames.PlayMaker.Tooltip("The relative amount of additional available width this layout element should fill out relative to its siblings.")]
        public FsmFloat flexibleWidth;
        [HutongGames.PlayMaker.Tooltip("The relative amount of additional available height this layout element should fill out relative to its siblings.")]
        public FsmFloat flexibleHeight;
        [ActionSection("Options"), HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private LayoutElement layoutElement;

        private void DoSetValues()
        {
            if (this.layoutElement != null)
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

        public override void Reset()
        {
            this.gameObject = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.minWidth = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.minHeight = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.preferredWidth = num3;
            FsmFloat num4 = new FsmFloat();
            num4.UseVariable = true;
            this.preferredHeight = num4;
            FsmFloat num5 = new FsmFloat();
            num5.UseVariable = true;
            this.flexibleWidth = num5;
            FsmFloat num6 = new FsmFloat();
            num6.UseVariable = true;
            this.flexibleHeight = num6;
        }
    }
}

