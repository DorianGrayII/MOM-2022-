namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the text value of a UI InputField component as an Int.")]
    public class UiInputFieldGetTextAsInt : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the text value as an int.")]
        public FsmInt value;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if text resolves to an int")]
        public FsmBool isInt;
        [HutongGames.PlayMaker.Tooltip("Event to send if text resolves to an int")]
        public FsmEvent isIntEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if text does NOT resolve to an int")]
        public FsmEvent isNotIntEvent;
        public bool everyFrame;
        private InputField inputField;
        private int _value;
        private bool _success;

        private void DoGetTextValue()
        {
            if (this.inputField != null)
            {
                this._success = int.TryParse(this.inputField.text, out this._value);
                this.value.Value = this._value;
                this.isInt.Value = this._success;
                base.Fsm.Event(this._success ? this.isIntEvent : this.isNotIntEvent);
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.DoGetTextValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetTextValue();
        }

        public override void Reset()
        {
            this.value = null;
            this.isInt = null;
            this.isIntEvent = null;
            this.isNotIntEvent = null;
            this.everyFrame = false;
        }
    }
}

