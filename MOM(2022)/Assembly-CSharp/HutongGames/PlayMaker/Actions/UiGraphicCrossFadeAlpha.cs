namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Tweens the alpha of the CanvasRenderer color associated with this Graphic.")]
    public class UiGraphicCrossFadeAlpha : ComponentAction<Graphic>
    {
        [RequiredField, CheckForComponent(typeof(Graphic)), HutongGames.PlayMaker.Tooltip("The GameObject with an Unity UI component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The alpha target")]
        public FsmFloat alpha;
        [HutongGames.PlayMaker.Tooltip("The duration of the tween")]
        public FsmFloat duration;
        [HutongGames.PlayMaker.Tooltip("Should ignore Time.scale?")]
        public FsmBool ignoreTimeScale;
        private Graphic uiComponent;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.uiComponent = base.cachedComponent;
            }
            this.uiComponent.CrossFadeAlpha(this.alpha.Value, this.duration.Value, this.ignoreTimeScale.Value);
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.alpha = null;
            this.duration = 1f;
            this.ignoreTimeScale = null;
        }
    }
}

