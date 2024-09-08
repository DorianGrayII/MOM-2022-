namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the focused state of a UI InputField component.")]
    public class UiInputFieldGetIsFocused : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the is focused flag value of the UI InputField component.")]
        public FsmBool isFocused;
        [HutongGames.PlayMaker.Tooltip("Event sent if inputField is focused")]
        public FsmEvent isfocusedEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if nputField is not focused")]
        public FsmEvent isNotFocusedEvent;
        private InputField inputField;

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                this.isFocused.Value = this.inputField.isFocused;
                base.Fsm.Event(this.inputField.isFocused ? this.isfocusedEvent : this.isNotFocusedEvent);
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
            base.Finish();
        }

        public override void Reset()
        {
            this.isFocused = null;
            this.isfocusedEvent = null;
            this.isNotFocusedEvent = null;
        }
    }
}

