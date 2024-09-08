namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), HutongGames.PlayMaker.Tooltip("Unload a scene asynchronously by its name or index in Build Settings. Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.")]
    public class UnloadSceneAsynch : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The reference options of the Scene")]
        public SceneReferenceOptions sceneReference;
        [HutongGames.PlayMaker.Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
        public FsmString sceneByName;
        [HutongGames.PlayMaker.Tooltip("The build index of the scene to unload.")]
        public FsmInt sceneAtBuildIndex;
        [HutongGames.PlayMaker.Tooltip("The index of the scene to unload.")]
        public FsmInt sceneAtIndex;
        [HutongGames.PlayMaker.Tooltip("The scene Path.")]
        public FsmString sceneByPath;
        [HutongGames.PlayMaker.Tooltip("The GameObject unload scene of")]
        public FsmOwnerDefault sceneByGameObject;
        [HutongGames.PlayMaker.Tooltip("lets you tweak in which order async operation calls will be performed. Leave to none for default")]
        public FsmInt operationPriority;
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The loading's progress."), UIHint(UIHint.Variable)]
        public FsmFloat progress;
        [HutongGames.PlayMaker.Tooltip("True when loading is done"), UIHint(UIHint.Variable)]
        public FsmBool isDone;
        [HutongGames.PlayMaker.Tooltip("Event sent when scene loading is done")]
        public FsmEvent doneEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if the scene to load was not found")]
        public FsmEvent sceneNotFoundEvent;
        private AsyncOperation _asyncOperation;

        private bool DoUnLoadAsynch()
        {
            try
            {
                switch (this.sceneReference)
                {
                    case SceneReferenceOptions.ActiveScene:
                        this._asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                        break;

                    case SceneReferenceOptions.SceneAtBuildIndex:
                        this._asyncOperation = SceneManager.UnloadSceneAsync(this.sceneAtBuildIndex.Value);
                        break;

                    case SceneReferenceOptions.SceneAtIndex:
                        this._asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(this.sceneAtIndex.Value));
                        break;

                    case SceneReferenceOptions.SceneByName:
                        this._asyncOperation = SceneManager.UnloadSceneAsync(this.sceneByName.Value);
                        break;

                    case SceneReferenceOptions.SceneByPath:
                        this._asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByPath(this.sceneByPath.Value));
                        break;

                    case SceneReferenceOptions.SceneByGameObject:
                    {
                        GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.sceneByGameObject);
                        if (ownerDefaultTarget == null)
                        {
                            throw new Exception("Null GameObject");
                        }
                        this._asyncOperation = SceneManager.UnloadSceneAsync(ownerDefaultTarget.scene);
                        break;
                    }
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                base.LogError(exception.Message);
                return false;
            }
            if (!this.operationPriority.IsNone)
            {
                this._asyncOperation.priority = this.operationPriority.Value;
            }
            return true;
        }

        public override void OnEnter()
        {
            this.isDone.Value = false;
            this.progress.Value = 0f;
            if (!this.DoUnLoadAsynch())
            {
                base.Fsm.Event(this.sceneNotFoundEvent);
                base.Finish();
            }
        }

        public override void OnExit()
        {
            this._asyncOperation = null;
        }

        public override void OnUpdate()
        {
            if (this._asyncOperation != null)
            {
                if (!this._asyncOperation.isDone)
                {
                    this.progress.Value = this._asyncOperation.progress;
                }
                else
                {
                    this.isDone.Value = true;
                    this.progress.Value = this._asyncOperation.progress;
                    this._asyncOperation = null;
                    base.Fsm.Event(this.doneEvent);
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.sceneReference = SceneReferenceOptions.SceneAtBuildIndex;
            this.sceneByName = null;
            this.sceneAtBuildIndex = null;
            this.sceneAtIndex = null;
            this.sceneByPath = null;
            this.sceneByGameObject = null;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.operationPriority = num1;
            this.isDone = null;
            this.progress = null;
            this.doneEvent = null;
            this.sceneNotFoundEvent = null;
        }

        public enum SceneReferenceOptions
        {
            ActiveScene,
            SceneAtBuildIndex,
            SceneAtIndex,
            SceneByName,
            SceneByPath,
            SceneByGameObject
        }
    }
}

