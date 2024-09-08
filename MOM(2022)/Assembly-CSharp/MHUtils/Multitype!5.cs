namespace MHUtils
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class Multitype<T0, T1, T2, T3, T4>
    {
        [ProtoMember(1)]
        public T0 t0;
        [ProtoMember(2)]
        public T1 t1;
        [ProtoMember(3)]
        public T2 t2;
        [ProtoMember(4)]
        public T3 t3;
        [ProtoMember(5)]
        public T4 t4;

        public Multitype()
        {
        }

        public Multitype(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            this.t0 = t0;
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            this.t4 = t4;
        }

        public override string ToString()
        {
            string[] textArray1 = new string[10];
            textArray1[0] = "Multitype: ";
            textArray1[1] = this.t0?.ToString();
            string[] local1 = textArray1;
            local1[2] = " ";
            local1[3] = this.t1?.ToString();
            string[] local6 = local1;
            local6[4] = " ";
            local6[5] = this.t2?.ToString();
            string[] local7 = local6;
            local7[6] = " ";
            local7[7] = this.t3?.ToString();
            string[] local8 = local7;
            local8[8] = " ";
            local8[9] = this.t4?.ToString();
            return string.Concat(local8);
        }
    }
}

