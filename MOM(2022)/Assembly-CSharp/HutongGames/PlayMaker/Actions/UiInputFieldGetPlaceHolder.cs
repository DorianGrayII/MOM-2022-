namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the placeHolder GameObject of a UI InputField component.")]
    public class UiInputFieldGetPlaceHolder : ComponentAction<InputField>
    {
        [RequiredField, CheckForComponent(typeof(InputField)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI InputField component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the placeholder for the UI InputField component.")]
        public FsmGameObject placeHolder;
        [HutongGames.PlayMaker.Tooltip("true if placeholder is found")]
        public FsmBool placeHolderDefined;
        [HutongGames.PlayMaker.Tooltip("Event sent if no placeholder is defined")]
        public FsmEvent foundEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if a placeholder is defined")]
        public FsmEvent notFoundEvent;
        private InputField inputField;

        private void DoGetValue()
        {
            if (this.inputField != null)
            {
                Graphic placeholder = this.inputField.placeholder;
                this.placeHolderDefined.Value = placeholder != null;
                if (placeholder == null)
                {
                    base.Fsm.Event(this.notFoundEvent);
                }
                else
                {
                    this.placeHolder.set_Value(placeholder.gameObject);
                    base.Fsm.Event(this.foundEvent);
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
            this.DoGetValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.placeHolder = null;
            this.placeHolderDefined = null;
            this.foundEvent = null;
            this.notFoundEvent = null;
        }
    }
}

