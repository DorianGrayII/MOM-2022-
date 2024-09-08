namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get a scene RootCount, the number of root transforms of this scene.")]
    public class GetSceneRootCount : GetSceneActionBase
    {
        [ActionSection("Result"), Tooltip("The scene RootCount"), RequiredField, UIHint(UIHint.Variable)]
        public FsmInt rootCount;
        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoGetSceneRootCount()
        {
            if (base._sceneFound)
            {
                if (!this.rootCount.IsNone)
                {
                    this.rootCount.Value = base._scene.rootCount;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneRootCount();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSceneRootCount();
        }

        public override void Reset()
        {
            base.Reset();
            this.rootCount = null;
            this.everyFrame = false;
        }
    }
}

