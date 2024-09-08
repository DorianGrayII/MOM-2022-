namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get a scene name.")]
    public class GetSceneName : GetSceneActionBase
    {
        [ActionSection("Result"), RequiredField, UIHint(UIHint.Variable), Tooltip("The scene name")]
        public FsmString name;

        private void DoGetSceneName()
        {
            if (base._sceneFound)
            {
                if (!this.name.IsNone)
                {
                    this.name.Value = base._scene.name;
                }
                base.Fsm.Event(base.sceneFoundEvent);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.DoGetSceneName();
            base.Finish();
        }

        public override void Reset()
        {
            base.Reset();
            this.name = null;
        }
    }
}

