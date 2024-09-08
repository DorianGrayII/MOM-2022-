using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Enable or disable Canvas Raycasting. Optionally reset on state exit")]
    public class UiCanvasEnableRaycast : ComponentAction<PlayMakerCanvasRaycastFilterProxy>
    {
        [RequiredField]
        [Tooltip("The GameObject to enable or disable Canvas Raycasting on.")]
        public FsmOwnerDefault gameObject;

        public FsmBool enableRaycasting;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        public bool everyFrame;

        [SerializeField]
        private PlayMakerCanvasRaycastFilterProxy raycastFilterProxy;

        private bool originalValue;

        public override void Reset()
        {
            this.gameObject = null;
            this.enableRaycasting = false;
            this.resetOnExit = null;
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            if (this.gameObject == null)
            {
                this.gameObject = new FsmOwnerDefault();
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCacheAddComponent(ownerDefaultTarget))
            {
                this.raycastFilterProxy = base.cachedComponent;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCacheAddComponent(ownerDefaultTarget))
            {
                this.raycastFilterProxy = base.cachedComponent;
                this.originalValue = this.raycastFilterProxy.RayCastingEnabled;
            }
            this.DoAction();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoAction();
        }

        private void DoAction()
        {
            if (this.raycastFilterProxy != null)
            {
                this.raycastFilterProxy.RayCastingEnabled = this.enableRaycasting.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this.raycastFilterProxy == null) && this.resetOnExit.Value)
            {
                this.raycastFilterProxy.RayCastingEnabled = this.originalValue;
            }
        }
    }
}
