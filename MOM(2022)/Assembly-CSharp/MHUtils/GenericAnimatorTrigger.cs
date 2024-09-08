namespace MHUtils
{
    using System;
    using UnityEngine;

    public class GenericAnimatorTrigger : MonoBehaviour
    {
        public string eventName;

        public void Trigger()
        {
            MHEventSystem.TriggerEvent<GenericAnimatorTrigger>(this, this.eventName);
        }
    }
}

