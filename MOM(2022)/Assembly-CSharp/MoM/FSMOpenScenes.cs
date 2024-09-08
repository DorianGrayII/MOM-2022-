namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMOpenScenes : FSMStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            MHEventSystem.ForceClear();
            Application.targetFrameRate = 60;
            SceneManager.LoadScene("UIRoot", LoadSceneMode.Additive);
        }

        public override void OnExit()
        {
            CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (UIManager.Get() != null)
            {
                base.Finish();
            }
        }
    }
}

