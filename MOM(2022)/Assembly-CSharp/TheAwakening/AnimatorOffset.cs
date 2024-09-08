using System;
using UnityEngine;

namespace TheAwakening
{
    public class AnimatorOffset : MonoBehaviour
    {
        public float offsetA;

        public float offsetB;

        private void OnEnable()
        {
            Animator componentInChildren = base.GetComponentInChildren<Animator>();
            float normalizedTime = global::UnityEngine.Random.Range(this.offsetA, this.offsetB);
            if (componentInChildren != null)
            {
                try
                {
                    componentInChildren.Play("Idle", -1, normalizedTime);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("object " + base.gameObject.name + " does not have animation Idle " + ex.ToString());
                }
            }
        }
    }
}
