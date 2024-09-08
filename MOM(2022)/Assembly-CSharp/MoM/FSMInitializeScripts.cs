namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMInitializeScripts : FSMStateBase
    {
        private void LoadScripts()
        {
            ScriptLoader.Get().LoadScripts();
            MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
            base.Finish();
        }

        public override void OnEnter()
        {
            this.LoadScripts();
            base.OnEnter();
        }
    }
}

