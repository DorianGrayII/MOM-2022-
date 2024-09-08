using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Gets the Right n characters from a String.")]
    public class GetStringRight : FsmStateAction
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

        private void DoGetStringRight()
        {
            if (!this.stringVariable.IsNone && !this.storeResult.IsNone)
            {
                string value = this.stringVariable.Value;
                int num = Mathf.Clamp(this.charCount.Value, 0, value.Length);
                this.storeResult.Value = value.Substring(value.Length - num, num);
            }
        }
    }
}
