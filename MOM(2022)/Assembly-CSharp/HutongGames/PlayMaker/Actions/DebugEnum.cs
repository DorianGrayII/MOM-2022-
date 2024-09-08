namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of an Enum Variable in the PlayMaker Log Window.")]
    public class DebugEnum : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [UIHint(UIHint.Variable)]
        [Tooltip("The Enum Variable to debug.")]
        public FsmEnum enumVariable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.enumVariable = null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.enumVariable.IsNone)
            {
                text = this.enumVariable.Name + ": " + this.enumVariable.Value;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }
    }
}
