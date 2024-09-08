namespace MOM
{
    using DBDef;
    using System;
    using UnityEngine;

    public class RealmEnabler : MonoBehaviour
    {
        [SerializeField]
        private GameObjectEnabler<ERealm> objects;

        public void Set(ERealm realm)
        {
            this.objects.Set(realm);
        }
    }
}

