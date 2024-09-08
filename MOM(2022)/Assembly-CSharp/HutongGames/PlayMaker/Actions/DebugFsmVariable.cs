namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Print the value of any FSM Variable in the PlayMaker Log Window.")]
    public class DebugFsmVariable : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [HideTypeFilter, UIHint(UIHint.Variable), Tooltip("The variable to debug.")]
        public FsmVar variable;

        public override void OnEnter()
        {
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.variable.DebugString(), base.sendToUnityLog);
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.variable = null;
            base.Reset();
        }
    }
}

