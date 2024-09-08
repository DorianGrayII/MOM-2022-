namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Reverses the direction of a Vector2 Variable. Same as multiplying by -1.")]
    public class Vector2Invert : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The vector to invert")]
        public FsmVector2 vector2Variable;
        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() * -1f);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value() * -1f);
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.everyFrame = false;
        }
    }
}

