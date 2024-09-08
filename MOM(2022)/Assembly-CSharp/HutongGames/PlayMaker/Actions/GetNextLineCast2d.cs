namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Iterate through a list of all colliders detected by a LineCastThe colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    public class GetNextLineCast2d : FsmStateAction
    {
        [ActionSection("Setup"), HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;
        [HutongGames.PlayMaker.Tooltip("Start ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
        public FsmVector2 fromPosition;
        [HutongGames.PlayMaker.Tooltip("End ray at game object position. \nOr use From Position parameter.")]
        public FsmGameObject toGameObject;
        [HutongGames.PlayMaker.Tooltip("End ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add toPosition to the ToGameObject position")]
        public FsmVector2 toPosition;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
        public FsmInt minDepth;
        [HutongGames.PlayMaker.Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
        public FsmInt maxDepth;
        [HutongGames.PlayMaker.Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again"), UIHint(UIHint.Variable)]
        public FsmBool resetFlag;
        [ActionSection("Filter"), UIHint(UIHint.Layer), HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("Store the number of colliders found for this overlap."), UIHint(UIHint.Variable)]
        public FsmInt collidersCount;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the next collider in a GameObject variable.")]
        public FsmGameObject storeNextCollider;
        [HutongGames.PlayMaker.Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
        public FsmVector2 storeNextHitPoint;
        [HutongGames.PlayMaker.Tooltip("Get the 2d normal at the next hit point and store it in a variable.")]
        public FsmVector2 storeNextHitNormal;
        [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
        public FsmFloat storeNextHitDistance;
        [HutongGames.PlayMaker.Tooltip("Event to send to get the next collider.")]
        public FsmEvent loopEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when there are no more colliders to iterate.")]
        public FsmEvent finishedEvent;
        private RaycastHit2D[] hits;
        private int colliderCount;
        private int nextColliderIndex;

        private void DoGetNextCollider()
        {
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.hits = null;
                this.nextColliderIndex = 0;
                base.Fsm.Event(this.finishedEvent);
            }
            else
            {
                Fsm.RecordLastRaycastHit2DInfo(base.Fsm, this.hits[this.nextColliderIndex]);
                this.storeNextCollider.set_Value(this.hits[this.nextColliderIndex].collider.gameObject);
                this.storeNextHitPoint.set_Value(this.hits[this.nextColliderIndex].point);
                this.storeNextHitNormal.set_Value(this.hits[this.nextColliderIndex].normal);
                this.storeNextHitDistance.Value = this.hits[this.nextColliderIndex].fraction;
                if (this.nextColliderIndex >= this.colliderCount)
                {
                    this.hits = new RaycastHit2D[0];
                    this.nextColliderIndex = 0;
                    base.Fsm.Event(this.finishedEvent);
                }
                else
                {
                    this.nextColliderIndex++;
                    if (this.loopEvent != null)
                    {
                        base.Fsm.Event(this.loopEvent);
                    }
                }
            }
        }

        private unsafe RaycastHit2D[] GetLineCastAll()
        {
            Vector2 start = this.fromPosition.get_Value();
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            if (ownerDefaultTarget != null)
            {
                float* singlePtr1 = &start.x;
                singlePtr1[0] += ownerDefaultTarget.transform.position.x;
                float* singlePtr2 = &start.y;
                singlePtr2[0] += ownerDefaultTarget.transform.position.y;
            }
            Vector2 end = this.toPosition.get_Value();
            GameObject obj3 = this.toGameObject.get_Value();
            if (obj3 != null)
            {
                float* singlePtr3 = &end.x;
                singlePtr3[0] += obj3.transform.position.x;
                float* singlePtr4 = &end.y;
                singlePtr4[0] += obj3.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
            {
                return Physics2D.LinecastAll(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            }
            float minDepth = this.minDepth.IsNone ? float.NegativeInfinity : ((float) this.minDepth.Value);
            return Physics2D.LinecastAll(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, this.maxDepth.IsNone ? float.PositiveInfinity : ((float) this.maxDepth.Value));
        }

        public override void OnEnter()
        {
            if ((this.hits == null) || this.resetFlag.Value)
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

        public override void Reset()
        {
            this.fromGameObject = null;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.fromPosition = vector1;
            this.toGameObject = null;
            FsmVector2 vector2 = new FsmVector2();
            vector2.UseVariable = true;
            this.toPosition = vector2;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.minDepth = num1;
            FsmInt num2 = new FsmInt();
            num2.UseVariable = true;
            this.maxDepth = num2;
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
    }
}

