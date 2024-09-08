namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of an Enum Variable in the PlayMaker Log Window.")]
    public class DebugEnum : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [UIHint(UIHint.Variable), Tooltip("The Enum Variable to debug.")]
        public FsmEnum enumVariable;

        public override void OnEnter()
        {
            string text = "None";
            if (!this.enumVariable.IsNone)
            {
                string text1;
                Enum enum1 = this.enumVariable.Value;
                if (enum1 != 0)
                {
                    text1 = enum1.ToString();
                }
                else
                {
                    Enum local1 = enum1;
                    text1 = null;
                }
                text = this.enumVariable.Name + ": " + text1;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.enumVariable = null;
            base.Reset();
        }
    }
}

