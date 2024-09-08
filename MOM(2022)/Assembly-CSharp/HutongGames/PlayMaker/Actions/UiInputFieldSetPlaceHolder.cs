using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the placeholder of a UI InputField component. Optionally reset on exit")]
    public class UiInputFieldSetPlaceHolder : ComponentAction<InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(InputField))]
        [Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [CheckForComponent(typeof(Graphic))]
        [Tooltip("The placeholder (any graphic UI Component) for the UI InputField component.")]
        public FsmGameObject placeholder;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private InputField inputField;

        private Graphic originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.placeholder = null;
            this.resetOnExit = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.inputField = base.cachedComponent;
            }
            this.originalValue = this.inputField.placeholder;
            this.DoSetValue();
            base.Finish();
        }

        private void DoSetValue()
        {
            if (this.inputField != null)
            {
                GameObject value = this.placeholder.Value;
                if (value == null)
                {
                    this.inputField.placeholder = null;
                }
                else
                {
                    this.inputField.placeholder = value.GetComponent<Graphic>();
                }
            }
        }

        public override void OnExit()
        {
            if (!(this.inputField == null) && this.resetOnExit.Value)
            {
                this.inputField.placeholder = this.originalValue;
            }
        }
    }
}
