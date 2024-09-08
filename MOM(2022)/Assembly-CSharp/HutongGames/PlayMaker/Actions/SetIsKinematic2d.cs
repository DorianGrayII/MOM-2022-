using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Controls whether 2D physics affects the Game Object.")]
    public class SetIsKinematic2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The isKinematic value")]
        public FsmBool isKinematic;

        public override void Reset()
        {
            this.gameObject = null;
            this.isKinematic = false;
        }

        public override void OnEnter()
        {
            this.DoSetIsKinematic();
            base.Finish();
        }

        private void DoSetIsKinematic()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.isKinematic = this.isKinematic.Value;
            }
        }
    }
}
