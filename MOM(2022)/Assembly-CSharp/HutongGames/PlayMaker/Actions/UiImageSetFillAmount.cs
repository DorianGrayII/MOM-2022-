using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Set The Fill Amount on a UI Image")]
    public class UiImageSetFillAmount : ComponentAction<Image>
    {
        [RequiredField]
        [CheckForComponent(typeof(Image))]
        [Tooltip("The GameObject with the UI Image component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [HasFloatSlider(0f, 1f)]
        [Tooltip("The fill amount.")]
        public FsmFloat ImageFillAmount;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Image image;

        public override void Reset()
        {
            this.gameObject = null;
            this.ImageFillAmount = 1f;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.image = base.cachedComponent;
            }
            this.DoSetFillAmount();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFillAmount();
        }

        private void DoSetFillAmount()
        {
            if (this.image != null)
            {
                this.image.fillAmount = this.ImageFillAmount.Value;
            }
        }
    }
}
