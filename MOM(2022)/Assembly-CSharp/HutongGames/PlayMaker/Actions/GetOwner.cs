namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GameObject), Tooltip("Gets the Game Object that owns the FSM and stores it in a game object variable.")]
    public class GetOwner : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmGameObject storeGameObject;

        public override void OnEnter()
        {
            this.storeGameObject.set_Value(base.get_Owner());
            base.Finish();
        }

        public override void Reset()
        {
            this.storeGameObject = null;
        }
    }
}

