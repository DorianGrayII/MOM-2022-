using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Set the isTrigger option of a Collider2D. Optionally set all collider2D found on the gameobject Target.")]
    public class SetCollider2dIsTrigger : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Collider2D))]
        [Tooltip("The GameObject with the Collider2D attached")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The flag value")]
        public FsmBool isTrigger;

        [Tooltip("Set all Colliders on the GameObject target")]
        public bool setAllColliders;

        public override void Reset()
        {
            this.gameObject = null;
            this.isTrigger = false;
            this.setAllColliders = false;
        }

        public override void OnEnter()
        {
            this.DoSetIsTrigger();
            base.Finish();
        }

        private void DoSetIsTrigger()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                return;
            }
            if (this.setAllColliders)
            {
                Collider2D[] components = ownerDefaultTarget.GetComponents<Collider2D>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].isTrigger = this.isTrigger.Value;
                }
            }
            else if (ownerDefaultTarget.GetComponent<Collider2D>() != null)
            {
                ownerDefaultTarget.GetComponent<Collider2D>().isTrigger = this.isTrigger.Value;
            }
        }
    }
}
