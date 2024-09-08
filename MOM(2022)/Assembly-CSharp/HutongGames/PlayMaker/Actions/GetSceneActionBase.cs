using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    public abstract class GetSceneActionBase : FsmStateAction
    {
        public enum SceneReferenceOptions
        {
            SceneAtIndex = 0,
            SceneByName = 1,
            SceneByPath = 2
        }

        public enum SceneSimpleReferenceOptions
        {
            SceneAtIndex = 0,
            SceneByName = 1
        }

        public enum SceneBuildReferenceOptions
        {
            SceneAtBuildIndex = 0,
            SceneByName = 1
        }

        public enum SceneAllReferenceOptions
        {
            ActiveScene = 0,
            SceneAtIndex = 1,
            SceneByName = 2,
            SceneByPath = 3,
            SceneByGameObject = 4
        }

        [Tooltip("The reference option of the Scene")]
        public SceneAllReferenceOptions sceneReference;

        [Tooltip("The scene Index.")]
        public FsmInt sceneAtIndex;

        [Tooltip("The scene Name.")]
        public FsmString sceneByName;

        [Tooltip("The scene Path.")]
        public FsmString sceneByPath;

        [Tooltip("The Scene of GameObject")]
        public FsmOwnerDefault sceneByGameObject;

        [Tooltip("True if SceneReference resolves to a scene")]
        [UIHint(UIHint.Variable)]
        public FsmBool sceneFound;

        [Tooltip("Event sent if SceneReference resolves to a scene")]
        public FsmEvent sceneFoundEvent;

        [Tooltip("Event sent if SceneReference do not resolve to a scene")]
        public FsmEvent sceneNotFoundEvent;

        [Tooltip("The Scene Cache")]
        protected Scene _scene;

        [Tooltip("True if a scene was found, use _scene to access it")]
        protected bool _sceneFound;

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
                }
            }
            catch (Exception ex)
            {
                base.LogError(ex.Message);
            }
            if (this._scene == default(Scene))
            {
                this._sceneFound = false;
                if (!this.sceneFound.IsNone)
                {
                    this.sceneFound.Value = false;
                }
                base.Fsm.Event(this.sceneNotFoundEvent);
            }
            else
            {
                this._sceneFound = true;
                if (!this.sceneFound.IsNone)
                {
                    this.sceneFound.Value = true;
                }
            }
        }
    }
}
