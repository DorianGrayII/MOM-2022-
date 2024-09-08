﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Effects), HutongGames.PlayMaker.Tooltip("Randomly flickers a Game Object on/off.")]
    public class Flicker : ComponentAction<Renderer>
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to flicker.")]
        public FsmOwnerDefault gameObject;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("The frequency of the flicker in seconds.")]
        public FsmFloat frequency;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Amount of time flicker is On (0-1). E.g. Use 0.95 for an occasional flicker.")]
        public FsmFloat amountOn;
        [HutongGames.PlayMaker.Tooltip("Only effect the renderer, leaving other components active.")]
        public bool rendererOnly;
        [HutongGames.PlayMaker.Tooltip("Ignore time scale. Useful if flickering UI when the game is paused.")]
        public bool realTime;
        private float startTime;
        private float timer;

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.timer = 0f;
        }

        public override void OnUpdate()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this.timer = !this.realTime ? (this.timer + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
                if (this.timer > this.frequency.Value)
                {
                    bool flag = UnityEngine.Random.Range((float) 0f, (float) 1f) < this.amountOn.Value;
                    if (!this.rendererOnly)
                    {
                        ownerDefaultTarget.SetActive(flag);
                    }
                    else if (base.UpdateCache(ownerDefaultTarget))
                    {
                        base.renderer.enabled = flag;
                    }
                    this.startTime = this.timer;
                    this.timer = 0f;
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.frequency = 0.1f;
            this.amountOn = 0.5f;
            this.rendererOnly = true;
            this.realTime = false;
        }
    }
}

