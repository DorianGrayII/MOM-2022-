namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Obsolete("Use UnloadSceneAsynch Instead"), ActionCategory(ActionCategory.Scene), HutongGames.PlayMaker.Tooltip("Unload Scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadUnusedAssets.")]
    public class UnloadScene : FsmStateAction
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
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("True if scene was unloaded"), UIHint(UIHint.Variable)]
        public FsmBool unloaded;
        [HutongGames.PlayMaker.Tooltip("Event sent if scene was unloaded ")]
        public FsmEvent unloadedEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent scene was not unloaded"), UIHint(UIHint.Variable)]
        public FsmEvent failureEvent;

        public override string ErrorCheck()
        {
            switch (this.sceneReference)
            {
            }
            return string.Empty;
        }

        public override void OnEnter()
        {
            bool flag = false;
            try
            {
                switch (this.sceneReference)
                {
                    case SceneReferenceOptions.ActiveScene:
                        flag = SceneManager.UnloadScene(SceneManager.GetActiveScene());
                        break;

                    case SceneReferenceOptions.SceneAtBuildIndex:
                        flag = SceneManager.UnloadScene(this.sceneAtBuildIndex.Value);
                        break;

                    case SceneReferenceOptions.SceneAtIndex:
                        flag = SceneManager.UnloadScene(SceneManager.GetSceneAt(this.sceneAtIndex.Value));
                        break;

                    case SceneReferenceOptions.SceneByName:
                        flag = SceneManager.UnloadScene(this.sceneByName.Value);
                        break;

                    case SceneReferenceOptions.SceneByPath:
                        flag = SceneManager.UnloadScene(SceneManager.GetSceneByPath(this.sceneByPath.Value));
                        break;

                    case SceneReferenceOptions.SceneByGameObject:
                    {
                        GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.sceneByGameObject);
                        if (ownerDefaultTarget == null)
                        {
                            throw new Exception("Null GameObject");
                        }
                        flag = SceneManager.UnloadScene(ownerDefaultTarget.scene);
                        break;
                    }
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                base.LogError(exception.Message);
            }
            if (!this.unloaded.IsNone)
            {
                this.unloaded.Value = flag;
            }
            if (flag)
            {
                base.Fsm.Event(this.unloadedEvent);
            }
            else
            {
                base.Fsm.Event(this.failureEvent);
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.sceneReference = SceneReferenceOptions.SceneAtBuildIndex;
            this.sceneByName = null;
            this.sceneAtBuildIndex = null;
            this.sceneAtIndex = null;
            this.sceneByPath = null;
            this.sceneByGameObject = null;
            this.unloaded = null;
            this.unloadedEvent = null;
            this.failureEvent = null;
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

