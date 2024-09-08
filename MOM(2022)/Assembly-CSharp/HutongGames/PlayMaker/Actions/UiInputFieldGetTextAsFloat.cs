namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the text value of a UI InputField component as a float.")]
    public class UiInputFieldGetTextAsFloat : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The text value as a float of the UI InputField component.")]
        public FsmFloat value;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("true if text resolves to a float")]
        public FsmBool isFloat;
        [HutongGames.PlayMaker.Tooltip("true if text resolves to a float")]
        public FsmEvent isFloatEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if text does not resolves to a float")]
        public FsmEvent isNotFloatEvent;
        public bool everyFrame;
        private InputField inputField;
        private float _value;
        private bool _success;

        private void DoGetTextValue()
        {
            if (this.inputField != null)
            {
                this._success = float.TryParse(this.inputField.text, out this._value);
                this.value.Value = this._value;
                this.isFloat.Value = this._success;
                base.Fsm.Event(this._success ? this.isFloatEvent : this.isNotFloatEvent);
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
            this.isFloat = null;
            this.isFloatEvent = null;
            this.isNotFloatEvent = null;
            this.everyFrame = false;
        }
    }
}

