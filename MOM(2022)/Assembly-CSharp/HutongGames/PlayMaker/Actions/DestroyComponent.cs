namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Destroys a Component of an Object.")]
    public class DestroyComponent : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the Component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.ScriptComponent), HutongGames.PlayMaker.Tooltip("The name of the Component to destroy.")]
        public FsmString component;
        private Component aComponent;

        private void DoDestroyComponent(GameObject go)
        {
            this.aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(this.component.Value));
            if (this.aComponent == null)
            {
                base.LogError("No such component: " + this.component.Value);
            }
            else
            {
                UnityEngine.Object.Destroy(this.aComponent);
            }
        }

        public override void OnEnter()
        {
            this.DoDestroyComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
            base.Finish();
        }

        public override void Reset()
        {
            this.aComponent = null;
            this.gameObject = null;
            this.component = null;
        }
    }
}

