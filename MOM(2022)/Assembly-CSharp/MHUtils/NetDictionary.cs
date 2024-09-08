using System;
using System.Collections.Generic;
using ProtoBuf;

namespace MHUtils
{
	// Token: 0x0200051F RID: 1311
	[global::ProtoBuf.ProtoContract]
	public class NetDictionary<K, V>
	{
		// Token: 0x06001B85 RID: 7045 RVA: 0x000820C0 File Offset: 0x000802C0
		[global::ProtoBuf.ProtoBeforeSerialization]
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
			this.content = new global::System.Collections.Generic.List<global::MHUtils.Multitype<K, V>>(this.localDict.Count);
			foreach (global::System.Collections.Generic.KeyValuePair<K, V> keyValuePair in this.localDict)
			{
				this.content.Add(new global::MHUtils.Multitype<K, V>(keyValuePair.Key, keyValuePair.Value));
			}
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x0008215C File Offset: 0x0008035C
		[global::ProtoBuf.ProtoAfterDeserialization]
		public void AfterDeserialization()
		{
			if (this.content == null)
			{
				this.localDict = null;
				return;
			}
			this.localDict = new global::System.Collections.Generic.Dictionary<K, V>();
			foreach (global::MHUtils.Multitype<K, V> multitype in this.content)
			{
				this.localDict[multitype.t0] = multitype.t1;
			}
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x000821DC File Offset: 0x000803DC
		public global::MHUtils.NetDictionary<K, V> Clone()
		{
			return global::ProtoBuf.Serializer.DeepClone<global::MHUtils.NetDictionary<K, V>>(this);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x000821E4 File Offset: 0x000803E4
		private void Initialize()
		{
			if (this.localDict == null)
			{
				this.localDict = new global::System.Collections.Generic.Dictionary<K, V>();
			}
		}

		// Token: 0x170000B6 RID: 182
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06001B8B RID: 7051 RVA: 0x0008224F File Offset: 0x0008044F
		public int Count
		{
			get
			{
				this.Initialize();
				return this.localDict.Count;
			}
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00082262 File Offset: 0x00080462
		public global::System.Collections.Generic.Dictionary<K, V> GetDict()
		{
			this.Initialize();
			return this.localDict;
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00082270 File Offset: 0x00080470
		public bool ContainsKey(K key)
		{
			this.Initialize();
			return this.localDict.ContainsKey(key);
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x00082284 File Offset: 0x00080484
		public void Remove(K key)
		{
			this.Initialize();
			this.localDict.Remove(key);
			this.dirty = true;
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000822A0 File Offset: 0x000804A0
		public void Clear()
		{
			this.Initialize();
			this.localDict.Clear();
			this.dirty = true;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x000822BA File Offset: 0x000804BA
		public global::System.Collections.Generic.IEnumerator<global::System.Collections.Generic.KeyValuePair<K, V>> GetEnumerator()
		{
			this.Initialize();
			return this.localDict.GetEnumerator();
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x000822D2 File Offset: 0x000804D2
		public global::System.Collections.Generic.Dictionary<K, V> GetAsDictionary()
		{
			return this.localDict;
		}

		// Token: 0x040023AE RID: 9134
		[global::ProtoBuf.ProtoMember(1)]
		private global::System.Collections.Generic.List<global::MHUtils.Multitype<K, V>> content;

		// Token: 0x040023AF RID: 9135
		private bool dirty;

		// Token: 0x040023B0 RID: 9136
		private global::System.Collections.Generic.Dictionary<K, V> localDict;
	}
}
