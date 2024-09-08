using System;
using ProtoBuf;

namespace MHUtils
{
	// Token: 0x02000505 RID: 1285
	[global::ProtoBuf.ProtoContract]
	public class Multitype<T0, T1, T2, T3>
	{
		// Token: 0x06001A76 RID: 6774 RVA: 0x0007D362 File Offset: 0x0007B562
		public Multitype()
		{
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0007D36A File Offset: 0x0007B56A
		public Multitype(T0 t0, T1 t1, T2 t2, T3 t3)
		{
			this.t0 = t0;
			this.t1 = t1;
			this.t2 = t2;
			this.t3 = t3;
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0007D390 File Offset: 0x0007B590
		public override string ToString()
		{
			string[] array = new string[8];
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
			array[6] = " ";
			int num4 = 7;
			T3 t4 = this.t3;
			array[num4] = ((t4 != null) ? t4.ToString() : null);
			return string.Concat(array);
		}

		// Token: 0x04002353 RID: 9043
		[global::ProtoBuf.ProtoMember(1)]
		public T0 t0;

		// Token: 0x04002354 RID: 9044
		[global::ProtoBuf.ProtoMember(2)]
		public T1 t1;

		// Token: 0x04002355 RID: 9045
		[global::ProtoBuf.ProtoMember(3)]
		public T2 t2;

		// Token: 0x04002356 RID: 9046
		[global::ProtoBuf.ProtoMember(4)]
		public T3 t3;
	}
}
