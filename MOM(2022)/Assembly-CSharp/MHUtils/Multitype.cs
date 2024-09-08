using ProtoBuf;

namespace MHUtils
{
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
            T0 val = this.t0;
            string obj = val?.ToString();
            T1 val2 = this.t1;
            return "Multitype: " + obj + " " + val2;
        }
    }
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
            string[] obj = new string[6] { "Multitype: ", null, null, null, null, null };
            T0 val = this.t0;
            obj[1] = val?.ToString();
            obj[2] = " ";
            T1 val2 = this.t1;
            obj[3] = val2?.ToString();
            obj[4] = " ";
            T2 val3 = this.t2;
            obj[5] = val3?.ToString();
            return string.Concat(obj);
        }
    }
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
            string[] obj = new string[8] { "Multitype: ", null, null, null, null, null, null, null };
            T0 val = this.t0;
            obj[1] = val?.ToString();
            obj[2] = " ";
            T1 val2 = this.t1;
            obj[3] = val2?.ToString();
            obj[4] = " ";
            T2 val3 = this.t2;
            obj[5] = val3?.ToString();
            obj[6] = " ";
            T3 val4 = this.t3;
            obj[7] = val4?.ToString();
            return string.Concat(obj);
        }
    }
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
            string[] obj = new string[10] { "Multitype: ", null, null, null, null, null, null, null, null, null };
            T0 val = this.t0;
            obj[1] = val?.ToString();
            obj[2] = " ";
            T1 val2 = this.t1;
            obj[3] = val2?.ToString();
            obj[4] = " ";
            T2 val3 = this.t2;
            obj[5] = val3?.ToString();
            obj[6] = " ";
            T3 val4 = this.t3;
            obj[7] = val4?.ToString();
            obj[8] = " ";
            T4 val5 = this.t4;
            obj[9] = val5?.ToString();
            return string.Concat(obj);
        }
    }
}
