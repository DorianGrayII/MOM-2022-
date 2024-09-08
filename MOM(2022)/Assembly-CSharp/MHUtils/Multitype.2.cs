using System;
using ProtoBuf;

namespace MHUtils
{
	// Token: 0x02000504 RID: 1284
	[global::ProtoBuf.ProtoContract]
	public class Multitype<T0, T1, T2>
	{
		// Token: 0x06001A73 RID: 6771 RVA: 0x0007D2A6 File Offset: 0x0007B4A6
		public Multitype()
		{
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x0007D2AE File Offset: 0x0007B4AE
		public Multitype(T0 t0, T1 t1, T2 t2)
		{
			this.t0 = t0;
			this.t1 = t1;
			this.t2 = t2;
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0007D2CC File Offset: 0x0007B4CC
		public override string ToString()
		{
			string[] array = new string[6];
			array[0] = "Multitype: ";
			int num = 1;
			T0 t = this.t0;
			array[num] = ((t != null) ? t.ToString() : null);
			array[2] = " ";
			int num2 = 3;
			T1 t2 = this.t1;
			array[num2] = ((t2 != null) ? t2.ToString() : null);
			array[4] = " ";
			int num3 = 5;
			T2 t3 = this.t2;
			array[num3] = ((t3 != null) ? t3.ToString() : null);
			return string.Concat(array);
		}

		// Token: 0x04002350 RID: 9040
		[global::ProtoBuf.ProtoMember(1)]
		public T0 t0;

		// Token: 0x04002351 RID: 9041
		[global::ProtoBuf.ProtoMember(2)]
		public T1 t1;

		// Token: 0x04002352 RID: 9042
		[global::ProtoBuf.ProtoMember(3)]
		public T2 t2;
	}
}
