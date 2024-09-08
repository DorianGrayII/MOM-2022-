namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Get the ScaleFactor of a CanvasScaler.")]
    public class UiCanvasScalerGetScaleFactor : ComponentAction<CanvasScaler>
    {
        [RequiredField, CheckForComponent(typeof(CanvasScaler)), HutongGames.PlayMaker.Tooltip("The GameObject with a UI CanvasScaler component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The scaleFactor of the CanvasScaler component.")]
        public FsmFloat scaleFactor;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame, useful for animation")]
        public bool everyFrame;
        private CanvasScaler component;

        private void DoGetValue()
        {
            if (this.component != null)
            {
                this.scaleFactor.Value = this.component.scaleFactor;
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.scaleFactor = null;
            this.everyFrame = false;
        }
    }
}

