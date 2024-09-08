namespace TheAwakening
{
    using System;
    using UnityEngine;

    public class AnimatorOffset4UI : MonoBehaviour
    {
        private static float offset;
        public string animName;

        private void OnEnable()
        {
            Animator componentInChildren = base.GetComponentInChildren<Animator>();
            offset += 0.7f;
            if (componentInChildren != null)
            {
                try
                {
                    componentInChildren.Play(this.animName, -1, offset);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("object " + base.gameObject.name + " does not have animation Idle " + exception.ToString());
                }
            }
        }
    }
}

