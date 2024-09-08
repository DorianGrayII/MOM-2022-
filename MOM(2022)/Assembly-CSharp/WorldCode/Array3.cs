using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public class Array3<T> where T : class
    {
        [ProtoMember(1)]
        private T index0;

        [ProtoMember(2)]
        private T index1;

        [ProtoMember(3)]
        private T index2;

        [ProtoMember(4)]
        private int count;

        public int Length => 3;

        public T this[int i]
        {
            get
            {
                switch (i)
                {
                case 0:
                    return this.index0;
                case 1:
                    return this.index1;
                case 2:
                    return this.index2;
                default:
                    Debug.LogError("Invalid index GET for Array3: " + i);
                    return null;
                }
            }
            set
            {
                switch (i)
                {
                case 0:
                    this.index0 = value;
                    this.count = Mathf.Max(this.count, 1);
                    break;
                case 1:
                    this.index1 = value;
                    this.count = Mathf.Max(this.count, 2);
                    break;
                case 2:
                    this.index2 = value;
                    this.count = Mathf.Max(this.count, 3);
                    break;
                default:
                    Debug.LogError("Invalid index SET for Array3: " + i);
                    break;
                }
            }
        }

        public void Add(T value)
        {
            if (this.count == 3)
            {
                Debug.LogError("no space on the Array3");
            }
            else
            {
                this[this.count] = value;
            }
        }

        public bool Contains(T value)
        {
            for (int i = 0; i < this.Length; i++)
            {
                T val = this[i];
                if (val != null && val.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
