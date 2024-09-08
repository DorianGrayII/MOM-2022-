namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get a scene path.")]
    public class GetScenePath : GetSceneActionBase
    {
        [ActionSection("Result")]
        [Tooltip("The scene path")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString path;

        public override void Reset()
        {
            base.Reset();
            this.path = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetScenePath();
            base.Finish();
        }

        private void DoGetScenePath()
        {
            if (base._sceneFound)
            {
                if (!this.path.IsNone)
                {
                    this.path.Value = base._scene.path;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }
    }
}
