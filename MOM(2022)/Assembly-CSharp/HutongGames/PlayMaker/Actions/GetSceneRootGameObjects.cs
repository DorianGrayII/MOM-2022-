namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get a scene Root GameObjects.")]
    public class GetSceneRootGameObjects : GetSceneActionBase
    {
        [ActionSection("Result")]
        [Tooltip("The scene Root GameObjects")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
        public FsmArray rootGameObjects;

        [Tooltip("Repeat every Frame")]
        public bool everyFrame;

        public override void Reset()
        {
            base.Reset();
            this.rootGameObjects = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneRootGameObjects();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSceneRootGameObjects();
        }

        private void DoGetSceneRootGameObjects()
        {
            if (base._sceneFound)
            {
                if (!this.rootGameObjects.IsNone)
                {
                    FsmArray fsmArray = this.rootGameObjects;
                    object[] values = base._scene.GetRootGameObjects();
                    fsmArray.Values = values;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }
    }
}
