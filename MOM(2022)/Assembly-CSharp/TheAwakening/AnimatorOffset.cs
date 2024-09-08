namespace TheAwakening
{
    using System;
    using UnityEngine;

    public class AnimatorOffset : MonoBehaviour
    {
        public float offsetA;
        public float offsetB;

        private void OnEnable()
        {
            Animator componentInChildren = base.GetComponentInChildren<Animator>();
            float normalizedTime = UnityEngine.Random.Range(this.offsetA, this.offsetB);
            if (componentInChildren != null)
            {
                try
                {
                    componentInChildren.Play("Idle", -1, normalizedTime);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("object " + base.gameObject.name + " does not have animation Idle " + exception.ToString());
                }
            }
        }
    }
}

