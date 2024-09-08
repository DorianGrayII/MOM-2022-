namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of a Bool Variable in the PlayMaker Log Window.")]
    public class DebugBool : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [UIHint(UIHint.Variable)]
        [Tooltip("The Bool variable to debug.")]
        public FsmBool boolVariable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.boolVariable = null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.boolVariable.IsNone)
            {
                text = this.boolVariable.Name + ": " + this.boolVariable.Value;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }
    }
}
