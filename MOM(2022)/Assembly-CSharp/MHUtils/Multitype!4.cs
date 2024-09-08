namespace MHUtils
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class Multitype<T0, T1, T2, T3>
    {
        [ProtoMember(1)]
        public T0 t0;
        [ProtoMember(2)]
        public T1 t1;
        [ProtoMember(3)]
        public T2 t2;
        [ProtoMember(4)]
        public T3 t3;

        public Multitype()
        {
        }

        public Multitype(T0 t0, T1 t1, T2 t2, T3 t3)
        {
            this.t0 = t0;
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
        }

        public override string ToString()
        {
            string[] textArray1 = new string[8];
            textArray1[0] = "Multitype: ";
            textArray1[1] = this.t0?.ToString();
            string[] local1 = textArray1;
            local1[2] = " ";
            local1[3] = this.t1?.ToString();
            string[] local5 = local1;
            local5[4] = " ";
            local5[5] = this.t2?.ToString();
            string[] local6 = local5;
            local6[6] = " ";
            local6[7] = this.t3?.ToString();
            return string.Concat(local6);
        }
    }
}

