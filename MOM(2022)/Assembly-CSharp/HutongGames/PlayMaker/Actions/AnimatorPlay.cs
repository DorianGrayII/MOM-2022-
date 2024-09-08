using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Plays a state. This could be used to synchronize your animation with audio or synchronize an Animator over the network.")]
    public class AnimatorPlay : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The name of the state that will be played.")]
        public FsmString stateName;

        [Tooltip("The layer where the state is.")]
        public FsmInt layer;

        [Tooltip("The normalized time at which the state will play")]
        public FsmFloat normalizedTime;

        [Tooltip("Repeat every frame. Useful when using normalizedTime to manually control the animation.")]
        public bool everyFrame;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.stateName = null;
            this.layer = new FsmInt
            {
                UseVariable = true
            };
            this.normalizedTime = new FsmFloat
            {
                UseVariable = true
            };
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            this._animator = ownerDefaultTarget.GetComponent<Animator>();
            this.DoAnimatorPlay();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoAnimatorPlay();
        }

        private void DoAnimatorPlay()
        {
            if (this._animator != null)
            {
                int num = (this.layer.IsNone ? (-1) : this.layer.Value);
                float num2 = (this.normalizedTime.IsNone ? float.NegativeInfinity : this.normalizedTime.Value);
                this._animator.Play(this.stateName.Value, num, num2);
            }
        }
    }
}
