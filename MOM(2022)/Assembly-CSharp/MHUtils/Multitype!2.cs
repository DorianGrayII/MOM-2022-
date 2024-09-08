namespace MHUtils
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class Multitype<T0, T1>
    {
        [ProtoMember(1)]
        public T0 t0;
        [ProtoMember(2)]
        public T1 t1;

        public Multitype()
        {
        }

        public Multitype(T0 t0, T1 t1)
        {
            this.t0 = t0;
            this.t1 = t1;
        }

        public override string ToString()
        {
            T0 local = this.t0;
            T1 local2 = this.t1;
            return ("Multitype: " + local?.ToString() + " " + local2?.ToString());
        }
    }
}

