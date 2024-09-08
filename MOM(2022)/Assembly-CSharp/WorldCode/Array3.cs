using System;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
	// Token: 0x02000400 RID: 1024
	[global::ProtoBuf.ProtoContract]
	public class Array3<T> where T : class
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06001494 RID: 5268 RVA: 0x0005EBD0 File Offset: 0x0005CDD0
		public int Length
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000029 RID: 41
		public T this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return this.index0;
				case 1:
					return this.index1;
				case 2:
					return this.index2;
				default:
					global::UnityEngine.Debug.LogError("Invalid index GET for Array3: " + i.ToString());
					return default(T);
				}
			}
			set
			{
				switch (i)
				{
				case 0:
					this.index0 = value;
					this.count = global::UnityEngine.Mathf.Max(this.count, 1);
					return;
				case 1:
					this.index1 = value;
					this.count = global::UnityEngine.Mathf.Max(this.count, 2);
					return;
				case 2:
					this.index2 = value;
					this.count = global::UnityEngine.Mathf.Max(this.count, 3);
					return;
				default:
					global::UnityEngine.Debug.LogError("Invalid index SET for Array3: " + i.ToString());
					return;
				}
			}
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x0005ECB1 File Offset: 0x0005CEB1
		public void Add(T value)
		{
			if (this.count == 3)
			{
				global::UnityEngine.Debug.LogError("no space on the Array3");
				return;
			}
			this[this.count] = value;
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x0005ECD4 File Offset: 0x0005CED4
		public bool Contains(T value)
		{
			for (int i = 0; i < this.Length; i++)
			{
				T t = this[i];
				if (t != null && t.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001443 RID: 5187
		[global::ProtoBuf.ProtoMember(1)]
		private T index0;

		// Token: 0x04001444 RID: 5188
		[global::ProtoBuf.ProtoMember(2)]
		private T index1;

		// Token: 0x04001445 RID: 5189
		[global::ProtoBuf.ProtoMember(3)]
		private T index2;

		// Token: 0x04001446 RID: 5190
		[global::ProtoBuf.ProtoMember(4)]
		private int count;
	}
}
