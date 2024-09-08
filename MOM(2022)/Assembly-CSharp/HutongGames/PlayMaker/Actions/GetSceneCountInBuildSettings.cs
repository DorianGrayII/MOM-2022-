using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Get the number of scenes in Build Settings.")]
    public class GetSceneCountInBuildSettings : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The number of scenes in Build Settings.")]
        public FsmInt sceneCountInBuildSettings;

        public override void Reset()
        {
            this.sceneCountInBuildSettings = null;
        }

        public override void OnEnter()
        {
            this.DoGetSceneCountInBuildSettings();
            base.Finish();
        }

        private void DoGetSceneCountInBuildSettings()
        {
            this.sceneCountInBuildSettings.Value = SceneManager.sceneCountInBuildSettings;
        }
    }
}
