using DBDef;
using UnityEngine;

namespace MOM
{
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
