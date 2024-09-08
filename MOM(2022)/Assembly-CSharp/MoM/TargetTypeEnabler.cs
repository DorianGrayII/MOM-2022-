namespace MOM
{
    using DBDef;
    using System;
    using UnityEngine;

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

