using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [Tooltip("Rewinds the named animation.")]
    public class RewindAnimation : BaseAnimationAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animation))]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Animation)]
        public FsmString animName;

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
        }

        public override void OnEnter()
        {
            this.DoRewindAnimation();
            base.Finish();
        }

        private void DoRewindAnimation()
        {
            if (!string.IsNullOrEmpty(this.animName.Value))
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (base.UpdateCache(ownerDefaultTarget))
                {
                    base.animation.Rewind(this.animName.Value);
                }
            }
        }
    }
}
