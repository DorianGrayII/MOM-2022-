using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MHUtils
{
    public class MHEventSystem
    {
        private static Dictionary<string, HoneyEvent> eventCollection = new Dictionary<string, HoneyEvent>();

        private static Dictionary<EventFunction, List<string>> registrationRecord = new Dictionary<EventFunction, List<string>>();

        private static Dictionary<object, List<EventFunction>> objectSpanEventControl = new Dictionary<object, List<EventFunction>>();

        private static Dictionary<object, List<Multitype<string, object, object>>> buckets = new Dictionary<object, List<Multitype<string, object, object>>>();

        public static void RegisterEventBucket(Enum eventName, object owner, Callback whenUpdate = null)
        {
            MHEventSystem.RegisterEventBucket(eventName.ToString(), owner, whenUpdate);
        }

        public static void RegisterEventBucket<T>(object owner, Callback whenUpdate = null)
        {
            MHEventSystem.RegisterEventBucket(typeof(T).ToString(), owner, whenUpdate);
        }

        public static void RegisterEventBucket(string eventName, object owner, Callback whenUpdate = null)
        {
            if (!MHEventSystem.buckets.ContainsKey(owner))
            {
                MHEventSystem.buckets[owner] = new List<Multitype<string, object, object>>();
            }
            MHEventSystem.RegisterListener(eventName, delegate(object sender, object e)
            {
                if (MHEventSystem.buckets.ContainsKey(owner))
                {
                    Multitype<string, object, object> multitype = new Multitype<string, object, object>(eventName, sender, e);
                    MHEventSystem.buckets[owner].Add(multitype);
                    if (whenUpdate != null)
                    {
                        whenUpdate(multitype);
                    }
                }
            }, owner);
        }

        public static void RegisterListener(Enum eventName, EventFunction handler, object owner)
        {
            MHEventSystem.RegisterListener(eventName.ToString(), handler, owner);
        }

        public static void RegisterListener<T>(EventFunction handler, object owner)
        {
            MHEventSystem.RegisterListener(typeof(T).ToString(), handler, owner);
        }

        public static void RegisterListener(object obj, EventFunction handler, object owner)
        {
            if (obj == null)
            {
                Debug.LogError("RegisterListener with NULL object");
            }
            else
            {
                MHEventSystem.RegisterListener(obj.GetType().ToString(), handler, owner);
            }
        }

        public static void RegisterListener(string eventName, EventFunction handler, object owner)
        {
            if (!MHEventSystem.eventCollection.ContainsKey(eventName))
            {
                MHEventSystem.eventCollection[eventName] = new HoneyEvent();
            }
            MHEventSystem.eventCollection[eventName].handler += handler;
            if (!MHEventSystem.registrationRecord.ContainsKey(handler))
            {
                MHEventSystem.registrationRecord[handler] = new List<string>();
            }
            MHEventSystem.registrationRecord[handler].Add(eventName);
            if (owner != null)
            {
                MHEventSystem.objectSpanEventControl.TryGetValue(owner, out var value);
                if (value == null)
                {
                    value = new List<EventFunction>();
                    MHEventSystem.objectSpanEventControl.Add(owner, value);
                }
                value.Add(handler);
            }
        }

        public static bool HandleBucket(Enum eventName, EventFunction handler, object owner)
        {
            return MHEventSystem.HandleBucket(eventName.ToString(), handler, owner);
        }

        public static bool HandleBucket<T>(EventFunction handler, object owner)
        {
            return MHEventSystem.HandleBucket(typeof(T).ToString(), handler, owner);
        }

        public static bool HandleBucket(string eventName, EventFunction handler, object owner)
        {
            if (MHEventSystem.buckets.ContainsKey(owner) && handler != null)
            {
                Multitype<string, object, object> multitype = MHEventSystem.buckets[owner].Find((Multitype<string, object, object> o) => o.t0 == eventName);
                if (multitype != null)
                {
                    MHEventSystem.buckets[owner].Remove(multitype);
                    handler(multitype.t1, multitype.t2);
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveInBucket(Multitype<string, object, object> instance, object owner)
        {
            if (MHEventSystem.buckets.ContainsKey(owner) && MHEventSystem.buckets[owner].Contains(instance))
            {
                MHEventSystem.buckets[owner].Remove(instance);
            }
            return false;
        }

        public static List<Multitype<string, object, object>> PeekOnBucket<T>(object owner)
        {
            return MHEventSystem.PeekOnBucket(typeof(T).ToString(), owner);
        }

        public static List<Multitype<string, object, object>> PeekOnBucket(string eventName, object owner)
        {
            if (MHEventSystem.buckets.ContainsKey(owner))
            {
                return MHEventSystem.buckets[owner].FindAll((Multitype<string, object, object> o) => o.t0 == eventName);
            }
            return null;
        }

        public static void UnRegisterListener(string eventName, EventFunction handler)
        {
            if (MHEventSystem.eventCollection.ContainsKey(eventName))
            {
                MHEventSystem.eventCollection[eventName].handler -= handler;
            }
            if (MHEventSystem.registrationRecord.ContainsKey(handler))
            {
                int num = MHEventSystem.registrationRecord[handler].IndexOf(eventName);
                if (num > -1)
                {
                    MHEventSystem.registrationRecord[handler].RemoveAt(num);
                }
            }
        }

        public static void UnRegisterListener(EventFunction handler)
        {
            if (!MHEventSystem.registrationRecord.ContainsKey(handler))
            {
                return;
            }
            foreach (string item in MHEventSystem.registrationRecord[handler])
            {
                if (MHEventSystem.eventCollection.ContainsKey(item))
                {
                    MHEventSystem.eventCollection[item].handler -= handler;
                }
            }
            MHEventSystem.registrationRecord.Remove(handler);
        }

        public static void UnRegisterListenersLinkedToObject(object owner)
        {
            if (owner == null)
            {
                return;
            }
            MHEventSystem.objectSpanEventControl.TryGetValue(owner, out var value);
            if (value == null)
            {
                return;
            }
            foreach (EventFunction item in value)
            {
                MHEventSystem.UnRegisterListener(item);
            }
            if (MHEventSystem.buckets.ContainsKey(owner))
            {
                MHEventSystem.buckets.Remove(owner);
            }
            MHEventSystem.objectSpanEventControl.Remove(owner);
        }

        public static void TriggerEvent(string eventName, object sender, object args)
        {
            if (MHEventSystem.eventCollection.ContainsKey(eventName))
            {
                MHEventSystem.eventCollection[eventName].TriggerEvent(sender, args);
            }
        }

        public static void TriggerEvent(Enum eventName, object sender, object args)
        {
            MHEventSystem.TriggerEvent(eventName.ToString(), sender, args);
        }

        public static void TriggerEvent<T>(object sender, object args)
        {
            MHEventSystem.TriggerEvent(typeof(T).ToString(), sender, args);
        }

        public static void TriggerEvent(object sender, object args)
        {
            MHEventSystem.TriggerEvent(sender.GetType().ToString(), sender, args);
        }

        public static void PrintRegistered()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (MHEventSystem.objectSpanEventControl == null)
            {
                return;
            }
            foreach (KeyValuePair<object, List<EventFunction>> item in MHEventSystem.objectSpanEventControl)
            {
                stringBuilder.AppendLine(item.Key.ToString());
            }
            Debug.LogWarning("Unregistered instances:  \n" + stringBuilder);
        }

        public static void ForceClear()
        {
            if (MHEventSystem.objectSpanEventControl == null || MHEventSystem.objectSpanEventControl.Count <= 0)
            {
                return;
            }
            foreach (object item in new List<object>(MHEventSystem.objectSpanEventControl.Keys))
            {
                MHEventSystem.UnRegisterListenersLinkedToObject(item);
            }
        }
    }
}
