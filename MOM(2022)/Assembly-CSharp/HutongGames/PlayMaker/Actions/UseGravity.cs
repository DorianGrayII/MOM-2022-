using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Sets whether a Game Object's Rigidbody is affected by Gravity.")]
    public class UseGravity : ComponentAction<Rigidbody>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        public FsmBool useGravity;

        public override void Reset()
        {
            this.gameObject = null;
            this.useGravity = true;
        }

        public override void OnEnter()
        {
            this.DoUseGravity();
            base.Finish();
        }

        private void DoUseGravity()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody.useGravity = this.useGravity.Value;
            }
        }
    }
}
