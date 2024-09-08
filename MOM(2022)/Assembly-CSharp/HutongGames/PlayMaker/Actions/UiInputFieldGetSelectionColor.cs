using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the selection color of a UI InputField component. This is the color of the highlighter to show what characters are selected")]
    public class UiInputFieldGetSelectionColor : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("This is the color of the highlighter to show what characters are selected of the UI InputField component.")]
        public FsmColor selectionColor;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private InputField inputField;

        public override void Reset()
        {
            this.selectionColor = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.DoGetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                this.selectionColor.Value = this.inputField.selectionColor;
            }
        }
    }
}
