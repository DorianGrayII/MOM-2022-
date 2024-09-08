namespace MOM
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class FortressRefs : MonoBehaviour
    {
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

        public delegate void Listener(AnimatorTrigger source);
    }
}

