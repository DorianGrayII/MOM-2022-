namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Sends events when a 2d object is touched. Optionally filter by a fingerID. NOTE: Uses the MainCamera!")]
    public class TouchObject2dEvent : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Collider2D)), HutongGames.PlayMaker.Tooltip("The Game Object to detect touches on.")]
        public FsmOwnerDefault gameObject;
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
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the 2d position where the object was touched.")]
        public FsmVector2 storeHitPoint;

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
                            Vector2 position = touch.position;
                            RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay((Vector3) position), float.PositiveInfinity);
                            Fsm.RecordLastRaycastHit2DInfo(base.Fsm, rayIntersection);
                            if ((rayIntersection.transform != null) && (rayIntersection.transform.gameObject == ownerDefaultTarget))
                            {
                                this.storeFingerId.Value = touch.fingerId;
                                this.storeHitPoint.set_Value(rayIntersection.point);
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
        }
    }
}

