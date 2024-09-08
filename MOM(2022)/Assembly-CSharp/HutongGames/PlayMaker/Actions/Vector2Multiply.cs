namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Multiplies a Vector2 variable by a Float.")]
    public class Vector2Multiply : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The vector to Multiply")]
        public FsmVector2 vector2Variable;
        [RequiredField, Tooltip("The multiplication factor")]
        public FsmFloat multiplyBy;
        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() * this.multiplyBy.Value);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() * this.multiplyBy.Value);
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.multiplyBy = 1f;
            this.everyFrame = false;
        }
    }
}

