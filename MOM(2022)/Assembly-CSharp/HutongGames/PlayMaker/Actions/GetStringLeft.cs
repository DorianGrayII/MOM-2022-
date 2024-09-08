namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.String), HutongGames.PlayMaker.Tooltip("Gets the Left n characters from a String Variable.")]
    public class GetStringLeft : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString stringVariable;
        [HutongGames.PlayMaker.Tooltip("Number of characters to get.")]
        public FsmInt charCount;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetStringLeft()
        {
            if (!this.stringVariable.IsNone && !this.storeResult.IsNone)
            {
                this.storeResult.Value = this.stringVariable.Value.Substring(0, Mathf.Clamp(this.charCount.Value, 0, this.stringVariable.Value.Length));
            }
        }

        public override void OnEnter()
        {
            this.DoGetStringLeft();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetStringLeft();
        }

        public override void Reset()
        {
            this.stringVariable = null;
            this.charCount = 0;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

