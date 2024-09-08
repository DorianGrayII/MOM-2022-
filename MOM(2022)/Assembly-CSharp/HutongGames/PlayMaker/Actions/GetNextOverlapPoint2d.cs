using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Iterate through a list of all colliders that overlap a point in space.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders overlap this point.")]
    public class GetNextOverlapPoint2d : FsmStateAction
    {
        [ActionSection("Setup")]
        [Tooltip("Point using the gameObject position. \nOr use From Position parameter.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Point as a world position. \nOr use gameObject parameter. If both define, will add position to the gameObject position")]
        public FsmVector2 position;

        [Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
        public FsmInt minDepth;

        [Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
        public FsmInt maxDepth;

        [Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
        [UIHint(UIHint.Variable)]
        public FsmBool resetFlag;

        [ActionSection("Filter")]
        [UIHint(UIHint.Layer)]
        [Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;

        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;

        [ActionSection("Result")]
        [Tooltip("Store the number of colliders found for this overlap.")]
        [UIHint(UIHint.Variable)]
        public FsmInt collidersCount;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the next collider in a GameObject variable.")]
        public FsmGameObject storeNextCollider;

        [Tooltip("Event to send to get the next collider.")]
        public FsmEvent loopEvent;

        [Tooltip("Event to send when there are no more colliders to iterate.")]
        public FsmEvent finishedEvent;

        private Collider2D[] colliders;

        private int colliderCount;

        private int nextColliderIndex;

        public override void Reset()
        {
            this.gameObject = null;
            this.position = new FsmVector2
            {
                UseVariable = true
            };
            this.minDepth = new FsmInt
            {
                UseVariable = true
            };
            this.maxDepth = new FsmInt
            {
                UseVariable = true
            };
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.resetFlag = null;
            this.collidersCount = null;
            this.storeNextCollider = null;
            this.loopEvent = null;
            this.finishedEvent = null;
        }

        public override void OnEnter()
        {
            if (this.colliders == null || this.resetFlag.Value)
            {
                this.nextColliderIndex = 0;
                this.colliders = this.GetOverlapPointAll();
                this.colliderCount = this.colliders.Length;
                this.collidersCount.Value = this.colliderCount;
                this.resetFlag.Value = false;
            }
            this.DoGetNextCollider();
            base.Finish();
        }

        private void DoGetNextCollider()
        {
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.nextColliderIndex = 0;
                base.Fsm.Event(this.finishedEvent);
                return;
            }
            this.storeNextCollider.Value = this.colliders[this.nextColliderIndex].gameObject;
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.colliders = null;
                this.nextColliderIndex = 0;
                base.Fsm.Event(this.finishedEvent);
                return;
            }
            this.nextColliderIndex++;
            if (this.loopEvent != null)
            {
                base.Fsm.Event(this.loopEvent);
            }
        }

        private Collider2D[] GetOverlapPointAll()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            Vector2 value = this.position.Value;
            if (ownerDefaultTarget != null)
            {
                value.x += ownerDefaultTarget.transform.position.x;
                value.y += ownerDefaultTarget.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
            {
                return Physics2D.OverlapPointAll(value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            }
            float num = (this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value));
            float num2 = (this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value));
            return Physics2D.OverlapPointAll(value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
        }
    }
}
