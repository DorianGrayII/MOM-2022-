using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the explicit navigation properties of a UI Selectable component. Note that it will have no effect until Navigation mode is set to 'Explicit'.")]
    public class UiNavigationExplicitSetProperties : ComponentAction<Selectable>
    {
        [RequiredField]
        [CheckForComponent(typeof(Selectable))]
        [Tooltip("The GameObject with the UI Selectable component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The down Selectable. Leave as None for no effect")]
        [CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnDown;

        [Tooltip("The up Selectable.  Leave as None for no effect")]
        [CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnUp;

        [Tooltip("The left Selectable.  Leave as None for no effect")]
        [CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnLeft;

        [Tooltip("The right Selectable.  Leave as None for no effect")]
        [CheckForComponent(typeof(Selectable))]
        public FsmGameObject selectOnRight;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Selectable selectable;

        private Navigation navigation;

        private Navigation originalState;

        public override void Reset()
        {
            this.gameObject = null;
            this.selectOnDown = new FsmGameObject
            {
                UseVariable = true
            };
            this.selectOnUp = new FsmGameObject
            {
                UseVariable = true
            };
            this.selectOnLeft = new FsmGameObject
            {
                UseVariable = true
            };
            this.selectOnRight = new FsmGameObject
            {
                UseVariable = true
            };
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
                this.originalState = this.selectable.navigation;
            }
            this.DoSetValue();
            base.Finish();
        }

        private void DoSetValue()
        {
            if (!(this.selectable == null))
            {
                this.navigation = this.selectable.navigation;
                if (!this.selectOnDown.IsNone)
                {
                    this.navigation.selectOnDown = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnDown);
                }
                if (!this.selectOnUp.IsNone)
                {
                    this.navigation.selectOnUp = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnUp);
                }
                if (!this.selectOnLeft.IsNone)
                {
                    this.navigation.selectOnLeft = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnLeft);
                }
                if (!this.selectOnRight.IsNone)
                {
                    this.navigation.selectOnRight = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnRight);
                }
                this.selectable.navigation = this.navigation;
            }
        }

        public override void OnExit()
        {
            if (!(this.selectable == null) && this.resetOnExit.Value)
            {
                this.navigation = this.selectable.navigation;
                this.navigation.selectOnDown = this.originalState.selectOnDown;
                this.navigation.selectOnLeft = this.originalState.selectOnLeft;
                this.navigation.selectOnRight = this.originalState.selectOnRight;
                this.navigation.selectOnUp = this.originalState.selectOnUp;
                this.selectable.navigation = this.navigation;
            }
        }

        private static T GetComponentFromFsmGameObject<T>(FsmGameObject variable) where T : Component
        {
            if (variable.Value != null)
            {
                return variable.Value.GetComponent<T>();
            }
            return null;
        }
    }
}
