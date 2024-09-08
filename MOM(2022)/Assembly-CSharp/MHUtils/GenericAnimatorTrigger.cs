using UnityEngine;

namespace MHUtils
{
    public class GenericAnimatorTrigger : MonoBehaviour
    {
        public string eventName;

        public void Trigger()
        {
            MHEventSystem.TriggerEvent<GenericAnimatorTrigger>(this, this.eventName);
        }
    }
}
