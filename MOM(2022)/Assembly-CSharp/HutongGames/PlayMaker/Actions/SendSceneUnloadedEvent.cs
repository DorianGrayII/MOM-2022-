using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Send an event when a scene was unloaded.")]
    public class SendSceneUnloadedEvent : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The event to send when scene was unloaded")]
        public FsmEvent sceneUnloaded;

        public static Scene lastUnLoadedScene;

        public override void Reset()
        {
            this.sceneUnloaded = null;
        }

        public override void OnEnter()
        {
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
            base.Finish();
        }

        private void SceneManager_sceneUnloaded(Scene scene)
        {
            Debug.Log(scene.name);
            SendSceneUnloadedEvent.lastUnLoadedScene = scene;
            base.Fsm.Event(this.sceneUnloaded);
            base.Finish();
        }

        public override void OnExit()
        {
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        }
    }
}
