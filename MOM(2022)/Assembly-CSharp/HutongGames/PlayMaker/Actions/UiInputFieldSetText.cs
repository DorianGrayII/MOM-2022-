namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the text value of a UI InputField component.")]
    public class UiInputFieldSetText : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.TextArea), HutongGames.PlayMaker.Tooltip("The text of the UI InputField component.")]
        public FsmString text;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private InputField inputField;
        private string originalString;

        private void DoSetTextValue()
        {
            if (this.inputField != null)
            {
                this.inputField.text = this.text.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.originalString = this.inputField.text;
            this.DoSetTextValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnExit()
        {
            if ((this.inputField != null) && this.resetOnExit.Value)
            {
                this.inputField.text = this.originalString;
            }
        }

        public override void OnUpdate()
        {
            this.DoSetTextValue();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.text = null;
            this.resetOnExit = null;
            this.everyFrame = false;
        }
    }
}

