namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets a Game Object's Transform and stores it in an Object Variable.")]
    public class GetTransform : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject gameObject;
        [RequiredField, UIHint(UIHint.Variable), ObjectType(typeof(Transform))]
        public FsmObject storeTransform;
        public bool everyFrame;

        private void DoGetGameObjectName()
        {
            GameObject obj2 = this.gameObject.get_Value();
            this.storeTransform.set_Value(obj2?.transform);
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
            this.storeTransform = null;
            this.everyFrame = false;
        }
    }
}

