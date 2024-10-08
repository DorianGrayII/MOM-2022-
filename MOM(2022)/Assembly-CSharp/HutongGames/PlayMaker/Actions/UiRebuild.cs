using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Rebuild a UI Graphic component.")]
    public class UiRebuild : ComponentAction<Graphic>
    {
        [RequiredField]
        [CheckForComponent(typeof(Graphic))]
        [Tooltip("The GameObject with the UI Graphic component.")]
        public FsmOwnerDefault gameObject;

        public CanvasUpdate canvasUpdate;

        [Tooltip("Only Rebuild when state exits.")]
        public bool rebuildOnExit;

        private Graphic graphic;

        public override void Reset()
        {
            this.gameObject = null;
            this.canvasUpdate = CanvasUpdate.LatePreRender;
            this.rebuildOnExit = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.graphic = base.cachedComponent;
            }
            if (!this.rebuildOnExit)
            {
                this.DoAction();
            }
            base.Finish();
        }

        private void DoAction()
        {
            if (this.graphic != null)
            {
                this.graphic.Rebuild(this.canvasUpdate);
            }
        }

        public override void OnExit()
        {
            if (this.rebuildOnExit)
            {
                this.DoAction();
            }
        }
    }
}
