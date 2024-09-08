﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Vector2), Tooltip("Sets the value of a Vector2 Variable.")]
    public class SetVector2Value : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;
        [RequiredField, Tooltip("The vector2 source")]
        public FsmVector2 vector2Value;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.vector2Variable.set_Value(this.vector2Value.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.vector2Variable.set_Value(this.vector2Value.get_Value());
        }

        public override void Reset()
        {
            this.vector2Variable = null;
            this.vector2Value = null;
            this.everyFrame = false;
        }
    }
}

