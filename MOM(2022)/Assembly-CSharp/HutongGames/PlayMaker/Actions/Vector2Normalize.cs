namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Normalizes a Vector2 Variable.")]
    public class Vector2Normalize : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The vector to normalize")]
        public FsmVector2 vector2Variable;
        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value().normalized);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.set_Value(this.vector2Variable.get_Value().normalized);
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.everyFrame = false;
        }
    }
}

