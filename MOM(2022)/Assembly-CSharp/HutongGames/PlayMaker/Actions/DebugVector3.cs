namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of a Vector3 Variable in the PlayMaker Log Window.")]
    public class DebugVector3 : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [UIHint(UIHint.Variable), Tooltip("The Vector3 variable to debug.")]
        public FsmVector3 vector3Variable;

        public override void OnEnter()
        {
            string text = "None";
            if (!this.vector3Variable.IsNone)
            {
                text = this.vector3Variable.Name + ": " + this.vector3Variable.get_Value().ToString();
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.vector3Variable = null;
            base.Reset();
        }
    }
}

