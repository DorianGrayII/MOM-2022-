using UnityEngine;

namespace MOM
{
    public class FortressRefs : MonoBehaviour
    {
        public delegate void Listener(AnimatorTrigger source);

        public Transform location;

        public GameObject projectileChaos;

        public GameObject projectileNature;

        public GameObject projectileLife;

        public GameObject projectileDeath;

        public GameObject projectileSorcery;

        public GameObject onHitEffectChaos;

        public GameObject onHitEffectNature;

        public GameObject onHitEffectLife;

        public GameObject onHitEffectDeath;

        public GameObject onHitEffectSorcery;
    }
}
