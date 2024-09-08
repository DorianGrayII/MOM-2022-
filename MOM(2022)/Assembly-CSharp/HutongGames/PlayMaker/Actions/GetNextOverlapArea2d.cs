namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Iterate through a list of all colliders that fall within a rectangular area.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
    public class GetNextOverlapArea2d : FsmStateAction
    {
        [ActionSection("Setup"), HutongGames.PlayMaker.Tooltip("First corner of the rectangle area using the game object position. \nOr use firstCornerPosition parameter.")]
        public FsmOwnerDefault firstCornerGameObject;
        [HutongGames.PlayMaker.Tooltip("First Corner of the rectangle area as a world position. \nOr use FirstCornerGameObject parameter. If both define, will add firstCornerPosition to the FirstCornerGameObject position")]
        public FsmVector2 firstCornerPosition;
        [HutongGames.PlayMaker.Tooltip("Second corner of the rectangle area using the game object position. \nOr use secondCornerPosition parameter.")]
        public FsmGameObject secondCornerGameObject;
        [HutongGames.PlayMaker.Tooltip("Second Corner rectangle area as a world position. \nOr use SecondCornerGameObject parameter. If both define, will add secondCornerPosition to the SecondCornerGameObject position")]
        public FsmVector2 secondCornerPosition;
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
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the next collider in a GameObject variable.")]
        public FsmGameObject storeNextCollider;
        [HutongGames.PlayMaker.Tooltip("Event to send to get the next collider.")]
        public FsmEvent loopEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when there are no more colliders to iterate.")]
        public FsmEvent finishedEvent;
        private Collider2D[] colliders;
        private int colliderCount;
        private int nextColliderIndex;

        private void DoGetNextCollider()
        {
            if (this.nextColliderIndex >= this.colliderCount)
            {
                this.nextColliderIndex = 0;
                base.Fsm.Event(this.finishedEvent);
            }
            else
            {
                this.storeNextCollider.set_Value(this.colliders[this.nextColliderIndex].gameObject);
                if (this.nextColliderIndex >= this.colliderCount)
                {
                    this.colliders = null;
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

        private unsafe Collider2D[] GetOverlapAreaAll()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.firstCornerGameObject);
            Vector2 pointA = this.firstCornerPosition.get_Value();
            if (ownerDefaultTarget != null)
            {
                float* singlePtr1 = &pointA.x;
                singlePtr1[0] += ownerDefaultTarget.transform.position.x;
                float* singlePtr2 = &pointA.y;
                singlePtr2[0] += ownerDefaultTarget.transform.position.y;
            }
            GameObject obj3 = this.secondCornerGameObject.get_Value();
            Vector2 pointB = this.secondCornerPosition.get_Value();
            if (obj3 != null)
            {
                float* singlePtr3 = &pointB.x;
                singlePtr3[0] += obj3.transform.position.x;
                float* singlePtr4 = &pointB.y;
                singlePtr4[0] += obj3.transform.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
            {
                return Physics2D.OverlapAreaAll(pointA, pointB, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            }
            float minDepth = this.minDepth.IsNone ? float.NegativeInfinity : ((float) this.minDepth.Value);
            return Physics2D.OverlapAreaAll(pointA, pointB, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, this.maxDepth.IsNone ? float.PositiveInfinity : ((float) this.maxDepth.Value));
        }

        public override void OnEnter()
        {
            if ((this.colliders == null) || this.resetFlag.Value)
            {
                this.nextColliderIndex = 0;
                this.colliders = this.GetOverlapAreaAll();
                this.colliderCount = this.colliders.Length;
                this.collidersCount.Value = this.colliderCount;
                this.resetFlag.Value = false;
            }
            this.DoGetNextCollider();
            base.Finish();
        }

        public override void Reset()
        {
            this.firstCornerGameObject = null;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.firstCornerPosition = vector1;
            this.secondCornerGameObject = null;
            FsmVector2 vector2 = new FsmVector2();
            vector2.UseVariable = true;
            this.secondCornerPosition = vector2;
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
            this.loopEvent = null;
            this.finishedEvent = null;
        }
    }
}

