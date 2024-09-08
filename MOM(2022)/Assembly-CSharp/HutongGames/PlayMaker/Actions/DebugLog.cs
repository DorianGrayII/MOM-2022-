namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Sends a log message to the PlayMaker Log Window.")]
    public class DebugLog : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [Tooltip("Text to send to the log.")]
        public FsmString text;

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(this.text.Value))
            {
                ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.text.Value, base.sendToUnityLog);
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.text = "";
            base.Reset();
        }
    }
}

