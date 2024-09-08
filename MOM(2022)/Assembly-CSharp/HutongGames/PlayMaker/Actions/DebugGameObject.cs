namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of a Game Object Variable in the PlayMaker Log Window.")]
    public class DebugGameObject : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public HutongGames.PlayMaker.LogLevel logLevel;
        [UIHint(UIHint.Variable), Tooltip("The GameObject variable to debug.")]
        public FsmGameObject gameObject;

        public override void OnEnter()
        {
            string text = "None";
            if (!this.gameObject.IsNone)
            {
                string text1;
                if (this.gameObject != null)
                {
                    text1 = this.gameObject.ToString();
                }
                else
                {
                    FsmGameObject gameObject = this.gameObject;
                    text1 = null;
                }
                text = this.gameObject.Name + ": " + text1;
            }
            ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, base.sendToUnityLog);
            base.Finish();
        }

        public override void Reset()
        {
            this.logLevel = HutongGames.PlayMaker.LogLevel.Info;
            this.gameObject = null;
            base.Reset();
        }
    }
}

