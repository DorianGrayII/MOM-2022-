namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector3), Tooltip("Sets the value of a Vector3 Variable.")]
    public class SetVector3Value : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        [RequiredField]
        public FsmVector3 vector3Value;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector3Variable.set_Value(this.vector3Value.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector3Variable.set_Value(this.vector3Value.get_Value());
        }

        public override void Reset()
        {
            this.vector3Variable = null;
            this.vector3Value = null;
            this.everyFrame = false;
        }
    }
}

