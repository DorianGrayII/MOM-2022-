namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    public abstract class BaseLogAction : FsmStateAction
    {
        public bool sendToUnityLog;

        protected BaseLogAction()
        {
        }

        public override void Reset()
        {
            this.sendToUnityLog = false;
        }
    }
}

