namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Subtracts a Vector2 value from a Vector2 variable.")]
    public class Vector2Subtract : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Vector2 operand")]
        public FsmVector2 vector2Variable;
        [RequiredField, Tooltip("The vector2 to subtract with")]
        public FsmVector2 subtractVector;
        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() - this.subtractVector.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() - this.subtractVector.get_Value());
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.subtractVector = vector1;
            this.everyFrame = false;
        }
    }
}

