namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Set The Fill Amount on a UI Image")]
    public class UiImageSetFillAmount : ComponentAction<Image>
    {
        [RequiredField, CheckForComponent(typeof(Image)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Image component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("The fill amount.")]
        public FsmFloat ImageFillAmount;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Image image;

        private void DoSetFillAmount()
        {
            if (this.image != null)
            {
                this.image.fillAmount = this.ImageFillAmount.Value;
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.ImageFillAmount = 1f;
            this.everyFrame = false;
        }
    }
}

