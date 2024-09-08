using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get the last Loaded Scene Event data when event was sent from the action 'SendSceneLoadedEvent")]
    public class GetSceneLoadedEventData : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("The scene loaded mode")]
        [ObjectType(typeof(LoadSceneMode))]
        public FsmEnum loadedMode;

        [UIHint(UIHint.Variable)]
        [Tooltip("The scene name")]
        public FsmString name;

        [Tooltip("The scene path")]
        [UIHint(UIHint.Variable)]
        public FsmString path;

        [Tooltip("true if the scene is valid.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isValid;

        [Tooltip("The scene Build Index")]
        [UIHint(UIHint.Variable)]
        public FsmInt buildIndex;

        [Tooltip("true if the scene is loaded.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isLoaded;

        [UIHint(UIHint.Variable)]
        [Tooltip("true if the scene is modified.")]
        public FsmBool isDirty;

        [Tooltip("The scene RootCount")]
        [UIHint(UIHint.Variable)]
        public FsmInt rootCount;

        [Tooltip("The scene Root GameObjects")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
        public FsmArray rootGameObjects;

        private Scene _scene;

        public override void Reset()
        {
            this.loadedMode = null;
            this.name = null;
            this.path = null;
            this.isValid = null;
            this.buildIndex = null;
            this.isLoaded = null;
            this.rootCount = null;
            this.rootGameObjects = null;
            this.isDirty = null;
        }

        public override void OnEnter()
        {
            this.DoGetSceneProperties();
            base.Finish();
        }

        private void DoGetSceneProperties()
        {
            this._scene = SendSceneLoadedEvent.lastLoadedScene;
            if (!this.name.IsNone)
            {
                this.loadedMode.Value = SendSceneLoadedEvent.lastLoadedMode;
            }
            if (!this.name.IsNone)
            {
                this.name.Value = this._scene.name;
            }
            if (!this.buildIndex.IsNone)
            {
                this.buildIndex.Value = this._scene.buildIndex;
            }
            if (!this.path.IsNone)
            {
                this.path.Value = this._scene.path;
            }
            if (!this.isValid.IsNone)
            {
                this.isValid.Value = this._scene.IsValid();
            }
            if (!this.isDirty.IsNone)
            {
                this.isDirty.Value = this._scene.isDirty;
            }
            if (!this.isLoaded.IsNone)
            {
                this.isLoaded.Value = this._scene.isLoaded;
            }
            if (!this.rootCount.IsNone)
            {
                this.rootCount.Value = this._scene.rootCount;
            }
            if (!this.rootGameObjects.IsNone)
            {
                if (this._scene.IsValid())
                {
                    FsmArray fsmArray = this.rootGameObjects;
                    object[] values = this._scene.GetRootGameObjects();
                    fsmArray.Values = values;
                }
                else
                {
                    this.rootGameObjects.Resize(0);
                }
            }
        }
    }
}
