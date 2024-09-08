using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Scene)]
    [Tooltip("This will merge the source scene into the destinationScene. This function merges the contents of the source scene into the destination scene, and deletes the source scene. All GameObjects at the root of the source scene are moved to the root of the destination scene. NOTE: This function is destructive: The source scene will be destroyed once the merge has been completed.")]
    public class MergeScenes : FsmStateAction
    {
        [ActionSection("Source")]
        [Tooltip("The reference options of the source Scene")]
        public GetSceneActionBase.SceneAllReferenceOptions sourceReference;

        [Tooltip("The source scene Index.")]
        public FsmInt sourceAtIndex;

        [Tooltip("The source scene Name.")]
        public FsmString sourceByName;

        [Tooltip("The source scene Path.")]
        public FsmString sourceByPath;

        [Tooltip("The source scene from GameObject")]
        public FsmOwnerDefault sourceByGameObject;

        [ActionSection("Destination")]
        [Tooltip("The reference options of the destination Scene")]
        public GetSceneActionBase.SceneAllReferenceOptions destinationReference;

        [Tooltip("The destination scene Index.")]
        public FsmInt destinationAtIndex;

        [Tooltip("The destination scene Name.")]
        public FsmString destinationByName;

        [Tooltip("The destination scene Path.")]
        public FsmString destinationByPath;

        [Tooltip("The destination scene from GameObject")]
        public FsmOwnerDefault destinationByGameObject;

        [ActionSection("Result")]
        [Tooltip("True if the merge succeeded")]
        [UIHint(UIHint.Variable)]
        public FsmBool success;

        [Tooltip("Event sent if merge succeeded")]
        public FsmEvent successEvent;

        [Tooltip("Event sent if merge failed")]
        public FsmEvent failureEvent;

        private Scene _sourceScene;

        private bool _sourceFound;

        private Scene _destinationScene;

        private bool _destinationFound;

        public override void Reset()
        {
            this.sourceReference = GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex;
            this.sourceByPath = null;
            this.sourceByName = null;
            this.sourceAtIndex = null;
            this.sourceByGameObject = null;
            this.destinationReference = GetSceneActionBase.SceneAllReferenceOptions.ActiveScene;
            this.destinationByPath = null;
            this.destinationByName = null;
            this.destinationAtIndex = null;
            this.destinationByGameObject = null;
            this.success = null;
            this.successEvent = null;
            this.failureEvent = null;
        }

        public override void OnEnter()
        {
            this.GetSourceScene();
            this.GetDestinationScene();
            if (this._destinationFound && this._sourceFound)
            {
                if (this._destinationScene.Equals(this._sourceScene))
                {
                    base.LogError("Source and Destination scenes can not be the same");
                }
                else
                {
                    SceneManager.MergeScenes(this._sourceScene, this._destinationScene);
                }
                this.success.Value = true;
                base.Fsm.Event(this.successEvent);
            }
            else
            {
                this.success.Value = false;
                base.Fsm.Event(this.failureEvent);
            }
            base.Finish();
        }

        private void GetSourceScene()
        {
            try
            {
                switch (this.sourceReference)
                {
                case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
                    this._sourceScene = SceneManager.GetActiveScene();
                    break;
                case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
                    this._sourceScene = SceneManager.GetSceneAt(this.sourceAtIndex.Value);
                    break;
                case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
                    this._sourceScene = SceneManager.GetSceneByName(this.sourceByName.Value);
                    break;
                case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
                    this._sourceScene = SceneManager.GetSceneByPath(this.sourceByPath.Value);
                    break;
                }
            }
            catch (Exception ex)
            {
                base.LogError(ex.Message);
            }
            if (this._sourceScene == default(Scene))
            {
                this._sourceFound = false;
            }
            else
            {
                this._sourceFound = true;
            }
        }

        private void GetDestinationScene()
        {
            try
            {
                switch (this.sourceReference)
                {
                case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
                    this._destinationScene = SceneManager.GetActiveScene();
                    break;
                case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
                    this._destinationScene = SceneManager.GetSceneAt(this.destinationAtIndex.Value);
                    break;
                case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
                    this._destinationScene = SceneManager.GetSceneByName(this.destinationByName.Value);
                    break;
                case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
                    this._destinationScene = SceneManager.GetSceneByPath(this.destinationByPath.Value);
                    break;
                }
            }
            catch (Exception ex)
            {
                base.LogError(ex.Message);
            }
            if (this._destinationScene == default(Scene))
            {
                this._destinationFound = false;
            }
            else
            {
                this._destinationFound = true;
            }
        }

        public override string ErrorCheck()
        {
            if (this.sourceReference == GetSceneActionBase.SceneAllReferenceOptions.ActiveScene && this.destinationReference == GetSceneActionBase.SceneAllReferenceOptions.ActiveScene)
            {
                return "Source and Destination scenes can not be the same";
            }
            return string.Empty;
        }
    }
}
