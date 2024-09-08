namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of an Integer Variable in the PlayMaker Log Window.")]
    public class DebugInt : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [UIHint(UIHint.Variable), Tooltip("The Int variable to debug.")]
        public FsmInt intVariable;

        public override void OnEnter()
        {
            string text = "None";
            if (!this.intVariable.IsNone)
            {
                text = this.intVariable.Name + ": " + this.intVariable.Value.ToString();
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.intVariable = null;
        }
    }
}

