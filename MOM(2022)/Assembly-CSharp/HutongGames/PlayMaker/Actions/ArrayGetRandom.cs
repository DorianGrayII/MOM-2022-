namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Array), HutongGames.PlayMaker.Tooltip("Get a Random item from an Array.")]
    public class ArrayGetRandom : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Array to use.")]
        public FsmArray array;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the value in a variable."), MatchElementType("array")]
        public FsmVar storeValue;
        [HutongGames.PlayMaker.Tooltip("The index of the value in the array."), UIHint(UIHint.Variable)]
        public FsmInt index;
        [HutongGames.PlayMaker.Tooltip("Don't get the same item twice in a row.")]
        public FsmBool noRepeat;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private int randomIndex;
        private int lastIndex = -1;

        private void DoGetRandomValue()
        {
            if (!this.storeValue.IsNone)
            {
                if (!this.noRepeat.Value || (this.array.Length == 1))
                {
                    this.randomIndex = UnityEngine.Random.Range(0, this.array.Length);
                }
                else
                {
                    while (true)
                    {
                        this.randomIndex = UnityEngine.Random.Range(0, this.array.Length);
                        if (this.randomIndex != this.lastIndex)
                        {
                            this.lastIndex = this.randomIndex;
                            break;
                        }
                    }
                }
                this.index.Value = this.randomIndex;
                this.storeValue.SetValue(this.array.Get(this.index.Value));
            }
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

        public override void Reset()
        {
            this.array = null;
            this.storeValue = null;
            this.index = null;
            this.everyFrame = false;
            this.noRepeat = false;
        }
    }
}

