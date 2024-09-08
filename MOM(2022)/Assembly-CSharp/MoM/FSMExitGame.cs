using HutongGames.PlayMaker;
using UnityEngine;

namespace MOM
{
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
