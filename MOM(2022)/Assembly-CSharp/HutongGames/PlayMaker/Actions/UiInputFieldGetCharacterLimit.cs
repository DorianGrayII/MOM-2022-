using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the Character Limit value of a UI InputField component. This is the maximum number of characters that the user can type into the field.")]
    public class UiInputFieldGetCharacterLimit : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The maximum number of characters that the user can type into the UI InputField component.")]
        public FsmInt characterLimit;

        [Tooltip("Event sent if limit is infinite (equal to 0)")]
        public FsmEvent hasNoLimitEvent;

        [Tooltip("Event sent if limit is more than 0")]
        public FsmEvent isLimitedEvent;

        [Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;

        private InputField inputField;

        public override void Reset()
        {
            this.characterLimit = null;
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
            if (!(this.inputField == null))
            {
                this.characterLimit.Value = this.inputField.characterLimit;
                base.Fsm.Event((this.inputField.characterLimit > 0) ? this.isLimitedEvent : this.hasNoLimitEvent);
            }
        }
    }
}
