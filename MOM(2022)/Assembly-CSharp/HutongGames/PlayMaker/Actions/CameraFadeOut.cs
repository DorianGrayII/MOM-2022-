using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Fade to a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
    public class CameraFadeOut : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Color to fade to. E.g., Fade to black.")]
        public FsmColor color;

        [RequiredField]
        [HasFloatSlider(0f, 10f)]
        [Tooltip("Fade out time in seconds.")]
        public FsmFloat time;

        [Tooltip("Event to send when finished.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;

        private float startTime;

        private float currentTime;

        private Color colorLerp;

        public override void Reset()
        {
            this.color = Color.black;
            this.time = 1f;
            this.finishEvent = null;
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0f;
            this.colorLerp = Color.clear;
        }

        public override void OnUpdate()
        {
            if (this.realTime)
            {
                this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
            }
            else
            {
                this.currentTime += Time.deltaTime;
            }
            this.colorLerp = Color.Lerp(Color.clear, this.color.Value, this.currentTime / this.time.Value);
            if (this.currentTime > this.time.Value && this.finishEvent != null)
            {
                base.Fsm.Event(this.finishEvent);
            }
        }

        public override void OnGUI()
        {
            Color obj = GUI.color;
            GUI.color = this.colorLerp;
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), ActionHelpers.WhiteTexture);
            GUI.color = obj;
        }
    }
}
