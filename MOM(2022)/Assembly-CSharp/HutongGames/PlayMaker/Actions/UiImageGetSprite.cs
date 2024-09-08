namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Gets the source image sprite of a UI Image component.")]
    public class UiImageGetSprite : ComponentAction<Image>
    {
        [RequiredField, CheckForComponent(typeof(Image)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI Image component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The source sprite of the UI Image component."), UIHint(UIHint.Variable), ObjectType(typeof(Sprite))]
        public FsmObject sprite;
        private Image image;

        private void DoSetImageSourceValue()
        {
            if (this.image != null)
            {
                this.sprite.set_Value(this.image.sprite);
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.image = base.cachedComponent;
            }
            this.DoSetImageSourceValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.sprite = null;
        }
    }
}

