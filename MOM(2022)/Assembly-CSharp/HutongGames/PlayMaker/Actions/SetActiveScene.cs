namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), HutongGames.PlayMaker.Tooltip("Set the scene to be active.")]
    public class SetActiveScene : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The reference options of the Scene")]
        public SceneReferenceOptions sceneReference;
        [HutongGames.PlayMaker.Tooltip("The name of the scene to activate. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
        public FsmString sceneByName;
        [HutongGames.PlayMaker.Tooltip("The build index of the scene to activate.")]
        public FsmInt sceneAtBuildIndex;
        [HutongGames.PlayMaker.Tooltip("The index of the scene to activate.")]
        public FsmInt sceneAtIndex;
        [HutongGames.PlayMaker.Tooltip("The scene Path.")]
        public FsmString sceneByPath;
        [HutongGames.PlayMaker.Tooltip("The GameObject scene to activate")]
        public FsmOwnerDefault sceneByGameObject;
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("True if set active succeeded"), UIHint(UIHint.Variable)]
        public FsmBool success;
        [HutongGames.PlayMaker.Tooltip("Event sent if setActive succeeded ")]
        public FsmEvent successEvent;
        [HutongGames.PlayMaker.Tooltip("True if SceneReference resolves to a scene"), UIHint(UIHint.Variable)]
        public FsmBool sceneFound;
        [HutongGames.PlayMaker.Tooltip("Event sent if scene not activated yet"), UIHint(UIHint.Variable)]
        public FsmEvent sceneNotActivatedEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if SceneReference do not resolve to a scene")]
        public FsmEvent sceneNotFoundEvent;
        private Scene _scene;
        private bool _sceneFound;
        private bool _success;

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
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                base.LogError(exception.Message);
                this._sceneFound = false;
                base.Fsm.Event(this.sceneNotFoundEvent);
                return;
            }
            Scene scene = new Scene();
            if (this._scene == scene)
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

        public enum SceneReferenceOptions
        {
            SceneAtBuildIndex,
            SceneAtIndex,
            SceneByName,
            SceneByPath,
            SceneByGameObject
        }
    }
}

