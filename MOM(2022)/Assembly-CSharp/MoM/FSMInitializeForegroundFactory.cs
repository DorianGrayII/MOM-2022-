namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMInitializeForegroundFactory : FSMStateBase
    {
        public override void OnEnter()
        {
            new GameObject("ForegroundFactory").AddComponent<ForegroundFactory>().PrepareFromCache();
            base.OnEnter();
            MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            base.Finish();
        }
    }
}

