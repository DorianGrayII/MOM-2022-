namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the interactable flag of a UI Selectable component.")]
    public class UiGetIsInteractable : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Interactable value"), UIHint(UIHint.Variable)]
        public FsmBool isInteractable;
        [HutongGames.PlayMaker.Tooltip("Event sent if Component is Interactable")]
        public FsmEvent isInteractableEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if Component is not Interactable")]
        public FsmEvent isNotInteractableEvent;
        private Selectable selectable;
        private bool originalState;

        private void DoGetValue()
        {
            if (this.selectable != null)
            {
                bool flag = this.selectable.IsInteractable();
                this.isInteractable.Value = flag;
                if (flag)
                {
                    base.Fsm.Event(this.isInteractableEvent);
                }
                else
                {
                    base.Fsm.Event(this.isNotInteractableEvent);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            this.DoGetValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isInteractable = null;
            this.isInteractableEvent = null;
            this.isNotInteractableEvent = null;
        }
    }
}

