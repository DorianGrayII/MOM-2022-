using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Sets an Integer Variable to a random value between Min/Max.")]
    public class RandomInt : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Minimum value for the random number.")]
        public FsmInt min;

        [RequiredField]
        [Tooltip("Maximim value for the random number.")]
        public FsmInt max;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in an Integer variable.")]
        public FsmInt storeResult;

        [Tooltip("Should the Max value be included in the possible results?")]
        public bool inclusiveMax;

        [Tooltip("Don't repeat the same value twice.")]
        public FsmBool noRepeat;

        private int randomIndex;

        private int lastIndex = -1;

        public override void Reset()
        {
            this.min = 0;
            this.max = 100;
            this.storeResult = null;
            this.inclusiveMax = false;
            this.noRepeat = true;
        }

        public override void OnEnter()
        {
            this.PickRandom();
            base.Finish();
        }

        private void PickRandom()
        {
            if (this.noRepeat.Value && this.max.Value != this.min.Value && !this.inclusiveMax && Mathf.Abs(this.max.Value - this.min.Value) > 1)
            {
                do
                {
                    this.randomIndex = (this.inclusiveMax ? Random.Range(this.min.Value, this.max.Value + 1) : Random.Range(this.min.Value, this.max.Value));
                }
                while (this.randomIndex == this.lastIndex);
                this.lastIndex = this.randomIndex;
                this.storeResult.Value = this.randomIndex;
            }
            else
            {
                this.randomIndex = (this.inclusiveMax ? Random.Range(this.min.Value, this.max.Value + 1) : Random.Range(this.min.Value, this.max.Value));
                this.storeResult.Value = this.randomIndex;
            }
        }
    }
}
