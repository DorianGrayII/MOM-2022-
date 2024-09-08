namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector3), Tooltip("Subtracts a Vector3 value from a Vector3 variable.")]
    public class Vector3Subtract : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        [RequiredField]
        public FsmVector3 subtractVector;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector3Variable.set_Value(this.vector3Variable.get_Value() - this.subtractVector.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector3Variable.set_Value(this.vector3Variable.get_Value() - this.subtractVector.get_Value());
        }

        public override void Reset()
        {
            this.vector3Variable = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.subtractVector = vector1;
            this.everyFrame = false;
        }
    }
}

