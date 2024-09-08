namespace MOM
{
    using DBDef;
    using System;
    using UnityEngine;

    public class RarityEnabler : MonoBehaviour
    {
        [SerializeField]
        private GameObjectEnabler<ERarity> objects;

        public void Set(ERarity rarity)
        {
            this.objects.Set(rarity);
        }
    }
}

