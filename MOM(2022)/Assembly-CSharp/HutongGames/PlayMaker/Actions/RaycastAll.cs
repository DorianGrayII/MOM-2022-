using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [Tooltip("Casts a Ray against all Colliders in the scene. Use either a GameObject or Vector3 world position as the origin of the ray. Use GetRaycastAllInfo to get more detailed info.")]
    public class RaycastAll : FsmStateAction
    {
        public static RaycastHit[] RaycastAllHitInfo;

        [Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
        public FsmOwnerDefault fromGameObject;

        [Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
        public FsmVector3 fromPosition;

        [Tooltip("A vector3 direction vector")]
        public FsmVector3 direction;

        [Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
        public Space space;

        [Tooltip("The length of the ray. Set to -1 for infinity.")]
        public FsmFloat distance;

        [ActionSection("Result")]
        [Tooltip("Event to send if the ray hits an object.")]
        [UIHint(UIHint.Variable)]
        public FsmEvent hitEvent;

        [Tooltip("Set a bool variable to true if hit something, otherwise false.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeDidHit;

        [Tooltip("Store the GameObjects hit in an array variable.")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
        public FsmArray storeHitObjects;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the world position of the ray hit point and store it in a variable.")]
        public FsmVector3 storeHitPoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the normal at the hit point and store it in a variable.")]
        public FsmVector3 storeHitNormal;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
        public FsmFloat storeHitDistance;

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

        private int repeat;

        public override void Reset()
        {
            this.fromGameObject = null;
            this.fromPosition = new FsmVector3
            {
                UseVariable = true
            };
            this.direction = new FsmVector3
            {
                UseVariable = true
            };
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
            this.debugColor = Color.yellow;
            this.debug = false;
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

        private void DoRaycast()
        {
            this.repeat = this.repeatInterval.Value;
            if (this.distance.Value == 0f)
            {
                return;
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
            Vector3 vector = ((ownerDefaultTarget != null) ? ownerDefaultTarget.transform.position : this.fromPosition.Value);
            float num = float.PositiveInfinity;
            if (this.distance.Value > 0f)
            {
                num = this.distance.Value;
            }
            Vector3 vector2 = this.direction.Value;
            if (ownerDefaultTarget != null && this.space == Space.Self)
            {
                vector2 = ownerDefaultTarget.transform.TransformDirection(this.direction.Value);
            }
            RaycastAll.RaycastAllHitInfo = Physics.RaycastAll(vector, vector2, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            bool flag = RaycastAll.RaycastAllHitInfo.Length != 0;
            this.storeDidHit.Value = flag;
            if (flag)
            {
                GameObject[] array = new GameObject[RaycastAll.RaycastAllHitInfo.Length];
                for (int i = 0; i < RaycastAll.RaycastAllHitInfo.Length; i++)
                {
                    RaycastHit raycastHit = RaycastAll.RaycastAllHitInfo[i];
                    array[i] = raycastHit.collider.gameObject;
                }
                FsmArray fsmArray = this.storeHitObjects;
                object[] values = array;
                fsmArray.Values = values;
                this.storeHitPoint.Value = base.Fsm.RaycastHitInfo.point;
                this.storeHitNormal.Value = base.Fsm.RaycastHitInfo.normal;
                this.storeHitDistance.Value = base.Fsm.RaycastHitInfo.distance;
                base.Fsm.Event(this.hitEvent);
            }
            if (this.debug.Value)
            {
                float num2 = Mathf.Min(num, 1000f);
                Debug.DrawLine(vector, vector + vector2 * num2, this.debugColor.Value);
            }
        }
    }
}
