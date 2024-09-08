using System.Collections.Generic;
using ProtoBuf;

namespace MHUtils
{
    [ProtoContract]
    public class NetDictionary<K, V>
    {
        [ProtoMember(1)]
        private List<Multitype<K, V>> content;

        private bool dirty;

        private Dictionary<K, V> localDict;

        public V this[K key]
        {
            get
            {
                this.Initialize();
                if (!this.localDict.ContainsKey(key))
                {
                    return default(V);
                }
                return this.localDict[key];
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

        [ProtoBeforeSerialization]
        public void BeforeSerialization()
        {
            if (!this.dirty)
            {
                return;
            }
            if (this.localDict == null)
            {
                this.content = null;
                return;
            }
            this.content = new List<Multitype<K, V>>(this.localDict.Count);
            foreach (KeyValuePair<K, V> item in this.localDict)
            {
                this.content.Add(new Multitype<K, V>(item.Key, item.Value));
            }
        }

        [ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
            if (this.content == null)
            {
                this.localDict = null;
                return;
            }
            this.localDict = new Dictionary<K, V>();
            foreach (Multitype<K, V> item in this.content)
            {
                this.localDict[item.t0] = item.t1;
            }
        }

        public NetDictionary<K, V> Clone()
        {
            return Serializer.DeepClone(this);
        }

        private void Initialize()
        {
            if (this.localDict == null)
            {
                this.localDict = new Dictionary<K, V>();
            }
        }

        public Dictionary<K, V> GetDict()
        {
            this.Initialize();
            return this.localDict;
        }

        public bool ContainsKey(K key)
        {
            this.Initialize();
            return this.localDict.ContainsKey(key);
        }

        public void Remove(K key)
        {
            this.Initialize();
            this.localDict.Remove(key);
            this.dirty = true;
        }

        public void Clear()
        {
            this.Initialize();
            this.localDict.Clear();
            this.dirty = true;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            this.Initialize();
            return this.localDict.GetEnumerator();
        }

        public Dictionary<K, V> GetAsDictionary()
        {
            return this.localDict;
        }
    }
}
