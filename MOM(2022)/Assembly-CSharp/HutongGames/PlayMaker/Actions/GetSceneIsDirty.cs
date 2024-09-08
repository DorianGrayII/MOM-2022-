namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get a scene isDirty flag. true if the scene is modified. ")]
    public class GetSceneIsDirty : GetSceneActionBase
    {
        [ActionSection("Result")]
        [UIHint(UIHint.Variable)]
        [Tooltip("true if the scene is modified.")]
        public FsmBool isDirty;

        [Tooltip("Event sent if the scene is modified.")]
        public FsmEvent isDirtyEvent;

        [Tooltip("Event sent if the scene is unmodified.")]
        public FsmEvent isNotDirtyEvent;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            base.Reset();
            this.isDirty = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneIsDirty();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSceneIsDirty();
        }

        private void DoGetSceneIsDirty()
        {
            if (base._sceneFound)
            {
                if (!this.isDirty.IsNone)
                {
                    this.isDirty.Value = base._scene.isDirty;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }
    }
}
