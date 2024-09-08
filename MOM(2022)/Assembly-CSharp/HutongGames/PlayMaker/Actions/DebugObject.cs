namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of an Object Variable in the PlayMaker Log Window.")]
    public class DebugObject : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [UIHint(UIHint.Variable)]
        [Tooltip("The Object variable to debug.")]
        public FsmObject fsmObject;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.fsmObject = null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.fsmObject.IsNone)
            {
                text = this.fsmObject.Name + ": " + this.fsmObject;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }
    }
}
