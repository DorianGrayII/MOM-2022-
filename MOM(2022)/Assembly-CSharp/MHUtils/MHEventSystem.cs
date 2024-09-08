namespace MHUtils
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class MHEventSystem
    {
        private static Dictionary<string, HoneyEvent> eventCollection = new Dictionary<string, HoneyEvent>();
        private static Dictionary<EventFunction, List<string>> registrationRecord = new Dictionary<EventFunction, List<string>>();
        private static Dictionary<object, List<EventFunction>> objectSpanEventControl = new Dictionary<object, List<EventFunction>>();
        private static Dictionary<object, List<Multitype<string, object, object>>> buckets = new Dictionary<object, List<Multitype<string, object, object>>>();

        public static void ForceClear()
        {
            if ((objectSpanEventControl != null) && (objectSpanEventControl.Count > 0))
            {
                using (List<object>.Enumerator enumerator = new List<object>(objectSpanEventControl.Keys).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        UnRegisterListenersLinkedToObject(enumerator.Current);
                    }
                }
            }
        }

        public static bool HandleBucket<T>(EventFunction handler, object owner)
        {
            return HandleBucket(typeof(T).ToString(), handler, owner);
        }

        public static bool HandleBucket(Enum eventName, EventFunction handler, object owner)
        {
            return HandleBucket(eventName.ToString(), handler, owner);
        }

        public static bool HandleBucket(string eventName, EventFunction handler, object owner)
        {
            if (buckets.ContainsKey(owner) && (handler != null))
            {
                Multitype<string, object, object> item = buckets[owner].Find(o => o.t0 == eventName);
                if (item != null)
                {
                    buckets[owner].Remove(item);
                    handler(item.t1, item.t2);
                    return true;
                }
            }
            return false;
        }

        public static List<Multitype<string, object, object>> PeekOnBucket<T>(object owner)
        {
            return PeekOnBucket(typeof(T).ToString(), owner);
        }

        public static List<Multitype<string, object, object>> PeekOnBucket(string eventName, object owner)
        {
            return (!buckets.ContainsKey(owner) ? null : buckets[owner].FindAll(o => o.t0 == eventName));
        }

        public static void PrintRegistered()
        {
            StringBuilder builder = new StringBuilder();
            if (objectSpanEventControl != null)
            {
                string text1;
                foreach (KeyValuePair<object, List<EventFunction>> pair in objectSpanEventControl)
                {
                    builder.AppendLine(pair.Key.ToString());
                }
                if (builder != null)
                {
                    text1 = builder.ToString();
                }
                else
                {
                    StringBuilder local1 = builder;
                    text1 = null;
                }
                Debug.LogWarning("Unregistered instances:  \n" + text1);
            }
        }

        public static void RegisterEventBucket<T>(object owner, Callback whenUpdate)
        {
            RegisterEventBucket(typeof(T).ToString(), owner, whenUpdate);
        }

        public static void RegisterEventBucket(Enum eventName, object owner, Callback whenUpdate)
        {
            RegisterEventBucket(eventName.ToString(), owner, whenUpdate);
        }

        public static void RegisterEventBucket(string eventName, object owner, Callback whenUpdate)
        {
            if (!buckets.ContainsKey(owner))
            {
                buckets[owner] = new List<Multitype<string, object, object>>();
            }
            RegisterListener(eventName, delegate (object sender, object e) {
                if (buckets.ContainsKey(owner))
                {
                    Multitype<string, object, object> item = new Multitype<string, object, object>(eventName, sender, e);
                    buckets[owner].Add(item);
                    if (whenUpdate != null)
                    {
                        whenUpdate(item);
                    }
                }
            }, owner);
        }

        public static void RegisterListener<T>(EventFunction handler, object owner)
        {
            RegisterListener(typeof(T).ToString(), handler, owner);
        }

        public static void RegisterListener(Enum eventName, EventFunction handler, object owner)
        {
            RegisterListener(eventName.ToString(), handler, owner);
        }

        public static void RegisterListener(object obj, EventFunction handler, object owner)
        {
            if (obj == null)
            {
                Debug.LogError("RegisterListener with NULL object");
            }
            else
            {
                RegisterListener(obj.GetType().ToString(), handler, owner);
            }
        }

        public static void RegisterListener(string eventName, EventFunction handler, object owner)
        {
            if (!eventCollection.ContainsKey(eventName))
            {
                eventCollection[eventName] = new HoneyEvent();
            }
            eventCollection[eventName].handler += handler;
            if (!registrationRecord.ContainsKey(handler))
            {
                registrationRecord[handler] = new List<string>();
            }
            registrationRecord[handler].Add(eventName);
            if (owner != null)
            {
                List<EventFunction> list;
                objectSpanEventControl.TryGetValue(owner, out list);
                if (list == null)
                {
                    list = new List<EventFunction>();
                    objectSpanEventControl.Add(owner, list);
                }
                list.Add(handler);
            }
        }

        public static bool RemoveInBucket(Multitype<string, object, object> instance, object owner)
        {
            if (buckets.ContainsKey(owner) && buckets[owner].Contains(instance))
            {
                buckets[owner].Remove(instance);
            }
            return false;
        }

        public static void TriggerEvent(object sender, object args)
        {
            TriggerEvent(sender.GetType().ToString(), sender, args);
        }

        public static void TriggerEvent<T>(object sender, object args)
        {
            TriggerEvent(typeof(T).ToString(), sender, args);
        }

        public static void TriggerEvent(Enum eventName, object sender, object args)
        {
            TriggerEvent(eventName.ToString(), sender, args);
        }

        public static void TriggerEvent(string eventName, object sender, object args)
        {
            if (eventCollection.ContainsKey(eventName))
            {
                eventCollection[eventName].TriggerEvent(sender, args);
            }
        }

        public static void UnRegisterListener(EventFunction handler)
        {
            if (registrationRecord.ContainsKey(handler))
            {
                foreach (string str in registrationRecord[handler])
                {
                    if (eventCollection.ContainsKey(str))
                    {
                        eventCollection[str].handler -= handler;
                    }
                }
                registrationRecord.Remove(handler);
            }
        }

        public static void UnRegisterListener(string eventName, EventFunction handler)
        {
            if (eventCollection.ContainsKey(eventName))
            {
                eventCollection[eventName].handler -= handler;
            }
            if (registrationRecord.ContainsKey(handler))
            {
                int index = registrationRecord[handler].IndexOf(eventName);
                if (index > -1)
                {
                    registrationRecord[handler].RemoveAt(index);
                }
            }
        }

        public static void UnRegisterListenersLinkedToObject(object owner)
        {
            if (owner != null)
            {
                List<EventFunction> list;
                objectSpanEventControl.TryGetValue(owner, out list);
                if (list != null)
                {
                    using (List<EventFunction>.Enumerator enumerator = list.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            UnRegisterListener(enumerator.Current);
                        }
                    }
                    if (buckets.ContainsKey(owner))
                    {
                        buckets.Remove(owner);
                    }
                    objectSpanEventControl.Remove(owner);
                }
            }
        }
    }
}

