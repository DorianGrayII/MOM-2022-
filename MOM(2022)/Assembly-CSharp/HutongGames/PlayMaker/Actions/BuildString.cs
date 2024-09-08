namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.String), Tooltip("Builds a String from other Strings.")]
    public class BuildString : FsmStateAction
    {
        [RequiredField, Tooltip("Array of Strings to combine.")]
        public FsmString[] stringParts;
        [Tooltip("Separator to insert between each String. E.g. space character.")]
        public FsmString separator;
        [Tooltip("Add Separator to end of built string.")]
        public FsmBool addToEnd;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("Store the final String in a variable.")]
        public FsmString storeResult;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private string result;

        private void DoBuildString()
        {
            if (this.storeResult != null)
            {
                string text3;
                this.result = "";
                for (int i = 0; i < (this.stringParts.Length - 1); i++)
                {
                    string text4;
                    FsmString text1 = this.stringParts[i];
                    if (text1 != null)
                    {
                        text4 = text1.ToString();
                    }
                    else
                    {
                        FsmString local1 = text1;
                        text4 = null;
                    }
                    this.result = this.result + text4;
                    this.result = this.result + this.separator.Value;
                }
                FsmString text2 = this.stringParts[this.stringParts.Length - 1];
                if (text2 != null)
                {
                    text3 = text2.ToString();
                }
                else
                {
                    FsmString local2 = text2;
                    text3 = null;
                }
                this.result = this.result + text3;
                if (this.addToEnd.Value)
                {
                    this.result = this.result + this.separator.Value;
                }
                this.storeResult.Value = this.result;
            }
        }

        public override void OnEnter()
        {
            this.DoBuildString();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoBuildString();
        }

        public override void Reset()
        {
            this.stringParts = new FsmString[3];
            this.separator = null;
            this.addToEnd = true;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

