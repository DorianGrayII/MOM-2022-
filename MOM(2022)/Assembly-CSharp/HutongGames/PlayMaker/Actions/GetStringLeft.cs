using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Gets the Left n characters from a String Variable.")]
    public class GetStringLeft : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString stringVariable;

        [Tooltip("Number of characters to get.")]
        public FsmInt charCount;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.stringVariable = null;
            this.charCount = 0;
            this.storeResult = null;
            this.everyFrame = false;
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

        private void DoGetStringLeft()
        {
            if (!this.stringVariable.IsNone && !this.storeResult.IsNone)
            {
                this.storeResult.Value = this.stringVariable.Value.Substring(0, Mathf.Clamp(this.charCount.Value, 0, this.stringVariable.Value.Length));
            }
        }
    }
}
