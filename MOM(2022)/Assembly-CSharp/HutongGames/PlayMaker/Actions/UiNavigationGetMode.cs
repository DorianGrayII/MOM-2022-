namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the navigation mode of a UI Selectable component.")]
    public class UiNavigationGetMode : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The navigation mode value")]
        public FsmString navigationMode;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is ColorTint")]
        public FsmEvent automaticEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is ColorTint")]
        public FsmEvent horizontalEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is SpriteSwap")]
        public FsmEvent verticalEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is Animation")]
        public FsmEvent explicitEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if transition is none")]
        public FsmEvent noNavigationEvent;
        private Selectable selectable;
        private Selectable.Transition originalTransition;

        private void DoGetValue()
        {
            if (this.selectable != null)
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
        }
    }
}

