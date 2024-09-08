namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene.A linecast is an imaginary line between two points in world space. Any object making contact with the beam can be detected and reported. This differs from the similar raycast in that raycasting specifies the line using an origin and direction.Use GetRaycastHit2dInfo to get more detailed info.")]
    public class LineCast2d : FsmStateAction
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
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("Event to send if the ray hits an object."), UIHint(UIHint.Variable)]
        public FsmEvent hitEvent;
        [HutongGames.PlayMaker.Tooltip("Set a bool variable to true if hit something, otherwise false."), UIHint(UIHint.Variable)]
        public FsmBool storeDidHit;
        [HutongGames.PlayMaker.Tooltip("Store the game object hit in a variable."), UIHint(UIHint.Variable)]
        public FsmGameObject storeHitObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
        public FsmVector2 storeHitPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the 2d normal at the hit point and store it in a variable.")]
        public FsmVector2 storeHitNormal;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
        public FsmFloat storeHitDistance;
        [ActionSection("Filter"), HutongGames.PlayMaker.Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
        public FsmInt repeatInterval;
        [UIHint(UIHint.Layer), HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [ActionSection("Debug"), HutongGames.PlayMaker.Tooltip("The color to use for the debug line.")]
        public FsmColor debugColor;
        [HutongGames.PlayMaker.Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
        public FsmBool debug;
        private Transform _fromTrans;
        private Transform _toTrans;
        private int repeat;

        private unsafe void DoRaycast()
        {
            RaycastHit2D hitd;
            this.repeat = this.repeatInterval.Value;
            Vector2 start = this.fromPosition.get_Value();
            if (this._fromTrans != null)
            {
                float* singlePtr1 = &start.x;
                singlePtr1[0] += this._fromTrans.position.x;
                float* singlePtr2 = &start.y;
                singlePtr2[0] += this._fromTrans.position.y;
            }
            Vector2 end = this.toPosition.get_Value();
            if (this._toTrans != null)
            {
                float* singlePtr3 = &end.x;
                singlePtr3[0] += this._toTrans.position.x;
                float* singlePtr4 = &end.y;
                singlePtr4[0] += this._toTrans.position.y;
            }
            if (this.minDepth.IsNone && this.maxDepth.IsNone)
            {
                hitd = Physics2D.Linecast(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            }
            else
            {
                float minDepth = this.minDepth.IsNone ? float.NegativeInfinity : ((float) this.minDepth.Value);
                hitd = Physics2D.Linecast(start, end, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, this.maxDepth.IsNone ? float.PositiveInfinity : ((float) this.maxDepth.Value));
            }
            Fsm.RecordLastRaycastHit2DInfo(base.Fsm, hitd);
            bool flag = hitd.collider != null;
            this.storeDidHit.Value = flag;
            if (flag)
            {
                this.storeHitObject.set_Value(hitd.collider.gameObject);
                this.storeHitPoint.set_Value(hitd.point);
                this.storeHitNormal.set_Value(hitd.normal);
                this.storeHitDistance.Value = hitd.fraction;
                base.Fsm.Event(this.hitEvent);
            }
            if (this.debug.Value)
            {
                Vector3 vector3 = new Vector3(end.x, end.y, 0f);
                Debug.DrawLine(new Vector3(start.x, start.y, 0f), vector3, this.debugColor.get_Value());
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            if (ownerDefaultTarget != null)
            {
                this._fromTrans = ownerDefaultTarget.transform;
            }
            GameObject obj3 = this.toGameObject.get_Value();
            if (obj3 != null)
            {
                this._toTrans = obj3.transform;
            }
            this.DoRaycast();
            if (this.repeatInterval.Value == 0)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.repeat--;
            if (this.repeat == 0)
            {
                this.DoRaycast();
            }
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
            this.hitEvent = null;
            this.storeDidHit = null;
            this.storeHitObject = null;
            this.storeHitPoint = null;
            this.storeHitNormal = null;
            this.storeHitDistance = null;
            this.repeatInterval = 1;
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.debugColor = (FsmColor) Color.yellow;
            this.debug = false;
        }
    }
}

