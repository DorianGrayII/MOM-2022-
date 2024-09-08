using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Set the scene to be active.")]
    public class SetActiveScene : FsmStateAction
    {
        public enum SceneReferenceOptions
        {
            SceneAtBuildIndex = 0,
            SceneAtIndex = 1,
            SceneByName = 2,
            SceneByPath = 3,
            SceneByGameObject = 4
        }

        [Tooltip("The reference options of the Scene")]
        public SceneReferenceOptions sceneReference;

        [Tooltip("The name of the scene to activate. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
        public FsmString sceneByName;

        [Tooltip("The build index of the scene to activate.")]
        public FsmInt sceneAtBuildIndex;

        [Tooltip("The index of the scene to activate.")]
        public FsmInt sceneAtIndex;

        [Tooltip("The scene Path.")]
        public FsmString sceneByPath;

        [Tooltip("The GameObject scene to activate")]
        public FsmOwnerDefault sceneByGameObject;

        [ActionSection("Result")]
        [Tooltip("True if set active succeeded")]
        [UIHint(UIHint.Variable)]
        public FsmBool success;

        [Tooltip("Event sent if setActive succeeded ")]
        public FsmEvent successEvent;

        [Tooltip("True if SceneReference resolves to a scene")]
        [UIHint(UIHint.Variable)]
        public FsmBool sceneFound;

        [Tooltip("Event sent if scene not activated yet")]
        [UIHint(UIHint.Variable)]
        public FsmEvent sceneNotActivatedEvent;

        [Tooltip("Event sent if SceneReference do not resolve to a scene")]
        public FsmEvent sceneNotFoundEvent;

        private Scene _scene;

        private bool _sceneFound;

        private bool _success;

        public override void Reset()
        {
            this.sceneReference = SceneReferenceOptions.SceneAtIndex;
            this.sceneByName = null;
            this.sceneAtBuildIndex = null;
            this.sceneAtIndex = null;
            this.sceneByPath = null;
            this.sceneByGameObject = null;
            this.success = null;
            this.successEvent = null;
            this.sceneFound = null;
            this.sceneNotActivatedEvent = null;
            this.sceneNotFoundEvent = null;
        }

        public override void OnEnter()
        {
            this.DoSetActivate();
            if (!this.success.IsNone)
            {
                this.success.Value = this._success;
            }
            if (!this.sceneFound.IsNone)
            {
                this.sceneFound.Value = this._sceneFound;
            }
            if (this._success)
            {
                base.Fsm.Event(this.successEvent);
            }
        }

        private void DoSetActivate()
        {
            try
            {
                switch (this.sceneReference)
                {
                case SceneReferenceOptions.SceneAtIndex:
                    this._scene = SceneManager.GetSceneAt(this.sceneAtIndex.Value);
                    break;
                case SceneReferenceOptions.SceneByName:
                    this._scene = SceneManager.GetSceneByName(this.sceneByName.Value);
                    break;
                case SceneReferenceOptions.SceneByPath:
                    this._scene = SceneManager.GetSceneByPath(this.sceneByPath.Value);
                    break;
                case SceneReferenceOptions.SceneByGameObject:
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
                this._sceneFound = false;
                base.Fsm.Event(this.sceneNotFoundEvent);
                return;
            }
            if (this._scene == default(Scene))
            {
                this._sceneFound = false;
                base.Fsm.Event(this.sceneNotFoundEvent);
            }
            else
            {
                this._success = SceneManager.SetActiveScene(this._scene);
                this._sceneFound = true;
            }
        }
    }
}
