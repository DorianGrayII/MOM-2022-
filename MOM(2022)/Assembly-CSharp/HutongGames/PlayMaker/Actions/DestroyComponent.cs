using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Destroys a Component of an Object.")]
    public class DestroyComponent : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the Component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.ScriptComponent)]
        [Tooltip("The name of the Component to destroy.")]
        public FsmString component;

        private Component aComponent;

        public override void Reset()
        {
            this.aComponent = null;
            this.gameObject = null;
            this.component = null;
        }

        public override void OnEnter()
        {
            this.DoDestroyComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
            base.Finish();
        }

        private void DoDestroyComponent(GameObject go)
        {
            this.aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(this.component.Value));
            if (this.aComponent == null)
            {
                base.LogError("No such component: " + this.component.Value);
            }
            else
            {
                Object.Destroy(this.aComponent);
            }
        }
    }
}
