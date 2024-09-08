using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Move Caret to text end in a UI InputField component. Optionally select from the current caret position")]
    public class UiInputFieldMoveCaretToTextEnd : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Define if we select or not from the current caret position. Default is true = no selection")]
        public FsmBool shift;

        private InputField inputField;

        public override void Reset()
        {
            this.gameObject = null;
            this.shift = true;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.DoAction();
            base.Finish();
        }

        private void DoAction()
        {
            if (this.inputField != null)
            {
                this.inputField.MoveTextEnd(this.shift.Value);
            }
        }
    }
}
