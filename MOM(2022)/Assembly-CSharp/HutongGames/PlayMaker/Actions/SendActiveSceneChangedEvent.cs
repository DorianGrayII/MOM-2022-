using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Send an event when the active scene has changed.")]
    public class SendActiveSceneChangedEvent : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The event to send when an active scene changed")]
        public FsmEvent activeSceneChanged;

        public static Scene lastPreviousActiveScene;

        public static Scene lastNewActiveScene;

        public override void Reset()
        {
            this.activeSceneChanged = null;
        }

        public override void OnEnter()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            base.Finish();
        }

        private void SceneManager_activeSceneChanged(Scene previousActiveScene, Scene activeScene)
        {
            SendActiveSceneChangedEvent.lastNewActiveScene = activeScene;
            SendActiveSceneChangedEvent.lastPreviousActiveScene = previousActiveScene;
            base.Fsm.Event(this.activeSceneChanged);
            base.Finish();
        }

        public override void OnExit()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }
    }
}
