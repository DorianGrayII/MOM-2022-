using UnityEngine;
using WorldCode;

namespace MOM
{
    public class Actor : MonoBehaviour
    {
        public bool isRendering;

        public global::WorldCode.Plane plane;

        public bool isDead;

        public void SetWorldHeightPosition(Vector3 position, bool allowUnderwater = true)
        {
            float heightAt = this.plane.GetHeightAt(position, allowUnderwater);
            position.y = heightAt;
            base.transform.localPosition = position;
        }

        public virtual void DestroyObject()
        {
            this.isDead = true;
            Object.Destroy(base.gameObject);
        }

        public virtual void IsRendering(bool value)
        {
            this.isRendering = value;
        }
    }
}
