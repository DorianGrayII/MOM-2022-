using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using UnityEngine;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMOpenScreen : FSMStateBase
    {
        public ScreenBase screen;

        public bool surviveExit;

        public string screenKillExitOverride;

        private ScreenBase instance;

        private string navigationPath;

        public override void OnEnter()
        {
            if (this.screen == null)
            {
                Debug.LogError("Missing screen instance at " + base.DisplayName);
            }
            this.instance = UIManager.GetOrOpen<ScreenBase>(this.screen.gameObject, UIManager.Layer.Standard);
            MHEventSystem.RegisterListener(this.screen, Navigation, this);
            base.OnEnter();
        }

        public override void OnExit()
        {
            if (!this.surviveExit || this.screenKillExitOverride == this.navigationPath)
            {
                if (this.instance != null)
                {
                    UIManager.Close(this.instance);
                }
                this.instance = null;
            }
            if (this.instance != null)
            {
                MHEventSystem.UnRegisterListener(Navigation);
            }
            base.OnExit();
        }

        private void Navigation(object sender, object e)
        {
            if (e != null)
            {
                this.navigationPath = e.ToString();
                base.Fsm.Event(this.navigationPath);
            }
        }

        public override string GetStageName()
        {
            return "FSM->" + this.screen?.GetType().Name;
        }
    }
}
