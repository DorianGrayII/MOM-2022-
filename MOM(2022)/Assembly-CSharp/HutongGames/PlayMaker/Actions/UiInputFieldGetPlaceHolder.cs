using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the placeHolder GameObject of a UI InputField component.")]
    public class UiInputFieldGetPlaceHolder : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the placeholder for the UI InputField component.")]
        public FsmGameObject placeHolder;

        [Tooltip("true if placeholder is found")]
        public FsmBool placeHolderDefined;

        [Tooltip("Event sent if no placeholder is defined")]
        public FsmEvent foundEvent;

        [Tooltip("Event sent if a placeholder is defined")]
        public FsmEvent notFoundEvent;

        private InputField inputField;

        public override void Reset()
        {
            this.placeHolder = null;
            this.placeHolderDefined = null;
            this.foundEvent = null;
            this.notFoundEvent = null;
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

        private void DoGetValue()
        {
            if (!(this.inputField == null))
            {
                Graphic placeholder = this.inputField.placeholder;
                this.placeHolderDefined.Value = placeholder != null;
                if (placeholder != null)
                {
                    this.placeHolder.Value = placeholder.gameObject;
                    base.Fsm.Event(this.foundEvent);
                }
                else
                {
                    base.Fsm.Event(this.notFoundEvent);
                }
            }
        }
    }
}
