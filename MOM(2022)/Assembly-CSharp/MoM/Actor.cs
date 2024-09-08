namespace MOM
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using WorldCode;

    public class Actor : MonoBehaviour
    {
        public bool isRendering;
        public WorldCode.Plane plane;
        public bool isDead;

        public virtual void DestroyObject()
        {
            this.isDead = true;
            Destroy(base.gameObject);
        }

        public virtual void IsRendering(bool value)
        {
            this.isRendering = value;
        }

        public void SetWorldHeightPosition(Vector3 position, bool allowUnderwater)
        {
            float heightAt = this.plane.GetHeightAt(position, allowUnderwater);
            position.y = heightAt;
            base.transform.localPosition = position;
        }
    }
}

