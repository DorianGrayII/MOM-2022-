using DBDef;
using UnityEngine;

namespace MOM
{
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
