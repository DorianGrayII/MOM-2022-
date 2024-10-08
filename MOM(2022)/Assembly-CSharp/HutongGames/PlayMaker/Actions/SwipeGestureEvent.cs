using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Device)]
    [Tooltip("Sends an event when a swipe is detected.")]
    public class SwipeGestureEvent : FsmStateAction
    {
        [Tooltip("How far a touch has to travel to be considered a swipe. Uses normalized distance (e.g. 1 = 1 screen diagonal distance). Should generally be a very small number.")]
        public FsmFloat minSwipeDistance;

        [Tooltip("Event to send when swipe left detected.")]
        public FsmEvent swipeLeftEvent;

        [Tooltip("Event to send when swipe right detected.")]
        public FsmEvent swipeRightEvent;

        [Tooltip("Event to send when swipe up detected.")]
        public FsmEvent swipeUpEvent;

        [Tooltip("Event to send when swipe down detected.")]
        public FsmEvent swipeDownEvent;

        private float screenDiagonalSize;

        private float minSwipeDistancePixels;

        private bool touchStarted;

        private Vector2 touchStartPos;

        public override void Reset()
        {
            this.minSwipeDistance = 0.1f;
            this.swipeLeftEvent = null;
            this.swipeRightEvent = null;
            this.swipeUpEvent = null;
            this.swipeDownEvent = null;
        }

        public override void OnEnter()
        {
            this.screenDiagonalSize = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
            this.minSwipeDistancePixels = this.minSwipeDistance.Value * this.screenDiagonalSize;
        }

        public override void OnUpdate()
        {
            if (Input.touchCount <= 0)
            {
                return;
            }
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
            case TouchPhase.Began:
                this.touchStarted = true;
                this.touchStartPos = touch.position;
                break;
            case TouchPhase.Ended:
                if (this.touchStarted)
                {
                    this.TestForSwipeGesture(touch);
                    this.touchStarted = false;
                }
                break;
            case TouchPhase.Canceled:
                this.touchStarted = false;
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                break;
            }
        }

        private void TestForSwipeGesture(Touch touch)
        {
            Vector2 position = touch.position;
            if (Vector2.Distance(position, this.touchStartPos) > this.minSwipeDistancePixels)
            {
                float x = position.y - this.touchStartPos.y;
                float y = position.x - this.touchStartPos.x;
                float num = 57.29578f * Mathf.Atan2(y, x);
                num = (360f + num - 45f) % 360f;
                Debug.Log(num);
                if (num < 90f)
                {
                    base.Fsm.Event(this.swipeRightEvent);
                }
                else if (num < 180f)
                {
                    base.Fsm.Event(this.swipeDownEvent);
                }
                else if (num < 270f)
                {
                    base.Fsm.Event(this.swipeLeftEvent);
                }
                else
                {
                    base.Fsm.Event(this.swipeUpEvent);
                }
            }
        }
    }
}
