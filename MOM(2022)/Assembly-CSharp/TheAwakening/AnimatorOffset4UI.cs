using System;
using UnityEngine;

namespace TheAwakening
{
    public class AnimatorOffset4UI : MonoBehaviour
    {
        private static float offset;

        public string animName;

        private void OnEnable()
        {
            Animator componentInChildren = base.GetComponentInChildren<Animator>();
            AnimatorOffset4UI.offset += 0.7f;
            if (componentInChildren != null)
            {
                try
                {
                    componentInChildren.Play(this.animName, -1, AnimatorOffset4UI.offset);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("object " + base.gameObject.name + " does not have animation Idle " + ex.ToString());
                }
            }
        }
    }
}
