namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Camera), ActionTarget(typeof(Camera), "storeGameObject", false), HutongGames.PlayMaker.Tooltip("Gets the GameObject tagged MainCamera from the scene")]
    public class GetMainCamera : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmGameObject storeGameObject;

        public override void OnEnter()
        {
            this.storeGameObject.set_Value(Camera.main?.gameObject);
            base.Finish();
        }

        public override void Reset()
        {
            this.storeGameObject = null;
        }
    }
}

