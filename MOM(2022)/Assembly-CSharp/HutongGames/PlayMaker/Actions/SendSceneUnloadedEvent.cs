namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), HutongGames.PlayMaker.Tooltip("Send an event when a scene was unloaded.")]
    public class SendSceneUnloadedEvent : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The event to send when scene was unloaded")]
        public FsmEvent sceneUnloaded;
        public static Scene lastUnLoadedScene;

        public override void OnEnter()
        {
            SceneManager.sceneUnloaded += new UnityAction<Scene>(this.SceneManager_sceneUnloaded);
            base.Finish();
        }

        public override void OnExit()
        {
            SceneManager.sceneUnloaded -= new UnityAction<Scene>(this.SceneManager_sceneUnloaded);
        }

        public override void Reset()
        {
            this.sceneUnloaded = null;
        }

        private void SceneManager_sceneUnloaded(Scene scene)
        {
            Debug.Log(scene.name);
            lastUnLoadedScene = scene;
            base.Fsm.Event(this.sceneUnloaded);
            base.Finish();
        }
    }
}

