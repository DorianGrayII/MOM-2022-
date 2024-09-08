using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the navigation mode of a UI Selectable component.")]
    public class UiNavigationGetMode : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The navigation mode value")]
        public FsmString navigationMode;

        [Tooltip("Event sent if transition is ColorTint")]
        public FsmEvent automaticEvent;

        [Tooltip("Event sent if transition is ColorTint")]
        public FsmEvent horizontalEvent;

        [Tooltip("Event sent if transition is SpriteSwap")]
        public FsmEvent verticalEvent;

        [Tooltip("Event sent if transition is Animation")]
        public FsmEvent explicitEvent;

        [Tooltip("Event sent if transition is none")]
        public FsmEvent noNavigationEvent;

        private Selectable selectable;

        private Selectable.Transition originalTransition;

        public override void Reset()
        {
            this.gameObject = null;
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
                this.navigationMode.Value = this.selectable.navigation.mode.ToString();
                if (this.selectable.navigation.mode == Navigation.Mode.None)
                {
                    base.Fsm.Event(this.noNavigationEvent);
                }
                else if (this.selectable.navigation.mode == Navigation.Mode.Automatic)
                {
                    base.Fsm.Event(this.automaticEvent);
                }
                else if (this.selectable.navigation.mode == Navigation.Mode.Vertical)
                {
                    base.Fsm.Event(this.verticalEvent);
                }
                else if (this.selectable.navigation.mode == Navigation.Mode.Horizontal)
                {
                    base.Fsm.Event(this.horizontalEvent);
                }
                else if (this.selectable.navigation.mode == Navigation.Mode.Explicit)
                {
                    base.Fsm.Event(this.explicitEvent);
                }
            }
        }
    }
}
