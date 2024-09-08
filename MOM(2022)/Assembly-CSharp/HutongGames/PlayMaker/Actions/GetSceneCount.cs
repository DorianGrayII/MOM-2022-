namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get the total number of currently loaded scenes.")]
    public class GetSceneCount : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The number of currently loaded scenes.")]
        public FsmInt sceneCount;
        [Tooltip("Repeat every Frame")]
        public bool everyFrame;

        private void DoGetSceneCount()
        {
            this.sceneCount.Value = SceneManager.sceneCount;
        }

        public override void OnEnter()
        {
            this.DoGetSceneCount();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetSceneCount();
        }

        public override void Reset()
        {
            this.sceneCount = null;
            this.everyFrame = false;
        }
    }
}

