namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info.")]
    public class RayCast2d : FsmStateAction
    {
        [ActionSection("Setup"), HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;
        [HutongGames.PlayMaker.Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
        public FsmVector2 fromPosition;
        [HutongGames.PlayMaker.Tooltip("A vector2 direction vector")]
        public FsmVector2 direction;
        [HutongGames.PlayMaker.Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("The length of the ray. Set to -1 for infinity.")]
        public FsmFloat distance;
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
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalized then this value is simply the distance between the origin and the hit point. If the direction is not normalized then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
        public FsmFloat storeHitFraction;
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
        private Transform _transform;
        private int repeat;

        private unsafe void DoRaycast()
        {
            this.repeat = this.repeatInterval.Value;
            if (Math.Abs(this.distance.Value) >= Mathf.Epsilon)
            {
                RaycastHit2D hitd;
                Vector2 origin = this.fromPosition.get_Value();
                if (this._transform != null)
                {
                    float* singlePtr1 = &origin.x;
                    singlePtr1[0] += this._transform.position.x;
                    float* singlePtr2 = &origin.y;
                    singlePtr2[0] += this._transform.position.y;
                }
                float positiveInfinity = float.PositiveInfinity;
                if (this.distance.Value > 0f)
                {
                    positiveInfinity = this.distance.Value;
                }
                Vector2 normalized = this.direction.get_Value().normalized;
                if ((this._transform != null) && (this.space == Space.Self))
                {
                    Vector3 vector4 = this._transform.TransformDirection(new Vector3(this.direction.get_Value().x, this.direction.get_Value().y, 0f));
                    normalized.x = vector4.x;
                    normalized.y = vector4.y;
                }
                if (this.minDepth.IsNone && this.maxDepth.IsNone)
                {
                    hitd = Physics2D.Raycast(origin, normalized, positiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
                }
                else
                {
                    float minDepth = this.minDepth.IsNone ? float.NegativeInfinity : ((float) this.minDepth.Value);
                    hitd = Physics2D.Raycast(origin, normalized, positiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), minDepth, this.maxDepth.IsNone ? float.PositiveInfinity : ((float) this.maxDepth.Value));
                }
                Fsm.RecordLastRaycastHit2DInfo(base.Fsm, hitd);
                bool flag = hitd.collider != null;
                this.storeDidHit.Value = flag;
                if (flag)
                {
                    this.storeHitObject.set_Value(hitd.collider.gameObject);
                    this.storeHitPoint.set_Value(hitd.point);
                    this.storeHitNormal.set_Value(hitd.normal);
                    this.storeHitDistance.Value = hitd.distance;
                    this.storeHitFraction.Value = hitd.fraction;
                    base.Fsm.Event(this.hitEvent);
                }
                if (this.debug.Value)
                {
                    Vector3 vector5 = new Vector3(normalized.x, normalized.y, 0f);
                    Debug.DrawLine(new Vector3(origin.x, origin.y, 0f), new Vector3(origin.x, origin.y, 0f) + (vector5 * Mathf.Min(positiveInfinity, 1000f)), this.debugColor.get_Value());
                }
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            if (ownerDefaultTarget != null)
            {
                this._transform = ownerDefaultTarget.transform;
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
            FsmVector2 vector2 = new FsmVector2();
            vector2.UseVariable = true;
            this.direction = vector2;
            this.space = Space.Self;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.minDepth = num1;
            FsmInt num2 = new FsmInt();
            num2.UseVariable = true;
            this.maxDepth = num2;
            this.distance = 100f;
            this.hitEvent = null;
            this.storeDidHit = null;
            this.storeHitObject = null;
            this.storeHitPoint = null;
            this.storeHitNormal = null;
            this.storeHitDistance = null;
            this.storeHitFraction = null;
            this.repeatInterval = 1;
            this.layerMask = new FsmInt[0];
            this.invertMask = false;
            this.debugColor = (FsmColor) Color.yellow;
            this.debug = false;
        }
    }
}

