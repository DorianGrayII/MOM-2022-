using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets the source image sprite of a UI Image component.")]
    public class UiImageGetSprite : ComponentAction<Image>
    {
        [RequiredField]
        [CheckForComponent(typeof(Image))]
        [Tooltip("The GameObject with the UI Image component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The source sprite of the UI Image component.")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(Sprite))]
        public FsmObject sprite;

        private Image image;

        public override void Reset()
        {
            this.gameObject = null;
            this.sprite = null;
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

        private void DoSetImageSourceValue()
        {
            if (this.image != null)
            {
                this.sprite.Value = this.image.sprite;
            }
        }
    }
}
