namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Sends a log message to the PlayMaker Log Window.")]
    public class DebugLog : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [Tooltip("Text to send to the log.")]
        public FsmString text;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.text = "";
            base.Reset();
        }

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(this.text.Value))
            {
                ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.text.Value, base.sendToUnityLog);
            }
            base.Finish();
        }
    }
}
