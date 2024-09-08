using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Gets the Mass of a Game Object's Rigid Body 2D.")]
    public class GetMass2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with a Rigidbody2D attached.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the mass of gameObject.")]
        public FsmFloat storeResult;

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            this.DoGetMass();
            base.Finish();
        }

        private void DoGetMass()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.storeResult.Value = base.rigidbody2d.mass;
            }
        }
    }
}
