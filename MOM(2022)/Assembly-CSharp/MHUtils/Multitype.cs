using System;
using ProtoBuf;

namespace MHUtils
{
	// Token: 0x02000503 RID: 1283
	[global::ProtoBuf.ProtoContract]
	public class Multitype<T0, T1>
	{
		// Token: 0x06001A70 RID: 6768 RVA: 0x0007D22E File Offset: 0x0007B42E
		public Multitype()
		{
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0007D236 File Offset: 0x0007B436
		public Multitype(T0 t0, T1 t1)
		{
			this.t0 = t0;
			this.t1 = t1;
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x0007D24C File Offset: 0x0007B44C
		public override string ToString()
		{
			string str = "Multitype: ";
			T0 t = this.t0;
			string str2 = (t != null) ? t.ToString() : null;
			string str3 = " ";
			T1 t2 = this.t1;
			return str + str2 + str3 + ((t2 != null) ? t2.ToString() : null);
		}

		// Token: 0x0400234E RID: 9038
		[global::ProtoBuf.ProtoMember(1)]
		public T0 t0;

		// Token: 0x0400234F RID: 9039
		[global::ProtoBuf.ProtoMember(2)]
		public T1 t1;
	}
}
