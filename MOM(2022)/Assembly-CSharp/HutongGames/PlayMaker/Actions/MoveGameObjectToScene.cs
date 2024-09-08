namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [ActionCategory(ActionCategory.Scene), HutongGames.PlayMaker.Tooltip("Move a GameObject from its current scene to a new scene. It is required that the GameObject is at the root of its current scene.")]
    public class MoveGameObjectToScene : GetSceneActionBase
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The Root GameObject to move to the referenced scene")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Only root GameObject can be moved, set to true to get the root of the gameobject if necessary, else watch for failure events.")]
        public FsmBool findRootIfNecessary;
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("True if the merge succeeded"), UIHint(UIHint.Variable)]
        public FsmBool success;
        [HutongGames.PlayMaker.Tooltip("Event sent if merge succeeded")]
        public FsmEvent successEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if merge failed. Check log for information")]
        public FsmEvent failureEvent;
        private GameObject _go;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base._sceneFound)
            {
                this._go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (this.findRootIfNecessary.Value)
                {
                    this._go = this._go.transform.root.gameObject;
                }
                if (this._go.transform.parent == null)
                {
                    SceneManager.MoveGameObjectToScene(this._go, base._scene);
                    this.success.Value = true;
                    base.Fsm.Event(this.successEvent);
                }
                else
                {
                    base.LogError("GameObject must be a root ");
                    this.success.Value = false;
                    base.Fsm.Event(this.failureEvent);
                }
                base.Fsm.Event(base.sceneFoundEvent);
                this._go = null;
            }
            base.Finish();
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.findRootIfNecessary = null;
            this.success = null;
            this.successEvent = null;
            this.failureEvent = null;
        }
    }
}

