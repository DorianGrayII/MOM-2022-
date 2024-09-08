namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), Tooltip("Create an empty new scene with the given name additively. The path of the new scene will be empty")]
    public class CreateScene : FsmStateAction
    {
        [RequiredField, Tooltip("The name of the new scene. It cannot be empty or null, or same as the name of the existing scenes.")]
        public FsmString sceneName;

        public override void OnEnter()
        {
            SceneManager.CreateScene(this.sceneName.Value);
            base.Finish();
        }

        public override void Reset()
        {
            this.sceneName = null;
        }
    }
}

