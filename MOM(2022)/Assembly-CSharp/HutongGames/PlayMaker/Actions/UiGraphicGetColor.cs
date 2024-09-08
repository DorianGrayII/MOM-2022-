namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the color of a UI Graphic component. (E.g. UI Sprite)")]
    public class UiGraphicGetColor : ComponentAction<Graphic>
    {
        [RequiredField, CheckForComponent(typeof(Graphic)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Color of the UI component")]
        public FsmColor color;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Graphic uiComponent;

        private void DoGetColorValue()
        {
            if (this.uiComponent != null)
            {
                this.color.set_Value(this.uiComponent.color);
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiComponent = base.cachedComponent;
            }
            this.DoGetColorValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetColorValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.color = null;
        }
    }
}

