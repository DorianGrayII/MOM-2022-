namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the source image sprite of a UI Image component.")]
    public class UiImageSetSprite : ComponentAction<Image>
    {
        [RequiredField, CheckForComponent(typeof(Image)), HutongGames.PlayMaker.Tooltip("The GameObject with the Image UI component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The source sprite of the UI Image component."), ObjectType(typeof(Sprite))]
        public FsmObject sprite;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private Image image;
        private Sprite originalSprite;

        private void DoSetImageSourceValue()
        {
            if (this.image != null)
            {
                this.image.sprite = this.sprite.get_Value() as Sprite;
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.image = base.cachedComponent;
            }
            this.originalSprite = this.image.sprite;
            this.DoSetImageSourceValue();
            base.Finish();
        }

        public override void OnExit()
        {
            if ((this.image != null) && this.resetOnExit.Value)
            {
                this.image.sprite = this.originalSprite;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.resetOnExit = false;
        }
    }
}

