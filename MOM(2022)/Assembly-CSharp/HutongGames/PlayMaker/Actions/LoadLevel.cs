namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Level), HutongGames.PlayMaker.Tooltip("Loads a Level by Name. NOTE: Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
    public class LoadLevel : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The name of the level to load. NOTE: Must be in the list of levels defined in File->Build Settings... ")]
        public FsmString levelName;
        [HutongGames.PlayMaker.Tooltip("Load the level additively, keeping the current scene.")]
        public bool additive;
        [HutongGames.PlayMaker.Tooltip("Load the level asynchronously in the background.")]
        public bool async;
        [HutongGames.PlayMaker.Tooltip("Event to send when the level has loaded. NOTE: This only makes sense if the FSM is still in the scene!")]
        public FsmEvent loadedEvent;
        [HutongGames.PlayMaker.Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
        public FsmBool dontDestroyOnLoad;
        [HutongGames.PlayMaker.Tooltip("Event to send if the level cannot be loaded.")]
        public FsmEvent failedEvent;
        private AsyncOperation asyncOperation;

        public override void OnEnter()
        {
            if (!Application.CanStreamedLevelBeLoaded(this.levelName.Value))
            {
                base.Fsm.Event(this.failedEvent);
                base.Finish();
            }
            else
            {
                if (this.dontDestroyOnLoad.Value)
                {
                    UnityEngine.Object.DontDestroyOnLoad(base.get_Owner().transform.root.gameObject);
                }
                if (this.additive)
                {
                    if (this.async)
                    {
                        this.asyncOperation = SceneManager.LoadSceneAsync(this.levelName.Value, LoadSceneMode.Additive);
                        Debug.Log("LoadLevelAdditiveAsyc: " + this.levelName.Value);
                        return;
                    }
                    SceneManager.LoadScene(this.levelName.Value, LoadSceneMode.Additive);
                    Debug.Log("LoadLevelAdditive: " + this.levelName.Value);
                }
                else
                {
                    if (this.async)
                    {
                        this.asyncOperation = SceneManager.LoadSceneAsync(this.levelName.Value, LoadSceneMode.Single);
                        Debug.Log("LoadLevelAsync: " + this.levelName.Value);
                        return;
                    }
                    SceneManager.LoadScene(this.levelName.Value, LoadSceneMode.Single);
                    Debug.Log("LoadLevel: " + this.levelName.Value);
                }
                base.Log("LOAD COMPLETE");
                base.Fsm.Event(this.loadedEvent);
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            if (this.asyncOperation.isDone)
            {
                base.Fsm.Event(this.loadedEvent);
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.levelName = "";
            this.additive = false;
            this.async = false;
            this.loadedEvent = null;
            this.dontDestroyOnLoad = false;
        }
    }
}

