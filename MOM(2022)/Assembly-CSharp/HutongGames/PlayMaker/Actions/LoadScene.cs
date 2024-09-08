using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Loads the scene by its name or index in Build Settings. ")]
    public class LoadScene : FsmStateAction
    {
        [Tooltip("The reference options of the Scene")]
        public GetSceneActionBase.SceneSimpleReferenceOptions sceneReference;

        [Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
        public FsmString sceneByName;

        [Tooltip("The index of the scene to load.")]
        public FsmInt sceneAtIndex;

        [Tooltip("Allows you to specify whether or not to load the scene additively. See LoadSceneMode Unity doc for more information about the options.")]
        [ObjectType(typeof(LoadSceneMode))]
        public FsmEnum loadSceneMode;

        [ActionSection("Result")]
        [Tooltip("True if the scene was loaded")]
        public FsmBool success;

        [Tooltip("Event sent if the scene was loaded")]
        public FsmEvent successEvent;

        [Tooltip("Event sent if a problem occurred, check log for information")]
        public FsmEvent failureEvent;

        public override void Reset()
        {
            this.sceneReference = GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex;
            this.sceneByName = null;
            this.sceneAtIndex = null;
            this.loadSceneMode = null;
            this.success = null;
            this.successEvent = null;
            this.failureEvent = null;
        }

        public override void OnEnter()
        {
            bool flag = this.DoLoadScene();
            if (!this.success.IsNone)
            {
                this.success.Value = flag;
            }
            if (flag)
            {
                base.Fsm.Event(this.successEvent);
            }
            else
            {
                base.Fsm.Event(this.failureEvent);
            }
            base.Finish();
        }

        private bool DoLoadScene()
        {
            if (this.sceneReference == GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex)
            {
                if (SceneManager.GetActiveScene().buildIndex == this.sceneAtIndex.Value)
                {
                    return false;
                }
                SceneManager.LoadScene(this.sceneAtIndex.Value, (LoadSceneMode)(object)this.loadSceneMode.Value);
            }
            else
            {
                if (SceneManager.GetActiveScene().name == this.sceneByName.Value)
                {
                    return false;
                }
                SceneManager.LoadScene(this.sceneByName.Value, (LoadSceneMode)(object)this.loadSceneMode.Value);
            }
            return true;
        }
    }
}
