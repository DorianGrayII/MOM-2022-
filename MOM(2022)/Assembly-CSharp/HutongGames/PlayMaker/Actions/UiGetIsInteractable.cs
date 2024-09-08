using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the interactable flag of a UI Selectable component.")]
    public class UiGetIsInteractable : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Interactable value")]
        [UIHint(UIHint.Variable)]
        public FsmBool isInteractable;

        [Tooltip("Event sent if Component is Interactable")]
        public FsmEvent isInteractableEvent;

        [Tooltip("Event sent if Component is not Interactable")]
        public FsmEvent isNotInteractableEvent;

        private Selectable selectable;

        private bool originalState;

        public override void Reset()
        {
            this.gameObject = null;
            this.isInteractable = null;
            this.isInteractableEvent = null;
            this.isNotInteractableEvent = null;
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

        private void DoGetValue()
        {
            if (!(this.selectable == null))
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
    }
}
