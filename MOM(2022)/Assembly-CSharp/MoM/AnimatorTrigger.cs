using UnityEngine;

namespace MOM
{
    public class AnimatorTrigger : MonoBehaviour
    {
        public delegate void Listener(AnimatorTrigger source);

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

        public void RemoveListener(Listener listener)
        {
            this.onTrigger -= listener;
        }

        public void RemoveAllListeners()
        {
            this.onTrigger = null;
        }

        public void Trigger()
        {
            this.onTrigger?.Invoke(this);
        }
    }
}
