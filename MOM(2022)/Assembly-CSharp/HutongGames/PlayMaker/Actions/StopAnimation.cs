using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animation)]
    [Tooltip("Stops all playing Animations on a Game Object. Optionally, specify a single Animation to Stop.")]
    public class StopAnimation : BaseAnimationAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animation))]
        public FsmOwnerDefault gameObject;

        [Tooltip("Leave empty to stop all playing animations.")]
        [UIHint(UIHint.Animation)]
        public FsmString animName;

        public override void Reset()
        {
            this.gameObject = null;
            this.animName = null;
        }

        public override void OnEnter()
        {
            this.DoStopAnimation();
            base.Finish();
        }

        private void DoStopAnimation()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                if (FsmString.IsNullOrEmpty(this.animName))
                {
                    base.animation.Stop();
                }
                else
                {
                    base.animation.Stop(this.animName.Value);
                }
            }
        }
    }
}
