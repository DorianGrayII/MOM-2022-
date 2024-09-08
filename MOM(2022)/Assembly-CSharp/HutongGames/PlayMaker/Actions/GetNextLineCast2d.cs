using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Iterate through a list of all colliders detected by a LineCastThe colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    public class GetNextLineCast2d : FsmStateAction
    {
        [ActionSection("Setup")]
        [Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;

        [Tooltip("Start ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
        public FsmVector2 fromPosition;

        [Tooltip("End ray at game object position. \nOr use From Position parameter.")]
        public FsmGameObject toGameObject;

        [Tooltip("End ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add toPosition to the ToGameObject position")]
        public FsmVector2 toPosition;

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

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the next collider in a GameObject variable.")]
        public FsmGameObject storeNextCollider;

        [Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
        public FsmVector2 storeNextHitPoint;

        [Tooltip("Get the 2d normal at the next hit point and store it in a variable.")]
        public FsmVector2 storeNextHitNormal;

        [Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
        public FsmFloat storeNextHitDistance;

        [Tooltip("Event to send to get the next collider.")]
        public FsmEvent loopEvent;

        [Tooltip("Event to send when there are no more colliders to iterate.")]
        public FsmEvent finishedEvent;

        private RaycastHit2D[] hits;

        private int colliderCount;

        private int nextColliderIndex;

        public override void Reset()
        {
            this.fromGameObject = null;
            this.fromPosition = new FsmVector2
            {
                UseVariable = true
            };
            this.toGameObject = null;
            this.toPosition = new FsmVector2
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
            this.storeNextHitPoint = null;
            this.storeNextHitNormal = null;
            this.storeNextHitDistance = null;
            this.loopEvent = null;
            this.finishedEvent = null;
        }

        public override void OnEnter()
        {
            if (this.hits == null || this.resetFlag.Value)
            {
                this.nextColliderIndex = 0;
                this.hits = this.GetLineCastAll();
                this.colliderCount = this.hits.Length;
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
                this.hits = null;
                this.nextColliderIndex = 0;
                base.Fsm.Event(this.finishedEvent);
                return;
            }
            Fsm.RecordLastRaycastHit2DInfo(base.Fsm, this.hits[this.nextColliderIndex]);
            this.storeNextCollider.Value = this.hits[this.nextColliderIndex].collider.gameObject;
            this.storeNextHitPoint.Value = this.hits[this.nextColliderIndex].point;
            this.storeNextHitNormal.Value = this.hits[this.nextColliderIndex].normal;
            this.storeNextHitDistance.Value = this.hits[this.nextColliderIndex].fraction;
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.hits = new RaycastHit2D[0];
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

        private RaycastHit2D[] GetLineCastAll()
        {
            Vector2 value = this.fromPosition.Value;
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            if (ownerDefaultTarget != null)
            {
                value.x += ownerDefaultTarget.transform.position.x;
                value.y += ownerDefaultTarget.transform.position.y;
            }
            Vector2 value2 = this.toPosition.Value;
            GameObject value3 = this.toGameObject.Value;
            if (value3 != null)
            {
                value2.x += value3.transform.position.x;
                value2.y += value3.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
            {
                return Physics2D.LinecastAll(value, value2, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            }
            float num = (this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value));
            float num2 = (this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value));
            return Physics2D.LinecastAll(value, value2, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
        }
    }
}
