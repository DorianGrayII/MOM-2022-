using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Adds a Component to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Component on exiting the state.")]
    public class AddComponent : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to add the Component to.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.ScriptComponent)]
        [Title("Component Type")]
        [Tooltip("The type of Component to add to the Game Object.")]
        public FsmString component;

        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(Component))]
        [Tooltip("Store the component in an Object variable. E.g., to use with Set Property.")]
        public FsmObject storeComponent;

        [Tooltip("Remove the Component when this State is exited.")]
        public FsmBool removeOnExit;

        private Component addedComponent;

        public override void Reset()
        {
            this.gameObject = null;
            this.component = null;
            this.storeComponent = null;
        }

        public override void OnEnter()
        {
            this.DoAddComponent();
            base.Finish();
        }

        public override void OnExit()
        {
            if (this.removeOnExit.Value && this.addedComponent != null)
            {
                Object.Destroy(this.addedComponent);
            }
        }

        private void DoAddComponent()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                this.addedComponent = ownerDefaultTarget.AddComponent(ReflectionUtils.GetGlobalType(this.component.Value));
                this.storeComponent.Value = this.addedComponent;
                if (this.addedComponent == null)
                {
                    base.LogError("Can't add component: " + this.component.Value);
                }
            }
        }
    }
}
