namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Scene), Tooltip("Returns the index of a scene in the Build Settings. Always returns -1 if the scene was loaded through an AssetBundle.")]
    public class GetSceneBuildIndex : GetSceneActionBase
    {
        [ActionSection("Result"), Tooltip("The scene Build Index"), RequiredField, UIHint(UIHint.Variable)]
        public FsmInt buildIndex;

        private void DoGetSceneBuildIndex()
        {
            if (base._sceneFound)
            {
                if (!this.buildIndex.IsNone)
                {
                    this.buildIndex.Value = base._scene.buildIndex;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneBuildIndex();
            base.Finish();
        }

        public override void Reset()
        {
            base.Reset();
            this.buildIndex = null;
        }
    }
}

