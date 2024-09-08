using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Tweens the alpha of the CanvasRenderer color associated with this Graphic.")]
    public class UiGraphicCrossFadeAlpha : ComponentAction<Graphic>
    {
        [RequiredField]
        [CheckForComponent(typeof(Graphic))]
        [Tooltip("The GameObject with an Unity UI component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The alpha target")]
        public FsmFloat alpha;

        [Tooltip("The duration of the tween")]
        public FsmFloat duration;

        [Tooltip("Should ignore Time.scale?")]
        public FsmBool ignoreTimeScale;

        private Graphic uiComponent;

        public override void Reset()
        {
            this.gameObject = null;
            this.alpha = null;
            this.duration = 1f;
            this.ignoreTimeScale = null;
        }

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
    }
}
