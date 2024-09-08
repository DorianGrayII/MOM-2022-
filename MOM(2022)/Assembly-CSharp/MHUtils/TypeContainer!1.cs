namespace MHUtils
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class TypeContainer<T0>
    {
        [ProtoMember(1)]
        public T0 t0;

        public TypeContainer()
        {
        }

        public TypeContainer(T0 t0)
        {
            this.t0 = t0;
        }

        public override string ToString()
        {
            T0 local = this.t0;
            return ("TypeContainer: " + local?.ToString());
        }
    }
}

