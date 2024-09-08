using HutongGames.PlayMaker;
using MHUtils;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMLoadSettings : FSMStateBase
    {
        public override void OnEnter()
        {
            this.LoadScripts();
            base.OnEnter();
        }

        private void LoadScripts()
        {
            Settings.GetData();
            Settings.ApplyVisualSettings();
            AchievementManager.Load();
            MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
            base.Finish();
        }
    }
}
