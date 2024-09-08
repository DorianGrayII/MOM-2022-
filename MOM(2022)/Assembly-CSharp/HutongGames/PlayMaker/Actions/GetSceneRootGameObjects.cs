namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get a scene Root GameObjects.")]
    public class GetSceneRootGameObjects : GetSceneActionBase
    {
        [ActionSection("Result"), Tooltip("The scene Root GameObjects"), RequiredField, UIHint(UIHint.Variable), ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000)]
        public FsmArray rootGameObjects;
        [Tooltip("Repeat every Frame")]
        public bool everyFrame;

        private void DoGetSceneRootGameObjects()
        {
            if (base._sceneFound)
            {
                if (!this.rootGameObjects.IsNone)
                {
                    this.rootGameObjects.Values = base._scene.GetRootGameObjects();
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
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

        public override void Reset()
        {
            base.Reset();
            this.rootGameObjects = null;
            this.everyFrame = false;
        }
    }
}

