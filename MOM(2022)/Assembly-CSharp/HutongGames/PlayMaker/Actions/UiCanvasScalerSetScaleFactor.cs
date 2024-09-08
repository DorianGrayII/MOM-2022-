using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the ScaleFactor of a CanvasScaler.")]
    public class UiCanvasScalerSetScaleFactor : ComponentAction<CanvasScaler>
    {
        [RequiredField]
        [CheckForComponent(typeof(CanvasScaler))]
        [Tooltip("The GameObject with a UI CanvasScaler component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The scaleFactor of the UI CanvasScaler.")]
        public FsmFloat scaleFactor;

        [Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;

        private CanvasScaler component;

        public override void Reset()
        {
            this.gameObject = null;
            this.scaleFactor = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.component = base.cachedComponent;
            }
            this.DoSetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetValue();
        }

        private void DoSetValue()
        {
            if (this.component != null)
            {
                this.component.scaleFactor = this.scaleFactor.Value;
            }
        }
    }
}
