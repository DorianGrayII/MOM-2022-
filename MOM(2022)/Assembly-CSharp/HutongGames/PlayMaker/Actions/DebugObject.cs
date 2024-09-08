namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of an Object Variable in the PlayMaker Log Window.")]
    public class DebugObject : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [UIHint(UIHint.Variable), Tooltip("The Object variable to debug.")]
        public FsmObject fsmObject;

        public override void OnEnter()
        {
            string text = "None";
            if (!this.fsmObject.IsNone)
            {
                string text1;
                if (this.fsmObject != null)
                {
                    text1 = this.fsmObject.ToString();
                }
                else
                {
                    FsmObject fsmObject = this.fsmObject;
                    text1 = null;
                }
                text = this.fsmObject.Name + ": " + text1;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.fsmObject = null;
            base.Reset();
        }
    }
}

