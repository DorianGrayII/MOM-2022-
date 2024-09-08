namespace MHUtils
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [ProtoContract]
    public class NetDictionary<K, V>
    {
        [ProtoMember(1)]
        private List<Multitype<K, V>> content;
        private bool dirty;
        private Dictionary<K, V> localDict;

        [ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
            if (this.content == null)
            {
                this.localDict = null;
            }
            else
            {
                this.localDict = new Dictionary<K, V>();
                foreach (Multitype<K, V> multitype in this.content)
                {
                    this.localDict[multitype.t0] = multitype.t1;
                }
            }
        }

        [ProtoBeforeSerialization]
        public void BeforeSerialization()
        {
            if (this.dirty)
            {
                if (this.localDict == null)
                {
                    this.content = null;
                }
                else
                {
                    this.content = new List<Multitype<K, V>>(this.localDict.Count);
                    foreach (KeyValuePair<K, V> pair in this.localDict)
                    {
                        this.content.Add(new Multitype<K, V>(pair.Key, pair.Value));
                    }
                }
            }
        }

        public void Clear()
        {
            this.Initialize();
            this.localDict.Clear();
            this.dirty = true;
        }

        public NetDictionary<K, V> Clone()
        {
            return Serializer.DeepClone<NetDictionary<K, V>>((NetDictionary<K, V>) this);
        }

        public bool ContainsKey(K key)
        {
            this.Initialize();
            return this.localDict.ContainsKey(key);
        }

        public Dictionary<K, V> GetAsDictionary()
        {
            return this.localDict;
        }

        public Dictionary<K, V> GetDict()
        {
            this.Initialize();
            return this.localDict;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            this.Initialize();
            return this.localDict.GetEnumerator();
        }

        private void Initialize()
        {
            if (this.localDict == null)
                this.localDict = new Dictionary<K, V>();
        }

        public void Remove(K key)
        {
            this.Initialize();
            this.localDict.Remove(key);
            this.dirty = true;
        }

        public V this[K key]
        {
            get
            {
                this.Initialize();
                if (this.localDict.ContainsKey(key))
                {
                    return this.localDict[key];
                }
                return default(V);
            }
            set
            {
                this.Initialize();
                this.localDict[key] = value;
                this.dirty = true;
            }
        }

        public int Count
        {
            get
            {
                this.Initialize();
                return this.localDict.Count;
            }
        }
    }
}

