namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of a Float Variable in the PlayMaker Log Window.")]
    public class DebugFloat : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [UIHint(UIHint.Variable)]
        [Tooltip("The Float variable to debug.")]
        public FsmFloat floatVariable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.floatVariable = null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.floatVariable.IsNone)
            {
                text = this.floatVariable.Name + ": " + this.floatVariable.Value;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }
    }
}
