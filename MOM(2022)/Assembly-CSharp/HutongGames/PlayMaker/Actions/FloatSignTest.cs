﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Logic), Tooltip("Sends Events based on the sign of a Float.")]
    public class FloatSignTest : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The float variable to test.")]
        public FsmFloat floatValue;
        [Tooltip("Event to send if the float variable is positive.")]
        public FsmEvent isPositive;
        [Tooltip("Event to send if the float variable is negative.")]
        public FsmEvent isNegative;
        [Tooltip("Repeat every frame. Useful if the variable is changing and you're waiting for a particular result.")]
        public bool everyFrame;

        private void DoSignTest()
        {
            if (this.floatValue != null)
            {
                base.Fsm.Event((this.floatValue.Value < 0f) ? this.isNegative : this.isPositive);
            }
        }

        public override string ErrorCheck()
        {
            return ((!FsmEvent.IsNullOrEmpty(this.isPositive) || !FsmEvent.IsNullOrEmpty(this.isNegative)) ? "" : "Action sends no events!");
        }

        public override void OnEnter()
        {
            this.DoSignTest();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSignTest();
        }

        public override void Reset()
        {
            this.floatValue = 0f;
            this.isPositive = null;
            this.isNegative = null;
            this.everyFrame = false;
        }
    }
}

