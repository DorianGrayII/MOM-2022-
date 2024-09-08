using HutongGames.PlayMaker;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMGameplay : FSMStateBase
    {
        public static FSMGameplay instance;

        public override void OnEnter()
        {
            FSMGameplay.instance = this;
        }

        public static FSMGameplay Get()
        {
            return FSMGameplay.instance;
        }

        public void HandleEvent(string ev)
        {
            base.Fsm.Event(ev);
        }

        public static void Clear()
        {
            FSMGameplay.instance = null;
        }
    }
}
