namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), Tooltip("Send an event when the active scene has changed.")]
    public class SendActiveSceneChangedEvent : FsmStateAction
    {
        [RequiredField, Tooltip("The event to send when an active scene changed")]
        public FsmEvent activeSceneChanged;
        public static Scene lastPreviousActiveScene;
        public static Scene lastNewActiveScene;

        public override void OnEnter()
        {
            SceneManager.activeSceneChanged += new UnityAction<Scene, Scene>(this.SceneManager_activeSceneChanged);
            base.Finish();
        }

        public override void OnExit()
        {
            SceneManager.activeSceneChanged -= new UnityAction<Scene, Scene>(this.SceneManager_activeSceneChanged);
        }

        public override void Reset()
        {
            this.activeSceneChanged = null;
        }

        private void SceneManager_activeSceneChanged(Scene previousActiveScene, Scene activeScene)
        {
            lastNewActiveScene = activeScene;
            lastPreviousActiveScene = previousActiveScene;
            base.Fsm.Event(this.activeSceneChanged);
            base.Finish();
        }
    }
}

