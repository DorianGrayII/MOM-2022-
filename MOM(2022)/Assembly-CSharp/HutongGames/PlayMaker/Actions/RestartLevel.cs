namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Level), Tooltip("Restarts current level.")]
    public class RestartLevel : FsmStateAction
    {
        public override void OnEnter()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            base.Finish();
        }
    }
}

