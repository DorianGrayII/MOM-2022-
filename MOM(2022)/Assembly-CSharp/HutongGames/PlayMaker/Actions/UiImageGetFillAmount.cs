using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Set The Fill Amount on a UI Image")]
    public class UiImageGetFillAmount : ComponentAction<Image>
    {
        [RequiredField]
        [CheckForComponent(typeof(Image))]
        [Tooltip("The GameObject with the UI Image component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The fill amount.")]
        public FsmFloat ImageFillAmount;

        [Tooltip("Repeats every frame")]
        public bool everyFrame;

        private Image image;

        public override void Reset()
        {
            this.gameObject = null;
            this.ImageFillAmount = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.image = base.cachedComponent;
            }
            this.DoGetFillAmount();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFillAmount();
        }

        private void DoGetFillAmount()
        {
            if (this.image != null)
            {
                this.ImageFillAmount.Value = this.image.fillAmount;
            }
        }
    }
}
