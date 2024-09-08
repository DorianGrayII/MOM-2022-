namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets the name of a Game Object and stores it in a String Variable.")]
    public class GetName : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString storeName;
        public bool everyFrame;

        private void DoGetGameObjectName()
        {
            GameObject obj2 = this.gameObject.get_Value();
            this.storeName.Value = (obj2 != null) ? obj2.name : "";
        }

        public override void OnEnter()
        {
            this.DoGetGameObjectName();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetGameObjectName();
        }

        public override void Reset()
        {
            FsmGameObject obj1 = new FsmGameObject();
            obj1.UseVariable = true;
            this.gameObject = obj1;
            this.storeName = null;
            this.everyFrame = false;
        }
    }
}

