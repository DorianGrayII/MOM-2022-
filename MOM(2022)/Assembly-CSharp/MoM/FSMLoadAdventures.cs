using HutongGames.PlayMaker;
using MOM.Adventures;
using UnityEngine;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMLoadAdventures : FSMStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            AdventureLibrary.LoadModulesFromDrive(ModulesReady, ModulesLoadingFailed);
        }

        private void ModulesLoadingFailed(object o)
        {
            Debug.LogError("Loading modules failed! \n" + o);
        }

        private void ModulesReady(object o)
        {
            AdventureLibrary.currentLibrary = o as AdventureLibrary;
            base.Finish();
        }
    }
}
