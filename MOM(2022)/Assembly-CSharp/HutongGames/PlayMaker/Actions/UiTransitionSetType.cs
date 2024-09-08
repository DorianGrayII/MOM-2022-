using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the transition type of a UI Selectable component.")]
    public class UiTransitionSetType : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The transition value")]
        public Selectable.Transition transition;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Selectable selectable;

        private Selectable.Transition originalTransition;

        public override void Reset()
        {
            this.gameObject = null;
            this.transition = Selectable.Transition.ColorTint;
            this.resetOnExit = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            if (this.selectable != null && this.resetOnExit.Value)
            {
                this.originalTransition = this.selectable.transition;
            }
            this.DoSetValue();
            base.Finish();
        }

        private void DoSetValue()
        {
            if (this.selectable != null)
            {
                this.selectable.transition = this.transition;
            }
        }

        public override void OnExit()
        {
            if (!(this.selectable == null) && this.resetOnExit.Value)
            {
                this.selectable.transition = this.originalTransition;
            }
        }
    }
}
