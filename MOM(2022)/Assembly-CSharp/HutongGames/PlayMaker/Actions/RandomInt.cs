namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Sets an Integer Variable to a random value between Min/Max.")]
    public class RandomInt : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Minimum value for the random number.")]
        public FsmInt min;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Maximim value for the random number.")]
        public FsmInt max;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in an Integer variable.")]
        public FsmInt storeResult;
        [HutongGames.PlayMaker.Tooltip("Should the Max value be included in the possible results?")]
        public bool inclusiveMax;
        [HutongGames.PlayMaker.Tooltip("Don't repeat the same value twice.")]
        public FsmBool noRepeat;
        private int randomIndex;
        private int lastIndex = -1;

        public override void OnEnter()
        {
            this.PickRandom();
            base.Finish();
        }

        private void PickRandom()
        {
            if (!this.noRepeat.Value || ((this.max.Value == this.min.Value) || (this.inclusiveMax || (Mathf.Abs((int) (this.max.Value - this.min.Value)) <= 1))))
            {
                this.randomIndex = this.inclusiveMax ? UnityEngine.Random.Range(this.min.Value, this.max.Value + 1) : UnityEngine.Random.Range(this.min.Value, this.max.Value);
                this.storeResult.Value = this.randomIndex;
            }
            else
            {
                while (true)
                {
                    this.randomIndex = this.inclusiveMax ? UnityEngine.Random.Range(this.min.Value, this.max.Value + 1) : UnityEngine.Random.Range(this.min.Value, this.max.Value);
                    if (this.randomIndex != this.lastIndex)
                    {
                        this.lastIndex = this.randomIndex;
                        this.storeResult.Value = this.randomIndex;
                        return;
                    }
                }
            }
        }

        public override void Reset()
        {
            this.min = 0;
            this.max = 100;
            this.storeResult = null;
            this.inclusiveMax = false;
            this.noRepeat = true;
        }
    }
}

