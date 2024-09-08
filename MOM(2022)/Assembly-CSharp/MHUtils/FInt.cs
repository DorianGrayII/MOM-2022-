namespace MHUtils
{
    using ProtoBuf;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential), ProtoContract]
    public struct FInt : IComparable
    {
        [ProtoMember(1)]
        public int storage;
        public static FInt ZERO;
        public static FInt ONE;
        public static FInt N_ONE;
        public static FInt MAX;
        private const int SCALE = 100;
        public FInt(int iValue)
        {
            if (0x7fffffff == iValue)
            {
                this.storage = 0x7fffffff;
            }
            else
            {
                this.storage = iValue * 100;
            }
        }

        public FInt(float fValue)
        {
            this.storage = Mathf.RoundToInt(fValue * 100f);
        }

        public static explicit operator FInt(int iValue)
        {
            return new FInt(iValue);
        }

        public static explicit operator FInt(float fValue)
        {
            return new FInt(fValue);
        }

        public int ToInt()
        {
            return (this.storage / 100);
        }

        public float ToFloat()
        {
            return (((float) this.storage) / 100f);
        }

        public int ToIntX100()
        {
            return this.storage;
        }

        public FInt ReturnRounded()
        {
            int num = 50;
            return new FInt((this.storage + num) / 100);
        }

        public FInt ReturnRoundedFloor()
        {
            return new FInt(this.ToInt());
        }

        public FInt ReturnRoundedCeil()
        {
            return new FInt(this.ToFloat() + 0.5f);
        }

        public static FInt operator *(FInt a, FInt b)
        {
            return new FInt(a.ToFloat() * b.ToFloat());
        }

        public static FInt operator *(FInt a, int b)
        {
            return new FInt(a.ToFloat() * b);
        }

        public static FInt operator *(int a, FInt b)
        {
            return new FInt(a * b.ToFloat());
        }

        public static FInt operator *(FInt a, float b)
        {
            return new FInt(a.ToFloat() * b);
        }

        public static FInt operator *(float a, FInt b)
        {
            return new FInt(a * b.ToFloat());
        }

        public static FInt operator /(FInt a, FInt b)
        {
            return new FInt(a.ToFloat() / b.ToFloat());
        }

        public static FInt operator /(FInt a, int b)
        {
            return new FInt(a.ToFloat() / ((float) b));
        }

        public static FInt operator /(int a, FInt b)
        {
            return new FInt(((float) a) / b.ToFloat());
        }

        public static FInt operator /(FInt a, float b)
        {
            return new FInt(a.ToFloat() / b);
        }

        public static FInt operator /(float a, FInt b)
        {
            return new FInt(a / b.ToFloat());
        }

        public static FInt operator %(FInt a, FInt b)
        {
            return new FInt(a.ToFloat() % b.ToFloat());
        }

        public static FInt operator %(FInt a, int b)
        {
            return new FInt(a.ToFloat() % ((float) b));
        }

        public static FInt operator %(int a, FInt b)
        {
            return new FInt(((float) a) % b.ToFloat());
        }

        public static FInt operator +(FInt a, FInt b)
        {
            return new FInt(a.ToFloat() + b.ToFloat());
        }

        public static FInt operator +(FInt a, int b)
        {
            return new FInt(a.ToFloat() + b);
        }

        public static FInt operator +(int a, FInt b)
        {
            return new FInt(a + b.ToFloat());
        }

        public static FInt operator +(FInt a, float b)
        {
            return new FInt(a.ToFloat() + b);
        }

        public static FInt operator +(float a, FInt b)
        {
            return new FInt(a + b.ToFloat());
        }

        public static FInt operator -(FInt a, FInt b)
        {
            return new FInt(a.ToFloat() - b.ToFloat());
        }

        public static FInt operator -(FInt a, int b)
        {
            return new FInt(a.ToFloat() - b);
        }

        public static FInt operator -(int a, FInt b)
        {
            return new FInt(a - b.ToFloat());
        }

        public static FInt operator -(FInt a, float b)
        {
            return new FInt(a.ToFloat() - b);
        }

        public static FInt operator -(float a, FInt b)
        {
            return new FInt(a - b.ToFloat());
        }

        public static bool operator ==(FInt a, FInt b)
        {
            return (a.storage == b.storage);
        }

        public static bool operator ==(FInt a, int b)
        {
            return (a.storage == (b * 100));
        }

        public static bool operator ==(int a, FInt b)
        {
            return ((a * 100) == b.storage);
        }

        public static bool operator ==(FInt a, float b)
        {
            return (a.storage == new FInt(b).storage);
        }

        public static bool operator ==(float a, FInt b)
        {
            return (new FInt(a).storage == b.storage);
        }

        public static bool operator !=(FInt a, FInt b)
        {
            return (a.storage != b.storage);
        }

        public static bool operator !=(FInt a, int b)
        {
            return (a.storage != (b * 100));
        }

        public static bool operator !=(int a, FInt b)
        {
            return ((a * 100) != b.storage);
        }

        public static bool operator !=(FInt a, float b)
        {
            return (a.storage != new FInt(b).storage);
        }

        public static bool operator !=(float a, FInt b)
        {
            return (new FInt(a).storage != b.storage);
        }

        public static bool operator >=(FInt a, FInt b)
        {
            return (a.storage >= b.storage);
        }

        public static bool operator >=(FInt a, int b)
        {
            return (a.storage >= (b * 100));
        }

        public static bool operator >=(int a, FInt b)
        {
            return ((a * 100) >= b.storage);
        }

        public static bool operator >=(FInt a, float b)
        {
            return (a.storage >= new FInt(b).storage);
        }

        public static bool operator >=(float a, FInt b)
        {
            return (new FInt(a).storage >= b.storage);
        }

        public static bool operator <=(FInt a, FInt b)
        {
            return (a.storage <= b.storage);
        }

        public static bool operator <=(FInt a, int b)
        {
            return (a.storage <= (b * 100));
        }

        public static bool operator <=(int a, FInt b)
        {
            return ((a * 100) <= b.storage);
        }

        public static bool operator <=(FInt a, float b)
        {
            return (a.storage <= new FInt(b).storage);
        }

        public static bool operator <=(float a, FInt b)
        {
            return (new FInt(a).storage <= b.storage);
        }

        public static bool operator >(FInt a, FInt b)
        {
            return (a.storage > b.storage);
        }

        public static bool operator >(FInt a, int b)
        {
            return (a.storage > (b * 100));
        }

        public static bool operator >(int a, FInt b)
        {
            return ((a * 100) > b.storage);
        }

        public static bool operator >(FInt a, float b)
        {
            return (a.storage > new FInt(b).storage);
        }

        public static bool operator >(float a, FInt b)
        {
            return (new FInt(a).storage > b.storage);
        }

        public static bool operator <(FInt a, FInt b)
        {
            return (a.storage < b.storage);
        }

        public static bool operator <(FInt a, int b)
        {
            return (a.storage < (b * 100));
        }

        public static bool operator <(int a, FInt b)
        {
            return ((a * 100) < b.storage);
        }

        public static bool operator <(FInt a, float b)
        {
            return (a.storage < new FInt(b).storage);
        }

        public static bool operator <(float a, FInt b)
        {
            return (new FInt(a).storage < b.storage);
        }

        public override bool Equals(object obj)
        {
            return (!(obj is FInt) ? (!(obj is int) ? ((obj is float) && (this.storage == new FInt((float) obj))) : (this.storage == (((int) obj) * 100))) : (((FInt) obj).storage == this.storage));
        }

        public override int GetHashCode()
        {
            return this.storage.GetHashCode();
        }

        public string ToStringTMesh()
        {
            return ((this.storage >= 0) ? ((this.storage / 100).ToString() + "<#FFFFFF3F><size=70%>." + (this.storage % 100).ToString("00") + "</size></color>") : ((this.storage / 100).ToString() + "<#FFFFFF3F><size=70%>." + (-this.storage % 100).ToString("00") + "</size></color>"));
        }

        public override string ToString()
        {
            return this.ToString(2);
        }

        public string ToStringTryInt()
        {
            return ((((this.storage / 100) * 100) != this.storage) ? this.ToString(2) : (this.storage / 100).ToString());
        }

        public string ToString(int decimalPlaces)
        {
            return ((decimalPlaces < 2) ? ((decimalPlaces != 1) ? (this.storage / 100).ToString() : ((this.storage >= 0) ? ((this.storage / 100).ToString() + "." + ((this.storage / 10) % 10).ToString("0")) : ((this.storage >= 100) ? ((this.storage / 100).ToString() + "." + ((-this.storage / 10) % 10).ToString("0")) : ("-" + (this.storage / 100).ToString() + "." + ((-this.storage / 10) % 10).ToString("0"))))) : ((this.storage >= 0) ? ((this.storage / 100).ToString() + "." + (this.storage % 100).ToString("00")) : ((this.storage >= 100) ? ((this.storage / 100).ToString() + "." + (-this.storage % 100).ToString("00")) : ("-" + (this.storage / 100).ToString() + "." + (-this.storage % 100).ToString("00")))));
        }

        public int CompareTo(FInt fInt)
        {
            return this.storage.CompareTo(fInt.storage);
        }

        public int CompareTo(object obj)
        {
            return (!(obj is FInt) ? -1 : this.storage.CompareTo(((FInt) obj).storage));
        }

        public void CutToInt(bool onlyIfZero)
        {
            if (!onlyIfZero || ((this.storage < 100) && (this.storage > -100)))
            {
                int num = this.storage % 100;
                this.storage -= num;
            }
        }

        public static FInt Min(FInt a, FInt b)
        {
            return ((a.storage >= b.storage) ? b : a);
        }

        public static FInt Min(FInt a, float b)
        {
            return ((a.storage >= b) ? new FInt(b) : a);
        }

        public static FInt Min(float a, FInt b)
        {
            return ((a >= b.storage) ? b : new FInt(a));
        }

        public static FInt Max(FInt a, FInt b)
        {
            return ((a.storage <= b.storage) ? b : a);
        }

        public static FInt Max(FInt a, float b)
        {
            return ((a.storage <= (b * 100f)) ? new FInt(b) : a);
        }

        public static FInt Max(float a, FInt b)
        {
            return (((a * 100f) <= b.storage) ? b : new FInt(a));
        }

        public static FInt Lerp(FInt from, FInt to, float progress)
        {
            float num = from.ToFloat();
            return new FInt(num + ((to.ToFloat() - num) * progress));
        }

        public static int Sort(FInt a, FInt b)
        {
            return a.storage.CompareTo(b.storage);
        }

        public static int SortNeg(FInt a, FInt b)
        {
            return -a.storage.CompareTo(b.storage);
        }

        static FInt()
        {
            ZERO = new FInt(0);
            ONE = new FInt(1);
            N_ONE = new FInt(-1);
            MAX = new FInt(0x7fffffff);
        }
    }
}

