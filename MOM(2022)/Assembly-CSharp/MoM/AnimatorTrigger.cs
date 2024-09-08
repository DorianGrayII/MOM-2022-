namespace MOM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;

    public class AnimatorTrigger : MonoBehaviour
    {
        public Transform location;
        public GameObject instanceSource;
        public GameObject onHitEffect;
        public bool highLobProjectile;
        public bool lowLobProjectile;

        public event Listener onTrigger;

        public void AddListener(Listener listener)
        {
            this.onTrigger += listener;
        }

        public void RemoveAllListeners()
        {
            this.onTrigger = null;
        }

        public void RemoveListener(Listener listener)
        {
            this.onTrigger -= listener;
        }

        public void Trigger()
        {
            if (this.onTrigger == null)
            {
                Listener onTrigger = this.onTrigger;
            }
            else
            {
                this.onTrigger(this);
            }
        }

        public delegate void Listener(AnimatorTrigger source);
    }
}

