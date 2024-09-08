using System;
using ProtoBuf;

namespace MHUtils
{
	// Token: 0x02000502 RID: 1282
	[global::ProtoBuf.ProtoContract]
	public class TypeContainer<T0>
	{
		// Token: 0x06001A6D RID: 6765 RVA: 0x0007D1DF File Offset: 0x0007B3DF
		public TypeContainer()
		{
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x0007D1E7 File Offset: 0x0007B3E7
		public TypeContainer(T0 t0)
		{
			this.t0 = t0;
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0007D1F8 File Offset: 0x0007B3F8
		public override string ToString()
		{
			string str = "TypeContainer: ";
			T0 t = this.t0;
			return str + ((t != null) ? t.ToString() : null);
		}

		// Token: 0x0400234D RID: 9037
		[global::ProtoBuf.ProtoMember(1)]
		public T0 t0;
	}
}
