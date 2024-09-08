namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.String), HutongGames.PlayMaker.Tooltip("Gets the Right n characters from a String.")]
    public class GetStringRight : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString stringVariable;
        [HutongGames.PlayMaker.Tooltip("Number of characters to get.")]
        public FsmInt charCount;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoGetStringRight()
        {
            if (!this.stringVariable.IsNone && !this.storeResult.IsNone)
            {
                string str = this.stringVariable.Value;
                int length = Mathf.Clamp(this.charCount.Value, 0, str.Length);
                this.storeResult.Value = str.Substring(str.Length - length, length);
            }
        }

        public override void OnEnter()
        {
            this.DoGetStringRight();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetStringRight();
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

