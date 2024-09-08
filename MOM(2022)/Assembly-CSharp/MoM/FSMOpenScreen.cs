namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMOpenScreen : FSMStateBase
    {
        public ScreenBase screen;
        public bool surviveExit;
        public string screenKillExitOverride;
        private ScreenBase instance;
        private string navigationPath;

        public override string GetStageName()
        {
            string name;
            if (this.screen != null)
            {
                name = this.screen.GetType().Name;
            }
            else
            {
                ScreenBase screen = this.screen;
                name = null;
            }
            return ("FSM->" + name);
        }

        private void Navigation(object sender, object e)
        {
            if (e != null)
            {
                this.navigationPath = e.ToString();
                base.Fsm.Event(this.navigationPath);
            }
        }

        public override void OnEnter()
        {
            if (this.screen == null)
            {
                Debug.LogError("Missing screen instance at " + base.DisplayName);
            }
            this.instance = UIManager.GetOrOpen<ScreenBase>(this.screen.gameObject, UIManager.Layer.Standard, null);
            MHEventSystem.RegisterListener(this.screen, new EventFunction(this.Navigation), this);
            base.OnEnter();
        }

        public override void OnExit()
        {
            if (!this.surviveExit || (this.screenKillExitOverride == this.navigationPath))
            {
                if (this.instance != null)
                {
                    UIManager.Close(this.instance);
                }
                this.instance = null;
            }
            if (this.instance != null)
            {
                MHEventSystem.UnRegisterListener(new EventFunction(this.Navigation));
            }
            base.OnExit();
        }
    }
}

