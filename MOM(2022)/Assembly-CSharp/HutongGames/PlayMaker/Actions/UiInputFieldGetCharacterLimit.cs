namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the Character Limit value of a UI InputField component. This is the maximum number of characters that the user can type into the field.")]
    public class UiInputFieldGetCharacterLimit : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The maximum number of characters that the user can type into the UI InputField component.")]
        public FsmInt characterLimit;
        [HutongGames.PlayMaker.Tooltip("Event sent if limit is infinite (equal to 0)")]
        public FsmEvent hasNoLimitEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if limit is more than 0")]
        public FsmEvent isLimitedEvent;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private InputField inputField;

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                this.characterLimit.Value = this.inputField.characterLimit;
                base.Fsm.Event((this.inputField.characterLimit > 0) ? this.isLimitedEvent : this.hasNoLimitEvent);
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
            this.characterLimit = null;
            this.everyFrame = false;
        }
    }
}

