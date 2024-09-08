namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Move Caret to text end in a UI InputField component. Optionally select from the current caret position")]
    public class UiInputFieldMoveCaretToTextEnd : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Define if we select or not from the current caret position. Default is true = no selection")]
        public FsmBool shift;
        private InputField inputField;

        private void DoAction()
        {
            if (this.inputField != null)
            {
                this.inputField.MoveTextEnd(this.shift.Value);
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.shift = true;
        }
    }
}

