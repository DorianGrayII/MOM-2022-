using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Get the ScaleFactor of a CanvasScaler.")]
    public class UiCanvasScalerGetScaleFactor : ComponentAction<CanvasScaler>
    {
        [RequiredField]
        [CheckForComponent(typeof(CanvasScaler))]
        [Tooltip("The GameObject with a UI CanvasScaler component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The scaleFactor of the CanvasScaler component.")]
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
            this.DoGetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        private void DoGetValue()
        {
            if (this.component != null)
            {
                this.scaleFactor.Value = this.component.scaleFactor;
            }
        }
    }
}
