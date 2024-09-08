using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class ProtoRef<T> where T : ProtoLibraryItem
    {
        [ProtoMember(1)]
        public int index;

        [ProtoIgnore]
        private T reference;

        public static implicit operator ProtoRef<T>(T d)
        {
            return new ProtoRef<T>
            {
                reference = d,
                index = ProtoLibrary.Add(d)
            };
        }

        public static explicit operator T(ProtoRef<T> d)
        {
            if (d != null)
            {
                return d.Get();
            }
            return null;
        }

        public T Get()
        {
            if (this.reference == null)
            {
                this.reference = ProtoLibrary.Get(this.index) as T;
            }
            return this.reference;
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public int GetIndex()
        {
            return this.index;
        }
    }
}
