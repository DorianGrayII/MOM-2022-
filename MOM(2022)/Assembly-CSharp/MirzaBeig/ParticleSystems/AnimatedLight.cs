﻿namespace MirzaBeig.ParticleSystems
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [RequireComponent(typeof(Light))]
    public class AnimatedLight : MonoBehaviour
    {
        private Light light;
        public float duration = 1f;
        private bool evaluating = true;
        public Gradient colourOverLifetime;
        public AnimationCurve intensityOverLifetime;
        public bool loop;
        public bool autoDestruct;
        private Color startColour;
        private float startIntensity;

        public AnimatedLight()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f) };
            this.intensityOverLifetime = new AnimationCurve(keys);
            this.loop = true;
        }

        private void Awake()
        {
            this.light = base.GetComponent<Light>();
        }

        private void OnDisable()
        {
            this.light.color = this.startColour;
            this.light.intensity = this.startIntensity;
            this.time = 0f;
            this.evaluating = true;
            this.light.color = this.startColour * this.colourOverLifetime.Evaluate(0f);
            this.light.intensity = this.startIntensity * this.intensityOverLifetime.Evaluate(0f);
        }

        private void OnEnable()
        {
        }

        private void Start()
        {
            this.startColour = this.light.color;
            this.startIntensity = this.light.intensity;
            this.light.color = this.startColour * this.colourOverLifetime.Evaluate(0f);
            this.light.intensity = this.startIntensity * this.intensityOverLifetime.Evaluate(0f);
        }

        private void Update()
        {
            if (this.evaluating)
            {
                if (this.time < this.duration)
                {
                    this.time += Time.deltaTime;
                    if (this.time > this.duration)
                    {
                        if (this.autoDestruct)
                        {
                            Destroy(base.gameObject);
                        }
                        else if (this.loop)
                        {
                            this.time = 0f;
                        }
                        else
                        {
                            this.time = this.duration;
                            this.evaluating = false;
                        }
                    }
                }
                if (this.time <= this.duration)
                {
                    float time = this.time / this.duration;
                    this.light.color = this.startColour * this.colourOverLifetime.Evaluate(time);
                    this.light.intensity = this.startIntensity * this.intensityOverLifetime.Evaluate(time);
                }
            }
        }

        public float time { get; set; }
    }
}

