namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of a Vector2 Variable in the PlayMaker Log Window.")]
    public class DebugVector2 : FsmStateAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [UIHint(UIHint.Variable)]
        [Tooltip("Prints the value of a Vector2 variable in the PlayMaker log window.")]
        public FsmVector2 vector2Variable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.vector2Variable = null;
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.vector2Variable.IsNone)
            {
                text = this.vector2Variable.Name + ": " + this.vector2Variable.Value.ToString();
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
            base.Finish();
        }
    }
}
