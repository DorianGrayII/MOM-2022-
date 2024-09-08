using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Sends Events based on the comparison of 2 Colors.")]
    public class ColorCompare : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The first Color.")]
        public FsmColor color1;

        [RequiredField]
        [Tooltip("The second Color.")]
        public FsmColor color2;

        [RequiredField]
        [Tooltip("Tolerance of test, to test for 'almost equals' or to ignore small floating point rounding differences.")]
        public FsmFloat tolerance;

        [Tooltip("Event sent if Color 1 equals Color 2 (within Tolerance)")]
        public FsmEvent equal;

        [Tooltip("Event sent if Color 1 does not equal Color 2 (within Tolerance)")]
        public FsmEvent notEqual;

        [Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.color1 = Color.white;
            this.color2 = Color.white;
            this.tolerance = 0f;
            this.equal = null;
            this.notEqual = null;
            this.everyFrame = false;
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

        private void DoCompare()
        {
            if (Mathf.Abs(this.color1.Value.r - this.color2.Value.r) > this.tolerance.Value || Mathf.Abs(this.color1.Value.g - this.color2.Value.g) > this.tolerance.Value || Mathf.Abs(this.color1.Value.b - this.color2.Value.b) > this.tolerance.Value || Mathf.Abs(this.color1.Value.a - this.color2.Value.a) > this.tolerance.Value)
            {
                base.Fsm.Event(this.notEqual);
            }
            else
            {
                base.Fsm.Event(this.equal);
            }
        }

        public override string ErrorCheck()
        {
            if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.notEqual))
            {
                return "Action sends no events!";
            }
            return "";
        }
    }
}
