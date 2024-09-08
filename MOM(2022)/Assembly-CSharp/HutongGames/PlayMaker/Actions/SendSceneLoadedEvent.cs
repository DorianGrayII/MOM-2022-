using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Send an event when a scene was loaded. Use the Safe version when you want to access content of the loaded scene. Use GetSceneloadedEventData to find out about the loaded Scene and load mode")]
    public class SendSceneLoadedEvent : FsmStateAction
    {
        [Tooltip("The event to send when a scene was loaded")]
        public FsmEvent sceneLoaded;

        [Tooltip("The event to send when a scene was loaded, with a one frame delay to make sure the scene content was indeed initialized fully")]
        public FsmEvent sceneLoadedSafe;

        public static Scene lastLoadedScene;

        public static LoadSceneMode lastLoadedMode;

        private int _loaded = -1;

        public override void Reset()
        {
            this.sceneLoaded = null;
        }

        public override void OnEnter()
        {
            this._loaded = -1;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SendSceneLoadedEvent.lastLoadedScene = scene;
            SendSceneLoadedEvent.lastLoadedMode = mode;
            base.Fsm.Event(this.sceneLoaded);
            this._loaded = Time.frameCount;
            if (this.sceneLoadedSafe == null)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (this._loaded > -1 && Time.frameCount > this._loaded)
            {
                this._loaded = -1;
                base.Fsm.Event(this.sceneLoadedSafe);
                base.Finish();
            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public override string ErrorCheck()
        {
            if (this.sceneLoaded == null && this.sceneLoadedSafe == null)
            {
                return "At least one event setup is required";
            }
            return string.Empty;
        }
    }
}
