namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Print the value of any FSM Variable in the PlayMaker Log Window.")]
    public class DebugFsmVariable : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [HideTypeFilter]
        [UIHint(UIHint.Variable)]
        [Tooltip("The variable to debug.")]
        public FsmVar variable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.variable = null;
            base.Reset();
        }

        public override void OnEnter()
        {
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.variable.DebugString(), base.sendToUnityLog);
            base.Finish();
        }
    }
}
