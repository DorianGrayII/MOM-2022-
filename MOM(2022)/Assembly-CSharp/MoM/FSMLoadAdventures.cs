namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using MOM.Adventures;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMLoadAdventures : FSMStateBase
    {
        private void ModulesLoadingFailed(object o)
        {
            Debug.LogError("Loading modules failed! \n" + o?.ToString());
        }

        private void ModulesReady(object o)
        {
            AdventureLibrary.currentLibrary = o as AdventureLibrary;
            base.Finish();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AdventureLibrary.LoadModulesFromDrive(new Callback(this.ModulesReady), new Callback(this.ModulesLoadingFailed));
        }
    }
}

