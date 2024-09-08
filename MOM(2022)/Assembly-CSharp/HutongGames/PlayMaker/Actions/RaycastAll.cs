namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene. Use either a GameObject or Vector3 world position as the origin of the ray. Use GetRaycastAllInfo to get more detailed info.")]
    public class RaycastAll : FsmStateAction
    {
        public static RaycastHit[] RaycastAllHitInfo;
        [HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;
        [HutongGames.PlayMaker.Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
        public FsmVector3 fromPosition;
        [HutongGames.PlayMaker.Tooltip("A vector3 direction vector")]
        public FsmVector3 direction;
        [HutongGames.PlayMaker.Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("The length of the ray. Set to -1 for infinity.")]
        public FsmFloat distance;
        [ActionSection("Result"), HutongGames.PlayMaker.Tooltip("Event to send if the ray hits an object."), UIHint(UIHint.Variable)]
        public FsmEvent hitEvent;
        [HutongGames.PlayMaker.Tooltip("Set a bool variable to true if hit something, otherwise false."), UIHint(UIHint.Variable)]
        public FsmBool storeDidHit;
        [HutongGames.PlayMaker.Tooltip("Store the GameObjects hit in an array variable."), UIHint(UIHint.Variable), ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000)]
        public FsmArray storeHitObjects;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the world position of the ray hit point and store it in a variable.")]
        public FsmVector3 storeHitPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the normal at the hit point and store it in a variable.")]
        public FsmVector3 storeHitNormal;
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
        private int repeat;

        private void DoRaycast()
        {
            this.repeat = this.repeatInterval.Value;
            if (this.distance.Value != 0f)
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
                Vector3 origin = (ownerDefaultTarget != null) ? ownerDefaultTarget.transform.position : this.fromPosition.get_Value();
                float positiveInfinity = float.PositiveInfinity;
                if (this.distance.Value > 0f)
                {
                    positiveInfinity = this.distance.Value;
                }
                Vector3 direction = this.direction.get_Value();
                if ((ownerDefaultTarget != null) && (this.space == Space.Self))
                {
                    direction = ownerDefaultTarget.transform.TransformDirection(this.direction.get_Value());
                }
                RaycastAllHitInfo = Physics.RaycastAll(origin, direction, positiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
                bool flag = RaycastAllHitInfo.Length != 0;
                this.storeDidHit.Value = flag;
                if (flag)
                {
                    GameObject[] objArray = new GameObject[RaycastAllHitInfo.Length];
                    int index = 0;
                    while (true)
                    {
                        if (index >= RaycastAllHitInfo.Length)
                        {
                            this.storeHitObjects.Values = objArray;
                            this.storeHitPoint.set_Value(base.Fsm.get_RaycastHitInfo().point);
                            this.storeHitNormal.set_Value(base.Fsm.get_RaycastHitInfo().normal);
                            this.storeHitDistance.Value = base.Fsm.get_RaycastHitInfo().distance;
                            base.Fsm.Event(this.hitEvent);
                            break;
                        }
                        RaycastHit hit = RaycastAllHitInfo[index];
                        objArray[index] = hit.collider.gameObject;
                        index++;
                    }
                }
                if (this.debug.Value)
                {
                    Debug.DrawLine(origin, origin + (direction * Mathf.Min(positiveInfinity, 1000f)), this.debugColor.get_Value());
                }
            }
        }

        public override void OnEnter()
        {
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
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.fromPosition = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.direction = vector2;
            this.space = Space.Self;
            this.distance = 100f;
            this.hitEvent = null;
            this.storeDidHit = null;
            this.storeHitObjects = null;
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

