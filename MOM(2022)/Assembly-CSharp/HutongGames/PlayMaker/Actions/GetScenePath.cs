namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get a scene path.")]
    public class GetScenePath : GetSceneActionBase
    {
        [ActionSection("Result"), Tooltip("The scene path"), RequiredField, UIHint(UIHint.Variable)]
        public FsmString path;

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

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetScenePath();
            base.Finish();
        }

        public override void Reset()
        {
            base.Reset();
            this.path = null;
        }
    }
}

