﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Sends Events based on the comparison of 2 Floats.")]
    public class FloatCompare : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The first float variable.")]
        public FsmFloat float1;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The second float variable.")]
        public FsmFloat float2;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
        public FsmFloat tolerance;
        [HutongGames.PlayMaker.Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
        public FsmEvent equal;
        [HutongGames.PlayMaker.Tooltip("Event sent if Float 1 is less than Float 2")]
        public FsmEvent lessThan;
        [HutongGames.PlayMaker.Tooltip("Event sent if Float 1 is greater than Float 2")]
        public FsmEvent greaterThan;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
        public bool everyFrame;

        private void DoCompare()
        {
            if (Mathf.Abs((float) (this.float1.Value - this.float2.Value)) <= this.tolerance.Value)
            {
                base.Fsm.Event(this.equal);
            }
            else if (this.float1.Value < this.float2.Value)
            {
                base.Fsm.Event(this.lessThan);
            }
            else if (this.float1.Value > this.float2.Value)
            {
                base.Fsm.Event(this.greaterThan);
            }
        }

        public override string ErrorCheck()
        {
            return ((!FsmEvent.IsNullOrEmpty(this.equal) || (!FsmEvent.IsNullOrEmpty(this.lessThan) || !FsmEvent.IsNullOrEmpty(this.greaterThan))) ? "" : "Action sends no events!");
        }

        public override void OnEnter()
        {
            this.DoCompare();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoCompare();
        }

        public override void Reset()
        {
            this.float1 = 0f;
            this.float2 = 0f;
            this.tolerance = 0f;
            this.equal = null;
            this.lessThan = null;
            this.greaterThan = null;
            this.everyFrame = false;
        }
    }
}

