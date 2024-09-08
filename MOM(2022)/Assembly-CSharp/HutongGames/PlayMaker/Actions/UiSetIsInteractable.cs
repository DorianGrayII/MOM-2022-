namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the interactable flag of a UI Selectable component.")]
    public class UiSetIsInteractable : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Interactable value")]
        public FsmBool isInteractable;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Selectable _selectable;
        private bool _originalState;

        private void DoSetValue()
        {
            if (this._selectable != null)
            {
                this._selectable.interactable = this.isInteractable.Value;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._selectable = ownerDefaultTarget.GetComponent<Selectable>();
            }
            if ((this._selectable != null) && this.resetOnExit.Value)
            {
                this._originalState = this._selectable.IsInteractable();
            }
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this._selectable != null) && this.resetOnExit.Value)
            {
                this._selectable.interactable = this._originalState;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isInteractable = null;
            this.resetOnExit = false;
        }
    }
}

