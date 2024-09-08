using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Get a Random item from an Array.")]
    public class ArrayGetRandom : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array to use.")]
        public FsmArray array;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the value in a variable.")]
        [MatchElementType("array")]
        public FsmVar storeValue;

        [Tooltip("The index of the value in the array.")]
        [UIHint(UIHint.Variable)]
        public FsmInt index;

        [Tooltip("Don't get the same item twice in a row.")]
        public FsmBool noRepeat;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private int randomIndex;

        private int lastIndex = -1;

        public override void Reset()
        {
            this.array = null;
            this.storeValue = null;
            this.index = null;
            this.everyFrame = false;
            this.noRepeat = false;
        }

        public override void OnEnter()
        {
            this.DoGetRandomValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetRandomValue();
        }

        private void DoGetRandomValue()
        {
            if (this.storeValue.IsNone)
            {
                return;
            }
            if (!this.noRepeat.Value || this.array.Length == 1)
            {
                this.randomIndex = Random.Range(0, this.array.Length);
            }
            else
            {
                do
                {
                    this.randomIndex = Random.Range(0, this.array.Length);
                }
                while (this.randomIndex == this.lastIndex);
                this.lastIndex = this.randomIndex;
            }
            this.index.Value = this.randomIndex;
            this.storeValue.SetValue(this.array.Get(this.index.Value));
        }
    }
}
