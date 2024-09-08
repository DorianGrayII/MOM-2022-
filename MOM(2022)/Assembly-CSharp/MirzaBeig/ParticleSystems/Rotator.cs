namespace MirzaBeig.ParticleSystems
{
    using System;
    using UnityEngine;

    public class Rotator : MonoBehaviour
    {
        public Vector3 localRotationSpeed;
        public Vector3 worldRotationSpeed;
        public bool executeInEditMode;
        public bool unscaledTime;

        private void Awake()
        {
        }

        private void OnRenderObject()
        {
            if (this.executeInEditMode && !Application.isPlaying)
            {
                this.rotate();
            }
        }

        private void rotate()
        {
            float num = !this.unscaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            if (this.localRotationSpeed != Vector3.zero)
            {
                base.transform.Rotate(this.localRotationSpeed * num, Space.Self);
            }
            if (this.worldRotationSpeed != Vector3.zero)
            {
                base.transform.Rotate(this.worldRotationSpeed * num, Space.World);
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                this.rotate();
            }
        }
    }
}

