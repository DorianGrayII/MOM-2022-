namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if 2 Array Variables have the same values.")]
    public class ArrayCompare : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The first Array Variable to test.")]
        public FsmArray array1;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The second Array Variable to test.")]
        public FsmArray array2;

        [Tooltip("Event to send if the 2 arrays have the same values.")]
        public FsmEvent SequenceEqual;

        [Tooltip("Event to send if the 2 arrays have different values.")]
        public FsmEvent SequenceNotEqual;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.array1 = null;
            this.array2 = null;
            this.SequenceEqual = null;
            this.SequenceNotEqual = null;
        }

        public override void OnEnter()
        {
            this.DoSequenceEqual();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSequenceEqual()
        {
            if (this.array1.Values != null && this.array2.Values != null)
            {
                this.storeResult.Value = this.TestSequenceEqual(this.array1.Values, this.array2.Values);
                base.Fsm.Event(this.storeResult.Value ? this.SequenceEqual : this.SequenceNotEqual);
            }
        }

        private bool TestSequenceEqual(object[] _array1, object[] _array2)
        {
            if (_array1.Length != _array2.Length)
            {
                return false;
            }
            for (int i = 0; i < this.array1.Length; i++)
            {
                if (!_array1[i].Equals(_array2[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
