namespace MOM
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMGameplay : FSMStateBase
    {
        public static FSMGameplay instance;

        public static void Clear()
        {
            instance = null;
        }

        public static FSMGameplay Get()
        {
            return instance;
        }

        public void HandleEvent(string ev)
        {
            base.Fsm.Event(ev);
        }

        public override void OnEnter()
        {
            instance = this;
        }
    }
}

