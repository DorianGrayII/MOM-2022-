namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Set The Fill Amount on a UI Image")]
    public class UiImageGetFillAmount : ComponentAction<Image>
    {
        [RequiredField, CheckForComponent(typeof(Image)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Image component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The fill amount.")]
        public FsmFloat ImageFillAmount;
        [HutongGames.PlayMaker.Tooltip("Repeats every frame")]
        public bool everyFrame;
        private Image image;

        private void DoGetFillAmount()
        {
            if (this.image != null)
            {
                this.ImageFillAmount.Value = this.image.fillAmount;
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.ImageFillAmount = null;
            this.everyFrame = false;
        }
    }
}

