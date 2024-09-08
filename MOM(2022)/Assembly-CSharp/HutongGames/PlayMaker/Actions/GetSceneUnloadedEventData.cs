using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get the last Unloaded Scene Event data when event was sent from the action 'SendSceneUnloadedEvent")]
    public class GetSceneUnloadedEventData : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("The scene name")]
        public FsmString name;

        [Tooltip("The scene path")]
        [UIHint(UIHint.Variable)]
        public FsmString path;

        [Tooltip("The scene Build Index")]
        [UIHint(UIHint.Variable)]
        public FsmInt buildIndex;

        [Tooltip("true if the scene is valid.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isValid;

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

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        private Scene _scene;

        public override void Reset()
        {
            this.name = null;
            this.path = null;
            this.buildIndex = null;
            this.isLoaded = null;
            this.rootCount = null;
            this.rootGameObjects = null;
            this.isDirty = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetSceneProperties();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSceneProperties();
        }

        private void DoGetSceneProperties()
        {
            this._scene = SendSceneUnloadedEvent.lastUnLoadedScene;
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
