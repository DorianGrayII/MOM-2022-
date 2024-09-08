namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the caret's blink rate of a UI InputField component.")]
    public class UiInputFieldGetCaretBlinkRate : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The caret's blink rate for the UI InputField component.")]
        public FsmFloat caretBlinkRate;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private InputField inputField;

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                this.caretBlinkRate.Value = this.inputField.caretBlinkRate;
            }
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

        public override void Reset()
        {
            this.caretBlinkRate = null;
            this.everyFrame = false;
        }
    }
}

