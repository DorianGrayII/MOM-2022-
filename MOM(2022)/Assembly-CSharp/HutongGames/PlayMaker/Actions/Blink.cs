namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Effects), HutongGames.PlayMaker.Tooltip("Turns a Game Object on/off in a regular repeating pattern.")]
    public class Blink : ComponentAction<Renderer>
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to blink on/off.")]
        public FsmOwnerDefault gameObject;
        [HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Time to stay off in seconds.")]
        public FsmFloat timeOff;
        [HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Time to stay on in seconds.")]
        public FsmFloat timeOn;
        [HutongGames.PlayMaker.Tooltip("Should the object start in the active/visible state?")]
        public FsmBool startOn;
        [HutongGames.PlayMaker.Tooltip("Only effect the renderer, keeping other components active.")]
        public bool rendererOnly;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;
        private float startTime;
        private float timer;
        private bool blinkOn;

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.timer = 0f;
            this.UpdateBlinkState(this.startOn.Value);
        }

        public override void OnUpdate()
        {
            this.timer = !this.realTime ? (this.timer + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
            if (this.blinkOn && (this.timer > this.timeOn.Value))
            {
                this.UpdateBlinkState(false);
            }
            if (!this.blinkOn && (this.timer > this.timeOff.Value))
            {
                this.UpdateBlinkState(true);
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.timeOff = 0.5f;
            this.timeOn = 0.5f;
            this.rendererOnly = true;
            this.startOn = false;
            this.realTime = false;
        }

        private void UpdateBlinkState(bool state)
        {
            GameObject go = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value();
            if (go != null)
            {
                if (!this.rendererOnly)
                {
                    go.SetActive(state);
                }
                else if (base.UpdateCache(go))
                {
                    base.renderer.enabled = state;
                }
                this.blinkOn = state;
                this.startTime = FsmTime.RealtimeSinceStartup;
                this.timer = 0f;
            }
        }
    }
}

