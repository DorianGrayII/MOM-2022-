using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public struct Array6
    {
        [ProtoMember(1)]
        private int index0;

        [ProtoMember(2)]
        private int index1;

        [ProtoMember(3)]
        private int index2;

        [ProtoMember(4)]
        private int index3;

        [ProtoMember(5)]
        private int index4;

        [ProtoMember(6)]
        private int index5;

        public int Length => 6;

        public int this[int i]
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
                case 3:
                    return this.index3;
                case 4:
                    return this.index4;
                case 5:
                    return this.index5;
                default:
                    Debug.LogError("Invalid index GET for Array6: " + i);
                    return -1;
                }
            }
            set
            {
                switch (i)
                {
                case 0:
                    this.index0 = value;
                    break;
                case 1:
                    this.index1 = value;
                    break;
                case 2:
                    this.index2 = value;
                    break;
                case 3:
                    this.index3 = value;
                    break;
                case 4:
                    this.index4 = value;
                    break;
                case 5:
                    this.index5 = value;
                    break;
                default:
                    Debug.LogError("Invalid index SET for Array6: " + i);
                    break;
                }
            }
        }

        public void ClearTo(int index)
        {
            this.index0 = index;
            this.index1 = index;
            this.index2 = index;
            this.index3 = index;
            this.index4 = index;
            this.index5 = index;
        }
    }
}
