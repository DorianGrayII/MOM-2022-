namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the navigation mode of a UI Selectable component.")]
    public class UiNavigationSetMode : ComponentAction<Selectable>
    {
        [RequiredField, CheckForComponent(typeof(Selectable)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The navigation mode value")]
        public Navigation.Mode navigationMode;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Selectable selectable;
        private Navigation _navigation;
        private Navigation.Mode originalValue;

        private void DoSetValue()
        {
            if (this.selectable != null)
            {
                this._navigation = this.selectable.navigation;
                this._navigation.mode = this.navigationMode;
                this.selectable.navigation = this._navigation;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.selectable = base.cachedComponent;
            }
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this.originalValue = this.selectable.navigation.mode;
            }
            this.DoSetValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.selectable != null) && this.resetOnExit.Value)
            {
                this._navigation = this.selectable.navigation;
                this._navigation.mode = this.originalValue;
                this.selectable.navigation = this._navigation;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.navigationMode = Navigation.Mode.Automatic;
            this.resetOnExit = false;
        }
    }
}

