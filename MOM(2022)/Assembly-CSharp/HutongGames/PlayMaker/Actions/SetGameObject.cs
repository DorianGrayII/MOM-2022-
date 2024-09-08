namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GameObject), Tooltip("Sets the value of a Game Object Variable.")]
    public class SetGameObject : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmGameObject variable;
        public FsmGameObject gameObject;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.variable.set_Value(this.gameObject.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.variable.set_Value(this.gameObject.get_Value());
        }

        public override void Reset()
        {
            this.variable = null;
            this.gameObject = null;
            this.everyFrame = false;
        }
    }
}

