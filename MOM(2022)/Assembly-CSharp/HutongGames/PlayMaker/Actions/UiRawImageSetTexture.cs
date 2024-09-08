namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Sets the texture of a UI RawImage component.")]
    public class UiRawImageSetTexture : ComponentAction<RawImage>
    {
        [RequiredField, CheckForComponent(typeof(RawImage)), HutongGames.PlayMaker.Tooltip("The GameObject with the UI RawImage component.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The texture of the UI RawImage component.")]
        public FsmTexture texture;
        [HutongGames.PlayMaker.Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;
        private RawImage _texture;
        private Texture _originalTexture;

        private void DoSetValue()
        {
            if (this._texture != null)
            {
                this._texture.texture = this.texture.get_Value();
            }
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

        public override void OnExit()
        {
            if ((this._texture != null) && this.resetOnExit.Value)
            {
                this._texture.texture = this._originalTexture;
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.texture = null;
            this.resetOnExit = null;
        }
    }
}

