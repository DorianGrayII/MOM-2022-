namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMLoadSettings : FSMStateBase
    {
        private void LoadScripts()
        {
            Settings.GetData();
            Settings.ApplyVisualSettings();
            AchievementManager.Load();
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

