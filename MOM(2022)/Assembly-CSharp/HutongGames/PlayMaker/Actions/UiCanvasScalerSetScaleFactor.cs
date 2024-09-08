namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the ScaleFactor of a CanvasScaler.")]
    public class UiCanvasScalerSetScaleFactor : ComponentAction<CanvasScaler>
    {
        [RequiredField, CheckForComponent(typeof(CanvasScaler)), HutongGames.PlayMaker.Tooltip("The GameObject with a UI CanvasScaler component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The scaleFactor of the UI CanvasScaler.")]
        public FsmFloat scaleFactor;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private CanvasScaler component;

        private void DoSetValue()
        {
            if (this.component != null)
            {
                this.component.scaleFactor = this.scaleFactor.Value;
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.scaleFactor = null;
            this.everyFrame = false;
        }
    }
}

