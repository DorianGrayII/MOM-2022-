namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns the culling of this Animator component. Optionally sends events.\nIf true ('AlwaysAnimate'): always animate the entire character. Object is animated even when offscreen.\nIf False ('BasedOnRenderers') animation is disabled when renderers are not visible.")]
    public class GetAnimatorCullingMode : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
        public FsmBool alwaysAnimate;
        [HutongGames.PlayMaker.Tooltip("Event send if culling mode is 'AlwaysAnimate'")]
        public FsmEvent alwaysAnimateEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if culling mode is 'BasedOnRenders'")]
        public FsmEvent basedOnRenderersEvent;
        private Animator _animator;

        private void DoCheckCulling()
        {
            if (this._animator != null)
            {
                bool flag = this._animator.cullingMode == AnimatorCullingMode.AlwaysAnimate;
                this.alwaysAnimate.Value = flag;
                if (flag)
                {
                    base.Fsm.Event(this.alwaysAnimateEvent);
                }
                else
                {
                    base.Fsm.Event(this.basedOnRenderersEvent);
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if (this._animator == null)
                {
                    base.Finish();
                }
                else
                {
                    this.DoCheckCulling();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.alwaysAnimate = null;
            this.alwaysAnimateEvent = null;
            this.basedOnRenderersEvent = null;
        }
    }
}

