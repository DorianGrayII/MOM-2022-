namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Fade from a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
    public class CameraFadeIn : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Color to fade from. E.g., Fade up from black.")]
        public FsmColor color;
        [RequiredField, HasFloatSlider(0f, 10f), HutongGames.PlayMaker.Tooltip("Fade in time in seconds.")]
        public FsmFloat time;
        [HutongGames.PlayMaker.Tooltip("Event to send when finished.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;
        private float startTime;
        private float currentTime;
        private Color colorLerp;

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0f;
            this.colorLerp = this.color.get_Value();
        }

        public override void OnGUI()
        {
            GUI.color = this.colorLerp;
            GUI.DrawTexture(new Rect(0f, 0f, (float) Screen.width, (float) Screen.height), ActionHelpers.WhiteTexture);
            GUI.color = GUI.color;
        }

        public override void OnUpdate()
        {
            this.currentTime = !this.realTime ? (this.currentTime + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
            this.colorLerp = Color.Lerp(this.color.get_Value(), Color.clear, this.currentTime / this.time.Value);
            if (this.currentTime > this.time.Value)
            {
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.color = (FsmColor) Color.black;
            this.time = 1f;
            this.finishEvent = null;
        }
    }
}

