using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Device)]
    [Tooltip("Gets info on a touch event.")]
    public class GetTouchInfo : FsmStateAction
    {
        [Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
        public FsmInt fingerId;

        [Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
        public FsmBool normalize;

        [UIHint(UIHint.Variable)]
        public FsmVector3 storePosition;

        [UIHint(UIHint.Variable)]
        public FsmFloat storeX;

        [UIHint(UIHint.Variable)]
        public FsmFloat storeY;

        [UIHint(UIHint.Variable)]
        public FsmVector3 storeDeltaPosition;

        [UIHint(UIHint.Variable)]
        public FsmFloat storeDeltaX;

        [UIHint(UIHint.Variable)]
        public FsmFloat storeDeltaY;

        [UIHint(UIHint.Variable)]
        public FsmFloat storeDeltaTime;

        [UIHint(UIHint.Variable)]
        public FsmInt storeTapCount;

        public bool everyFrame = true;

        private float screenWidth;

        private float screenHeight;

        public override void Reset()
        {
            this.fingerId = new FsmInt
            {
                UseVariable = true
            };
            this.normalize = true;
            this.storePosition = null;
            this.storeDeltaPosition = null;
            this.storeDeltaTime = null;
            this.storeTapCount = null;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.screenWidth = Screen.width;
            this.screenHeight = Screen.height;
            this.DoGetTouchInfo();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetTouchInfo();
        }

        private void DoGetTouchInfo()
        {
            if (Input.touchCount <= 0)
            {
                return;
            }
            Touch[] touches = Input.touches;
            for (int i = 0; i < touches.Length; i++)
            {
                Touch touch = touches[i];
                if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
                {
                    float num = ((!this.normalize.Value) ? touch.position.x : (touch.position.x / this.screenWidth));
                    float num2 = ((!this.normalize.Value) ? touch.position.y : (touch.position.y / this.screenHeight));
                    if (!this.storePosition.IsNone)
                    {
                        this.storePosition.Value = new Vector3(num, num2, 0f);
                    }
                    this.storeX.Value = num;
                    this.storeY.Value = num2;
                    float num3 = ((!this.normalize.Value) ? touch.deltaPosition.x : (touch.deltaPosition.x / this.screenWidth));
                    float num4 = ((!this.normalize.Value) ? touch.deltaPosition.y : (touch.deltaPosition.y / this.screenHeight));
                    if (!this.storeDeltaPosition.IsNone)
                    {
                        this.storeDeltaPosition.Value = new Vector3(num3, num4, 0f);
                    }
                    this.storeDeltaX.Value = num3;
                    this.storeDeltaY.Value = num4;
                    this.storeDeltaTime.Value = touch.deltaTime;
                    this.storeTapCount.Value = touch.tapCount;
                }
            }
        }
    }
}
