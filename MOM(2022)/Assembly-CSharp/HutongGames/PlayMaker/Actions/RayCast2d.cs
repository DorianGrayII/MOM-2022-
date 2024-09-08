using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info.")]
    public class RayCast2d : FsmStateAction
    {
        [ActionSection("Setup")]
        [Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;

        [Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
        public FsmVector2 fromPosition;

        [Tooltip("A vector2 direction vector")]
        public FsmVector2 direction;

        [Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
        public Space space;

        [Tooltip("The length of the ray. Set to -1 for infinity.")]
        public FsmFloat distance;

        [Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
        public FsmInt minDepth;

        [Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
        public FsmInt maxDepth;

        [ActionSection("Result")]
        [Tooltip("Event to send if the ray hits an object.")]
        [UIHint(UIHint.Variable)]
        public FsmEvent hitEvent;

        [Tooltip("Set a bool variable to true if hit something, otherwise false.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeDidHit;

        [Tooltip("Store the game object hit in a variable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeHitObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
        public FsmVector2 storeHitPoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the 2d normal at the hit point and store it in a variable.")]
        public FsmVector2 storeHitNormal;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
        public FsmFloat storeHitDistance;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalized then this value is simply the distance between the origin and the hit point. If the direction is not normalized then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
        public FsmFloat storeHitFraction;

        [ActionSection("Filter")]
        [Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
        public FsmInt repeatInterval;

        [UIHint(UIHint.Layer)]
        [Tooltip("Pick only from these layers.")]
        public FsmInt[] layerMask;

        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;

        [ActionSection("Debug")]
        [Tooltip("The color to use for the debug line.")]
        public FsmColor debugColor;

        [Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
        public FsmBool debug;

        private Transform _transform;

        private int repeat;

        public override void Reset()
        {
            this.fromGameObject = null;
            this.fromPosition = new FsmVector2
            {
                UseVariable = true
            };
            this.direction = new FsmVector2
            {
                UseVariable = true
            };
            this.space = Space.Self;
            this.minDepth = new FsmInt
            {
                UseVariable = true
            };
            this.maxDepth = new FsmInt
            {
                UseVariable = true
            };
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
            this.debugColor = Color.yellow;
            this.debug = false;
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

        private void DoRaycast()
        {
            this.repeat = this.repeatInterval.Value;
            if (!(Math.Abs(this.distance.Value) < Mathf.Epsilon))
            {
                Vector2 value = this.fromPosition.Value;
                if (this._transform != null)
                {
                    value.x += this._transform.position.x;
                    value.y += this._transform.position.y;
                }
                float a = float.PositiveInfinity;
                if (this.distance.Value > 0f)
                {
                    a = this.distance.Value;
                }
                Vector2 normalized = this.direction.Value.normalized;
                if (this._transform != null && this.space == Space.Self)
                {
                    Vector3 vector = this._transform.TransformDirection(new Vector3(this.direction.Value.x, this.direction.Value.y, 0f));
                    normalized.x = vector.x;
                    normalized.y = vector.y;
                }
                RaycastHit2D info;
                if (this.minDepth.IsNone && this.maxDepth.IsNone)
                {
                    info = Physics2D.Raycast(value, normalized, a, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
                }
                else
                {
                    float num = (this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value));
                    float num2 = (this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value));
                    info = Physics2D.Raycast(value, normalized, a, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
                }
                Fsm.RecordLastRaycastHit2DInfo(base.Fsm, info);
                bool flag = info.collider != null;
                this.storeDidHit.Value = flag;
                if (flag)
                {
                    this.storeHitObject.Value = info.collider.gameObject;
                    this.storeHitPoint.Value = info.point;
                    this.storeHitNormal.Value = info.normal;
                    this.storeHitDistance.Value = info.distance;
                    this.storeHitFraction.Value = info.fraction;
                    base.Fsm.Event(this.hitEvent);
                }
                if (this.debug.Value)
                {
                    float num3 = Mathf.Min(a, 1000f);
                    Vector3 vector2 = new Vector3(value.x, value.y, 0f);
                    Vector3 vector3 = new Vector3(normalized.x, normalized.y, 0f);
                    Vector3 end = vector2 + vector3 * num3;
                    Debug.DrawLine(vector2, end, this.debugColor.Value);
                }
            }
        }
    }
}
