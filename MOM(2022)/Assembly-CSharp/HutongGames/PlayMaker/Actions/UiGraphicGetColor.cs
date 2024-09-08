using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the color of a UI Graphic component. (E.g. UI Sprite)")]
    public class UiGraphicGetColor : ComponentAction<Graphic>
    {
        [RequiredField]
        [CheckForComponent(typeof(Graphic))]
        [Tooltip("The GameObject with the UI component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Color of the UI component")]
        public FsmColor color;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Graphic uiComponent;

        public override void Reset()
        {
            this.gameObject = null;
            this.color = null;
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

        private void DoGetColorValue()
        {
            if (this.uiComponent != null)
            {
                this.color.Value = this.uiComponent.color;
            }
        }
    }
}
