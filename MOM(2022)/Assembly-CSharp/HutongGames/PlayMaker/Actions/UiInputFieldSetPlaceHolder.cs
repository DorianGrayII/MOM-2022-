namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the placeholder of a UI InputField component. Optionally reset on exit")]
    public class UiInputFieldSetPlaceHolder : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, CheckForComponent(typeof(Graphic)), HutongGames.PlayMaker.Tooltip("The placeholder (any graphic UI Component) for the UI InputField component.")]
        public FsmGameObject placeholder;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private InputField inputField;
        private Graphic originalValue;

        private void DoSetValue()
        {
            if (this.inputField != null)
            {
                GameObject obj2 = this.placeholder.get_Value();
                if (obj2 == null)
                {
                    this.inputField.placeholder = null;
                }
                else
                {
                    this.inputField.placeholder = obj2.GetComponent<Graphic>();
                }
            }
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

        public override void OnExit()
        {
            if ((this.inputField != null) && this.resetOnExit.Value)
            {
                this.inputField.placeholder = this.originalValue;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.placeholder = null;
            this.resetOnExit = null;
        }
    }
}

