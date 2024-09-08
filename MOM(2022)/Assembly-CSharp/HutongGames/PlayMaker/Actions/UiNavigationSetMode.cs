using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the navigation mode of a UI Selectable component.")]
    public class UiNavigationSetMode : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The navigation mode value")]
        public Navigation.Mode navigationMode;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Selectable selectable;

        private Navigation _navigation;

        private Navigation.Mode originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.navigationMode = Navigation.Mode.Automatic;
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
                this.originalValue = this.selectable.navigation.mode;
            }
            this.DoSetValue();
            base.Finish();
        }

        private void DoSetValue()
        {
            if (this.selectable != null)
            {
                this._navigation = this.selectable.navigation;
                this._navigation.mode = this.navigationMode;
                this.selectable.navigation = this._navigation;
            }
        }

        public override void OnExit()
        {
            if (!(this.selectable == null) && this.resetOnExit.Value)
            {
                this._navigation = this.selectable.navigation;
                this._navigation.mode = this.originalValue;
                this.selectable.navigation = this._navigation;
            }
        }
    }
}
