namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Gets info on a touch event.")]
    public class GetTouchInfo : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
        public FsmInt fingerId;
        [HutongGames.PlayMaker.Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
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

        private void DoGetTouchInfo()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (this.fingerId.IsNone || (touch.fingerId == this.fingerId.Value))
                    {
                        float x = !this.normalize.Value ? touch.position.x : (touch.position.x / this.screenWidth);
                        float y = !this.normalize.Value ? touch.position.y : (touch.position.y / this.screenHeight);
                        if (!this.storePosition.IsNone)
                        {
                            this.storePosition.set_Value(new Vector3(x, y, 0f));
                        }
                        this.storeX.Value = x;
                        this.storeY.Value = y;
                        float num4 = !this.normalize.Value ? touch.deltaPosition.x : (touch.deltaPosition.x / this.screenWidth);
                        float num5 = !this.normalize.Value ? touch.deltaPosition.y : (touch.deltaPosition.y / this.screenHeight);
                        if (!this.storeDeltaPosition.IsNone)
                        {
                            this.storeDeltaPosition.set_Value(new Vector3(num4, num5, 0f));
                        }
                        this.storeDeltaX.Value = num4;
                        this.storeDeltaY.Value = num5;
                        this.storeDeltaTime.Value = touch.deltaTime;
                        this.storeTapCount.Value = touch.tapCount;
                    }
                }
            }
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

        public override void Reset()
        {
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.fingerId = num1;
            this.normalize = true;
            this.storePosition = null;
            this.storeDeltaPosition = null;
            this.storeDeltaTime = null;
            this.storeTapCount = null;
            this.everyFrame = true;
        }
    }
}

