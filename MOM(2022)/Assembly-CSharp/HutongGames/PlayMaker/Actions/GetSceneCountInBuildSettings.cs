namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get the number of scenes in Build Settings.")]
    public class GetSceneCountInBuildSettings : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The number of scenes in Build Settings.")]
        public FsmInt sceneCountInBuildSettings;

        private void DoGetSceneCountInBuildSettings()
        {
            this.sceneCountInBuildSettings.Value = SceneManager.sceneCountInBuildSettings;
        }

        public override void OnEnter()
        {
            this.DoGetSceneCountInBuildSettings();
            base.Finish();
        }

        public override void Reset()
        {
            this.sceneCountInBuildSettings = null;
        }
    }
}

