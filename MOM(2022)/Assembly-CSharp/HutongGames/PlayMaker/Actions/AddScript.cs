namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Adds a Script to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Script on exiting the state.")]
    public class AddScript : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to add the script to.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The Script to add to the GameObject."), UIHint(UIHint.ScriptComponent)]
        public FsmString script;
        [HutongGames.PlayMaker.Tooltip("Remove the script from the GameObject when this State is exited.")]
        public FsmBool removeOnExit;
        private Component addedComponent;

        private void DoAddComponent(GameObject go)
        {
            this.addedComponent = go.AddComponent(ReflectionUtils.GetGlobalType(this.script.Value));
            if (this.addedComponent == null)
            {
                base.LogError("Can't add script: " + this.script.Value);
            }
        }

        public override void OnEnter()
        {
            this.DoAddComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value());
            base.Finish();
        }

        public override void OnExit()
        {
            if (this.removeOnExit.Value && (this.addedComponent != null))
            {
                UnityEngine.Object.Destroy(this.addedComponent);
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.script = null;
        }
    }
}

