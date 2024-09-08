using DBDef;
using UnityEngine;

namespace MOM
{
    public class TargetTypeEnabler : MonoBehaviour
    {
        [SerializeField]
        private GameObjectEnabler<ETargetType> objects;

        public void Set(ETargetType targetType)
        {
            this.objects.Set(targetType);
        }
    }
}
