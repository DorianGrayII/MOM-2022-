namespace MOM
{
    using MHUtils;
    using System;
    using UnityEngine;

    public class LocationSpawnController : MonoBehaviour
    {
        public Location owner;
        private bool isVisible;
        private int delay;

        private void OnBecameInvisible()
        {
            this.isVisible = false;
        }

        private void OnBecameVisible()
        {
            this.isVisible = true;
        }

        private void Update()
        {
            if ((this.owner == null) || this.owner.spawnedCorrectly)
            {
                Destroy(this);
            }
            else if (this.isVisible && (this.delay < 1))
            {
                if (this.owner.model == null)
                {
                    Destroy(this);
                }
                else
                {
                    Vector3 offset = HexCoordinates.HexToWorld3D(this.owner.GetPosition());
                    this.owner.SetHexPosition(this.owner.GetPosition(), offset);
                    this.delay = 10;
                }
            }
        }
    }
}

