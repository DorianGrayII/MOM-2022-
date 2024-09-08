using HutongGames.PlayMaker;
using MHUtils;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMInitializeScripts : FSMStateBase
    {
        public override void OnEnter()
        {
            this.LoadScripts();
            base.OnEnter();
        }

        private void LoadScripts()
        {
            ScriptLoader.Get().LoadScripts();
            MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
            base.Finish();
        }
    }
}
