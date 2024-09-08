namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), ActionTarget(typeof(GameObject), "gameObject", false), HutongGames.PlayMaker.Tooltip("Sends events when an object is touched. Optionally filter by a fingerID. NOTE: Uses the MainCamera!")]
    public class TouchObjectEvent : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Collider)), HutongGames.PlayMaker.Tooltip("The Game Object to detect touches on.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("How far from the camera is the Game Object pickable.")]
        public FsmFloat pickDistance;
        [HutongGames.PlayMaker.Tooltip("Only detect touches that match this fingerID, or set to None.")]
        public FsmInt fingerId;
        [ActionSection("Events"), HutongGames.PlayMaker.Tooltip("Event to send on touch began.")]
        public FsmEvent touchBegan;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch moved.")]
        public FsmEvent touchMoved;
        [HutongGames.PlayMaker.Tooltip("Event to send on stationary touch.")]
        public FsmEvent touchStationary;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch ended.")]
        public FsmEvent touchEnded;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch cancel.")]
        public FsmEvent touchCanceled;
        [ActionSection("Store Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the fingerId of the touch.")]
        public FsmInt storeFingerId;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the world position where the object was touched.")]
        public FsmVector3 storeHitPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the surface normal vector where the object was touched.")]
        public FsmVector3 storeHitNormal;

        public override void OnUpdate()
        {
            if (Camera.main == null)
            {
                base.LogError("No MainCamera defined!");
                base.Finish();
            }
            else if (Input.touchCount > 0)
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (ownerDefaultTarget != null)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (this.fingerId.IsNone || (touch.fingerId == this.fingerId.Value))
                        {
                            RaycastHit hit;
                            Vector2 position = touch.position;
                            Physics.Raycast(Camera.main.ScreenPointToRay((Vector3) position), out hit, this.pickDistance.Value);
                            base.Fsm.set_RaycastHitInfo(hit);
                            if ((hit.transform != null) && (hit.transform.gameObject == ownerDefaultTarget))
                            {
                                this.storeFingerId.Value = touch.fingerId;
                                this.storeHitPoint.set_Value(hit.point);
                                this.storeHitNormal.set_Value(hit.normal);
                                switch (touch.phase)
                                {
                                    case TouchPhase.Began:
                                        base.Fsm.Event(this.touchBegan);
                                        return;

                                    case TouchPhase.Moved:
                                        base.Fsm.Event(this.touchMoved);
                                        return;

                                    case TouchPhase.Stationary:
                                        base.Fsm.Event(this.touchStationary);
                                        return;

                                    case TouchPhase.Ended:
                                        base.Fsm.Event(this.touchEnded);
                                        return;

                                    case TouchPhase.Canceled:
                                        base.Fsm.Event(this.touchCanceled);
                                        return;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.pickDistance = 100f;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.fingerId = num1;
            this.touchBegan = null;
            this.touchMoved = null;
            this.touchStationary = null;
            this.touchEnded = null;
            this.touchCanceled = null;
            this.storeFingerId = null;
            this.storeHitPoint = null;
            this.storeHitNormal = null;
        }
    }
}

