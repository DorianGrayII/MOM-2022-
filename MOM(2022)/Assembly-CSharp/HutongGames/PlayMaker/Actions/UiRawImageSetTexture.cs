using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the texture of a UI RawImage component.")]
    public class UiRawImageSetTexture : ComponentAction<RawImage>
    {
        [RequiredField]
        [CheckForComponent(typeof(RawImage))]
        [Tooltip("The GameObject with the UI RawImage component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The texture of the UI RawImage component.")]
        public FsmTexture texture;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        private RawImage _texture;

        private Texture _originalTexture;

        public override void Reset()
        {
            this.gameObject = null;
            this.texture = null;
            this.resetOnExit = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this._texture = base.cachedComponent;
            }
            this._originalTexture = this._texture.texture;
            this.DoSetValue();
            base.Finish();
        }

        private void DoSetValue()
        {
            if (this._texture != null)
            {
                this._texture.texture = this.texture.Value;
            }
        }

        public override void OnExit()
        {
            if (!(this._texture == null) && this.resetOnExit.Value)
            {
                this._texture.texture = this._originalTexture;
            }
        }
    }
}
