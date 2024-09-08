namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get a scene isLoaded flag.")]
    public class GetSceneIsLoaded : GetSceneActionBase
    {
        [ActionSection("Result")]
        [Tooltip("true if the scene is loaded.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isLoaded;

        [Tooltip("Event sent if the scene is loaded.")]
        public FsmEvent isLoadedEvent;

        [Tooltip("Event sent if the scene is not loaded.")]
        public FsmEvent isNotLoadedEvent;

        [Tooltip("Repeat every Frame")]
        public bool everyFrame;

        public override void Reset()
        {
            base.Reset();
            this.isLoaded = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneIsLoaded();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSceneIsLoaded();
        }

        private void DoGetSceneIsLoaded()
        {
            if (base._sceneFound)
            {
                if (!this.isLoaded.IsNone)
                {
                    this.isLoaded.Value = base._scene.isLoaded;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }
    }
}
