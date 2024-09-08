using System;
using ProtoBuf;

namespace MHUtils
{
	// Token: 0x02000506 RID: 1286
	[global::ProtoBuf.ProtoContract]
	public class Multitype<T0, T1, T2, T3, T4>
	{
		// Token: 0x06001A79 RID: 6777 RVA: 0x0007D450 File Offset: 0x0007B650
		public Multitype()
		{
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0007D458 File Offset: 0x0007B658
		public Multitype(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
		{
			this.t0 = t0;
			this.t1 = t1;
			this.t2 = t2;
			this.t3 = t3;
			this.t4 = t4;
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x0007D488 File Offset: 0x0007B688
		public override string ToString()
		{
			string[] array = new string[10];
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
			array[8] = " ";
			int num5 = 9;
			T4 t5 = this.t4;
			array[num5] = ((t5 != null) ? t5.ToString() : null);
			return string.Concat(array);
		}

		// Token: 0x04002357 RID: 9047
		[global::ProtoBuf.ProtoMember(1)]
		public T0 t0;

		// Token: 0x04002358 RID: 9048
		[global::ProtoBuf.ProtoMember(2)]
		public T1 t1;

		// Token: 0x04002359 RID: 9049
		[global::ProtoBuf.ProtoMember(3)]
		public T2 t2;

		// Token: 0x0400235A RID: 9050
		[global::ProtoBuf.ProtoMember(4)]
		public T3 t3;

		// Token: 0x0400235B RID: 9051
		[global::ProtoBuf.ProtoMember(5)]
		public T4 t4;
	}
}
