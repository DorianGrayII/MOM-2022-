namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public abstract class GetSceneActionBase : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The reference option of the Scene")]
        public SceneAllReferenceOptions sceneReference;
        [HutongGames.PlayMaker.Tooltip("The scene Index.")]
        public FsmInt sceneAtIndex;
        [HutongGames.PlayMaker.Tooltip("The scene Name.")]
        public FsmString sceneByName;
        [HutongGames.PlayMaker.Tooltip("The scene Path.")]
        public FsmString sceneByPath;
        [HutongGames.PlayMaker.Tooltip("The Scene of GameObject")]
        public FsmOwnerDefault sceneByGameObject;
        [HutongGames.PlayMaker.Tooltip("True if SceneReference resolves to a scene"), UIHint(UIHint.Variable)]
        public FsmBool sceneFound;
        [HutongGames.PlayMaker.Tooltip("Event sent if SceneReference resolves to a scene")]
        public FsmEvent sceneFoundEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if SceneReference do not resolve to a scene")]
        public FsmEvent sceneNotFoundEvent;
        [HutongGames.PlayMaker.Tooltip("The Scene Cache")]
        protected Scene _scene;
        [HutongGames.PlayMaker.Tooltip("True if a scene was found, use _scene to access it")]
        protected bool _sceneFound;

        protected GetSceneActionBase()
        {
        }

        public override void OnEnter()
        {
            try
            {
                switch (this.sceneReference)
                {
                    case SceneAllReferenceOptions.ActiveScene:
                        this._scene = SceneManager.GetActiveScene();
                        break;

                    case SceneAllReferenceOptions.SceneAtIndex:
                        this._scene = SceneManager.GetSceneAt(this.sceneAtIndex.Value);
                        break;

                    case SceneAllReferenceOptions.SceneByName:
                        this._scene = SceneManager.GetSceneByName(this.sceneByName.Value);
                        break;

                    case SceneAllReferenceOptions.SceneByPath:
                        this._scene = SceneManager.GetSceneByPath(this.sceneByPath.Value);
                        break;

                    case SceneAllReferenceOptions.SceneByGameObject:
                    {
                        GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.sceneByGameObject);
                        if (ownerDefaultTarget == null)
                        {
                            throw new Exception("Null GameObject");
                        }
                        this._scene = ownerDefaultTarget.scene;
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
            Scene scene = new Scene();
            if (!(this._scene == scene))
            {
                this._sceneFound = true;
                if (!this.sceneFound.IsNone)
                {
                    this.sceneFound.Value = true;
                }
            }
            else
            {
                this._sceneFound = false;
                if (!this.sceneFound.IsNone)
                {
                    this.sceneFound.Value = false;
                }
                base.Fsm.Event(this.sceneNotFoundEvent);
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.sceneReference = SceneAllReferenceOptions.ActiveScene;
            this.sceneAtIndex = null;
            this.sceneByName = null;
            this.sceneByPath = null;
            this.sceneByGameObject = null;
            this.sceneFound = null;
            this.sceneFoundEvent = null;
            this.sceneNotFoundEvent = null;
        }

        public enum SceneAllReferenceOptions
        {
            ActiveScene,
            SceneAtIndex,
            SceneByName,
            SceneByPath,
            SceneByGameObject
        }

        public enum SceneBuildReferenceOptions
        {
            SceneAtBuildIndex,
            SceneByName
        }

        public enum SceneReferenceOptions
        {
            SceneAtIndex,
            SceneByName,
            SceneByPath
        }

        public enum SceneSimpleReferenceOptions
        {
            SceneAtIndex,
            SceneByName
        }
    }
}

