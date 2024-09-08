using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the source image sprite of a UI Image component.")]
    public class UiImageSetSprite : ComponentAction<Image>
    {
        [RequiredField]
        [CheckForComponent(typeof(Image))]
        [Tooltip("The GameObject with the Image UI component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The source sprite of the UI Image component.")]
        [ObjectType(typeof(Sprite))]
        public FsmObject sprite;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private Image image;

        private Sprite originalSprite;

        public override void Reset()
        {
            this.gameObject = null;
            this.resetOnExit = false;
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

        private void DoSetImageSourceValue()
        {
            if (!(this.image == null))
            {
                this.image.sprite = this.sprite.Value as Sprite;
            }
        }

        public override void OnExit()
        {
            if (!(this.image == null) && this.resetOnExit.Value)
            {
                this.image.sprite = this.originalSprite;
            }
        }
    }
}
