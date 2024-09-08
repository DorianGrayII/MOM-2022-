using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Forces a Game Object's Rigid Body 2D to wake up.")]
    public class WakeUp2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with a Rigidbody2d attached")]
        public FsmOwnerDefault gameObject;

        public override void Reset()
        {
            this.gameObject = null;
        }

        public override void OnEnter()
        {
            this.DoWakeUp();
            base.Finish();
        }

        private void DoWakeUp()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.WakeUp();
            }
        }
    }
}
