using HutongGames.PlayMaker;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMSendEvent : FSMStateBase
    {
        public string action;

        public override void OnEnter()
        {
            base.OnEnter();
            base.Fsm.BroadcastEvent(this.action);
            base.Finish();
        }
    }
}
