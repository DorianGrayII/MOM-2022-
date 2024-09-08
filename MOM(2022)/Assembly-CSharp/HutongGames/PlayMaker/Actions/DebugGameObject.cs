namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of a Game Object Variable in the PlayMaker Log Window.")]
    public class DebugGameObject : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [UIHint(UIHint.Variable)]
        [Tooltip("The GameObject variable to debug.")]
        public FsmGameObject gameObject;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.gameObject = null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.gameObject.IsNone)
            {
                text = this.gameObject.Name + ": " + this.gameObject;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }
    }
}
