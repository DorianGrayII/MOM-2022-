namespace MOM
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMExitGame : FSMStateBase
    {
        public override void OnEnter()
        {
            Application.Quit();
            base.OnEnter();
        }
    }
}

