namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMLoadAudio : FSMStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            AudioLibrary local1 = new GameObject("Audio Library").AddComponent<AudioLibrary>();
            local1.StartCoroutine(local1.LoadingSFX());
            local1.StartCoroutine(local1.LoadingMusic());
            MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
            base.Finish();
        }
    }
}

