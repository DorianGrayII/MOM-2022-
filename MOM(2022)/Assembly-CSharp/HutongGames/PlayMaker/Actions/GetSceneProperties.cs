namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get a scene isDirty flag. true if the scene is modified. ")]
    public class GetSceneProperties : GetSceneActionBase
    {
        [ActionSection("Result")]
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

        public override void Reset()
        {
            base.Reset();
            this.name = null;
            this.path = null;
            this.buildIndex = null;
            this.isValid = null;
            this.isLoaded = null;
            this.rootCount = null;
            this.rootGameObjects = null;
            this.isDirty = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneProperties();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoGetSceneProperties()
        {
            if (!base._sceneFound)
            {
                return;
            }
            if (!this.name.IsNone)
            {
                this.name.Value = base._scene.name;
            }
            if (!this.buildIndex.IsNone)
            {
                this.buildIndex.Value = base._scene.buildIndex;
            }
            if (!this.path.IsNone)
            {
                this.path.Value = base._scene.path;
            }
            if (!this.isValid.IsNone)
            {
                this.isValid.Value = base._scene.IsValid();
            }
            if (!this.isDirty.IsNone)
            {
                this.isDirty.Value = base._scene.isDirty;
            }
            if (!this.isLoaded.IsNone)
            {
                this.isLoaded.Value = base._scene.isLoaded;
            }
            if (!this.rootCount.IsNone)
            {
                this.rootCount.Value = base._scene.rootCount;
            }
            if (!this.rootGameObjects.IsNone)
            {
                if (base._scene.IsValid())
                {
                    FsmArray fsmArray = this.rootGameObjects;
                    object[] values = base._scene.GetRootGameObjects();
                    fsmArray.Values = values;
                }
                else
                {
                    this.rootGameObjects.Resize(0);
                }
            }
            base.Fsm.Event(base.sceneFoundEvent);
        }
    }
}
