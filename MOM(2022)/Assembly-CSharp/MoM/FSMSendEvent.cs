namespace MOM
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMSendEvent : FSMStateBase
    {
        public string action;

        public override void OnEnter()
        {
            base.OnEnter();
            base.Fsm.BroadcastEvent(this.action, false);
            base.Finish();
        }
    }
}

