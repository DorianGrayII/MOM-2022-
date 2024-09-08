namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMInitializeAssetBundles : FSMStateBase
    {
        public override void OnEnter()
        {
            AssetManager.Get().Initialize();
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

