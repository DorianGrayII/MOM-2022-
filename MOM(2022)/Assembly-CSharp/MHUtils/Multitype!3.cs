namespace MHUtils
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class Multitype<T0, T1, T2>
    {
        [ProtoMember(1)]
        public T0 t0;
        [ProtoMember(2)]
        public T1 t1;
        [ProtoMember(3)]
        public T2 t2;

        public Multitype()
        {
        }

        public Multitype(T0 t0, T1 t1, T2 t2)
        {
            this.t0 = t0;
            this.t1 = t1;
            this.t2 = t2;
        }

        public override string ToString()
        {
            string[] textArray1 = new string[6];
            textArray1[0] = "Multitype: ";
            textArray1[1] = this.t0?.ToString();
            string[] local1 = textArray1;
            local1[2] = " ";
            local1[3] = this.t1?.ToString();
            string[] local4 = local1;
            local4[4] = " ";
            local4[5] = this.t2?.ToString();
            return string.Concat(local4);
        }
    }
}

