using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("Loads the scene by its name or index in Build Settings.")]
    public class LoadSceneAsynch : FsmStateAction
    {
        [Tooltip("The reference options of the Scene")]
        public GetSceneActionBase.SceneSimpleReferenceOptions sceneReference;

        [Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
        public FsmString sceneByName;

        [Tooltip("The index of the scene to load.")]
        public FsmInt sceneAtIndex;

        [Tooltip("Allows you to specify whether or not to load the scene additively. See LoadSceneMode Unity doc for more information about the options.")]
        [ObjectType(typeof(LoadSceneMode))]
        public FsmEnum loadSceneMode;

        [Tooltip("Allow the scene to be activated as soon as it's ready")]
        public FsmBool allowSceneActivation;

        [Tooltip("lets you tweak in which order async operation calls will be performed. Leave to none for default")]
        public FsmInt operationPriority;

        [ActionSection("Result")]
        [Tooltip("Use this hash to activate the Scene if you have set 'AllowSceneActivation' to false, you'll need to use it in the action 'AllowSceneActivation' to effectively load the scene.")]
        [UIHint(UIHint.Variable)]
        public FsmInt aSyncOperationHashCode;

        [Tooltip("The loading's progress.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat progress;

        [Tooltip("True when loading is done")]
        [UIHint(UIHint.Variable)]
        public FsmBool isDone;

        [Tooltip("True when loading is done but still waiting for scene activation")]
        [UIHint(UIHint.Variable)]
        public FsmBool pendingActivation;

        [Tooltip("Event sent when scene loading is done")]
        public FsmEvent doneEvent;

        [Tooltip("Event sent when scene loading is done but scene not yet activated. Use aSyncOperationHashCode value in 'AllowSceneActivation' to proceed")]
        public FsmEvent pendingActivationEvent;

        [Tooltip("Event sent if the scene to load was not found")]
        public FsmEvent sceneNotFoundEvent;

        private AsyncOperation _asyncOperation;

        private int _asynchOperationUid = -1;

        private bool pendingActivationCallBackDone;

        public static Dictionary<int, AsyncOperation> aSyncOperationLUT;

        private static int aSynchUidCounter;

        public override void Reset()
        {
            this.sceneReference = GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex;
            this.sceneByName = null;
            this.sceneAtIndex = null;
            this.loadSceneMode = null;
            this.aSyncOperationHashCode = null;
            this.allowSceneActivation = null;
            this.operationPriority = new FsmInt
            {
                UseVariable = true
            };
            this.pendingActivation = null;
            this.pendingActivationEvent = null;
            this.isDone = null;
            this.progress = null;
            this.doneEvent = null;
            this.sceneNotFoundEvent = null;
        }

        public override void OnEnter()
        {
            this.pendingActivationCallBackDone = false;
            this.pendingActivation.Value = false;
            this.isDone.Value = false;
            this.progress.Value = 0f;
            if (!this.DoLoadAsynch())
            {
                base.Fsm.Event(this.sceneNotFoundEvent);
                base.Finish();
            }
        }

        private bool DoLoadAsynch()
        {
            if (this.sceneReference == GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex)
            {
                if (SceneManager.GetActiveScene().buildIndex == this.sceneAtIndex.Value)
                {
                    return false;
                }
                this._asyncOperation = SceneManager.LoadSceneAsync(this.sceneAtIndex.Value, (LoadSceneMode)(object)this.loadSceneMode.Value);
            }
            else
            {
                if (SceneManager.GetActiveScene().name == this.sceneByName.Value)
                {
                    return false;
                }
                this._asyncOperation = SceneManager.LoadSceneAsync(this.sceneByName.Value, (LoadSceneMode)(object)this.loadSceneMode.Value);
            }
            if (!this.operationPriority.IsNone)
            {
                this._asyncOperation.priority = this.operationPriority.Value;
            }
            this._asyncOperation.allowSceneActivation = this.allowSceneActivation.Value;
            if (!this.aSyncOperationHashCode.IsNone)
            {
                if (LoadSceneAsynch.aSyncOperationLUT == null)
                {
                    LoadSceneAsynch.aSyncOperationLUT = new Dictionary<int, AsyncOperation>();
                }
                this._asynchOperationUid = ++LoadSceneAsynch.aSynchUidCounter;
                this.aSyncOperationHashCode.Value = this._asynchOperationUid;
                LoadSceneAsynch.aSyncOperationLUT.Add(this._asynchOperationUid, this._asyncOperation);
            }
            return true;
        }

        public override void OnUpdate()
        {
            if (this._asyncOperation == null)
            {
                return;
            }
            if (this._asyncOperation.isDone)
            {
                this.isDone.Value = true;
                this.progress.Value = this._asyncOperation.progress;
                if (LoadSceneAsynch.aSyncOperationLUT != null && this._asynchOperationUid != -1)
                {
                    LoadSceneAsynch.aSyncOperationLUT.Remove(this._asynchOperationUid);
                }
                this._asyncOperation = null;
                base.Fsm.Event(this.doneEvent);
                base.Finish();
                return;
            }
            this.progress.Value = this._asyncOperation.progress;
            if (!this._asyncOperation.allowSceneActivation && this.allowSceneActivation.Value)
            {
                this._asyncOperation.allowSceneActivation = true;
            }
            if (this._asyncOperation.progress == 0.9f && !this._asyncOperation.allowSceneActivation && !this.pendingActivationCallBackDone)
            {
                this.pendingActivationCallBackDone = true;
                if (!this.pendingActivation.IsNone)
                {
                    this.pendingActivation.Value = true;
                }
                base.Fsm.Event(this.pendingActivationEvent);
            }
        }

        public override void OnExit()
        {
            this._asyncOperation = null;
        }
    }
}
