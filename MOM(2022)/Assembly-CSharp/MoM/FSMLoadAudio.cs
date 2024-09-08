using HutongGames.PlayMaker;
using MHUtils;
using UnityEngine;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMLoadAudio : FSMStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            AudioLibrary audioLibrary = new GameObject("Audio Library").AddComponent<AudioLibrary>();
            audioLibrary.StartCoroutine(audioLibrary.LoadingSFX());
            audioLibrary.StartCoroutine(audioLibrary.LoadingMusic());
            MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
            base.Finish();
        }
    }
}
