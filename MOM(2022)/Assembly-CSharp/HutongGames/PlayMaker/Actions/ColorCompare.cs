namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Logic), HutongGames.PlayMaker.Tooltip("Sends Events based on the comparison of 2 Colors.")]
    public class ColorCompare : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The first Color.")]
        public FsmColor color1;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The second Color.")]
        public FsmColor color2;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Tolerance of test, to test for 'almost equals' or to ignore small floating point rounding differences.")]
        public FsmFloat tolerance;
        [HutongGames.PlayMaker.Tooltip("Event sent if Color 1 equals Color 2 (within Tolerance)")]
        public FsmEvent equal;
        [HutongGames.PlayMaker.Tooltip("Event sent if Color 1 does not equal Color 2 (within Tolerance)")]
        public FsmEvent notEqual;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
        public bool everyFrame;

        private void DoCompare()
        {
            if ((Mathf.Abs((float) (this.color1.get_Value().r - this.color2.get_Value().r)) <= this.tolerance.Value) && ((Mathf.Abs((float) (this.color1.get_Value().g - this.color2.get_Value().g)) <= this.tolerance.Value) && ((Mathf.Abs((float) (this.color1.get_Value().b - this.color2.get_Value().b)) <= this.tolerance.Value) && (Mathf.Abs((float) (this.color1.get_Value().a - this.color2.get_Value().a)) <= this.tolerance.Value))))
            {
                base.Fsm.Event(this.equal);
            }
            else
            {
                base.Fsm.Event(this.notEqual);
            }
        }

        public override string ErrorCheck()
        {
            return ((!FsmEvent.IsNullOrEmpty(this.equal) || !FsmEvent.IsNullOrEmpty(this.notEqual)) ? "" : "Action sends no events!");
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
            this.color1 = (FsmColor) Color.white;
            this.color2 = (FsmColor) Color.white;
            this.tolerance = 0f;
            this.equal = null;
            this.notEqual = null;
            this.everyFrame = false;
        }
    }
}

