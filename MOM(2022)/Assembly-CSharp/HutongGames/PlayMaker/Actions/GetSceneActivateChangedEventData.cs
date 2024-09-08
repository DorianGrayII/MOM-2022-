namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), Tooltip("Get the last activateChanged Scene Event data when event was sent from the action 'SendSceneActiveChangedEvent")]
    public class GetSceneActivateChangedEventData : FsmStateAction
    {
        [ActionSection("New Active Scene"), UIHint(UIHint.Variable), Tooltip("The new active scene name")]
        public FsmString newName;
        [Tooltip("The new active scene path"), UIHint(UIHint.Variable)]
        public FsmString newPath;
        [Tooltip("true if the new active scene is valid."), UIHint(UIHint.Variable)]
        public FsmBool newIsValid;
        [Tooltip("The new active scene Build Index"), UIHint(UIHint.Variable)]
        public FsmInt newBuildIndex;
        [Tooltip("true if the new active scene is loaded."), UIHint(UIHint.Variable)]
        public FsmBool newIsLoaded;
        [UIHint(UIHint.Variable), Tooltip("true if the new active scene is modified.")]
        public FsmBool newIsDirty;
        [Tooltip("The new active scene RootCount"), UIHint(UIHint.Variable)]
        public FsmInt newRootCount;
        [Tooltip("The new active scene Root GameObjects"), UIHint(UIHint.Variable), ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000)]
        public FsmArray newRootGameObjects;
        [ActionSection("Previous Active Scene"), UIHint(UIHint.Variable), Tooltip("The previous active scene name")]
        public FsmString previousName;
        [Tooltip("The previous active scene path"), UIHint(UIHint.Variable)]
        public FsmString previousPath;
        [Tooltip("true if the previous active scene is valid."), UIHint(UIHint.Variable)]
        public FsmBool previousIsValid;
        [Tooltip("The previous active scene Build Index"), UIHint(UIHint.Variable)]
        public FsmInt previousBuildIndex;
        [Tooltip("true if the previous active scene is loaded."), UIHint(UIHint.Variable)]
        public FsmBool previousIsLoaded;
        [UIHint(UIHint.Variable), Tooltip("true if the previous active scene is modified.")]
        public FsmBool previousIsDirty;
        [Tooltip("The previous active scene RootCount"), UIHint(UIHint.Variable)]
        public FsmInt previousRootCount;
        [Tooltip("The previous active scene Root GameObjects"), UIHint(UIHint.Variable), ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000)]
        public FsmArray previousRootGameObjects;
        private Scene _scene;

        private void DoGetSceneProperties()
        {
            this._scene = SendActiveSceneChangedEvent.lastPreviousActiveScene;
            if (!this.previousName.IsNone)
            {
                this.previousName.Value = this._scene.name;
            }
            if (!this.previousBuildIndex.IsNone)
            {
                this.previousBuildIndex.Value = this._scene.buildIndex;
            }
            if (!this.previousPath.IsNone)
            {
                this.previousPath.Value = this._scene.path;
            }
            if (!this.previousIsValid.IsNone)
            {
                this.previousIsValid.Value = this._scene.IsValid();
            }
            if (!this.previousIsDirty.IsNone)
            {
                this.previousIsDirty.Value = this._scene.isDirty;
            }
            if (!this.previousIsLoaded.IsNone)
            {
                this.previousIsLoaded.Value = this._scene.isLoaded;
            }
            if (!this.previousRootCount.IsNone)
            {
                this.previousRootCount.Value = this._scene.rootCount;
            }
            if (!this.previousRootGameObjects.IsNone)
            {
                if (this._scene.IsValid())
                {
                    this.previousRootGameObjects.Values = this._scene.GetRootGameObjects();
                }
                else
                {
                    this.previousRootGameObjects.Resize(0);
                }
            }
            this._scene = SendActiveSceneChangedEvent.lastNewActiveScene;
            if (!this.newName.IsNone)
            {
                this.newName.Value = this._scene.name;
            }
            if (!this.newBuildIndex.IsNone)
            {
                this.newBuildIndex.Value = this._scene.buildIndex;
            }
            if (!this.newPath.IsNone)
            {
                this.newPath.Value = this._scene.path;
            }
            if (!this.newIsValid.IsNone)
            {
                this.newIsValid.Value = this._scene.IsValid();
            }
            if (!this.newIsDirty.IsNone)
            {
                this.newIsDirty.Value = this._scene.isDirty;
            }
            if (!this.newIsLoaded.IsNone)
            {
                this.newIsLoaded.Value = this._scene.isLoaded;
            }
            if (!this.newRootCount.IsNone)
            {
                this.newRootCount.Value = this._scene.rootCount;
            }
            if (!this.newRootGameObjects.IsNone)
            {
                if (this._scene.IsValid())
                {
                    this.newRootGameObjects.Values = this._scene.GetRootGameObjects();
                }
                else
                {
                    this.newRootGameObjects.Resize(0);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetSceneProperties();
            base.Finish();
        }

        public override void OnUpdate()
        {
            this.DoGetSceneProperties();
        }

        public override void Reset()
        {
            this.newName = null;
            this.newPath = null;
            this.newIsValid = null;
            this.newBuildIndex = null;
            this.newIsLoaded = null;
            this.newRootCount = null;
            this.newRootGameObjects = null;
            this.newIsDirty = null;
            this.previousName = null;
            this.previousPath = null;
            this.previousIsValid = null;
            this.previousBuildIndex = null;
            this.previousIsLoaded = null;
            this.previousRootCount = null;
            this.previousRootGameObjects = null;
            this.previousIsDirty = null;
        }
    }
}

