using MHUtils;
using UnityEngine;

namespace MOM
{
    public class LocationSpawnController : MonoBehaviour
    {
        public Location owner;

        private bool isVisible;

        private int delay;

        private void Update()
        {
            if (this.owner == null || this.owner.spawnedCorrectly)
            {
                Object.Destroy(this);
            }
            else if (this.isVisible && this.delay < 1)
            {
                if (this.owner.model == null)
                {
                    Object.Destroy(this);
                    return;
                }
                Vector3 offset = HexCoordinates.HexToWorld3D(this.owner.GetPosition());
                this.owner.SetHexPosition(this.owner.GetPosition(), offset);
                this.delay = 10;
            }
        }

        private void OnBecameVisible()
        {
            this.isVisible = true;
        }

        private void OnBecameInvisible()
        {
            this.isVisible = false;
        }
    }
}
