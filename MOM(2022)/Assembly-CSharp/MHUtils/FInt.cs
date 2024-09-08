using System;
using ProtoBuf;
using UnityEngine;

namespace MHUtils
{
    [ProtoContract]
    public struct FInt : IComparable
    {
        [ProtoMember(1)]
        public int storage;

        public static FInt ZERO = new FInt(0);

        public static FInt ONE = new FInt(1);

        public static FInt N_ONE = new FInt(-1);

        public static FInt MAX = new FInt(int.MaxValue);

        private const int SCALE = 100;

        public FInt(int iValue)
        {
            if (int.MaxValue == iValue)
            {
                this.storage = int.MaxValue;
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
            return this.storage / 100;
        }

        public float ToFloat()
        {
            return (float)this.storage / 100f;
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
            return new FInt(a.ToFloat() * (float)b);
        }

        public static FInt operator *(int a, FInt b)
        {
            return new FInt((float)a * b.ToFloat());
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
            return new FInt(a.ToFloat() / (float)b);
        }

        public static FInt operator /(int a, FInt b)
        {
            return new FInt((float)a / b.ToFloat());
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
            return new FInt(a.ToFloat() % (float)b);
        }

        public static FInt operator %(int a, FInt b)
        {
            return new FInt((float)a % b.ToFloat());
        }

        public static FInt operator +(FInt a, FInt b)
        {
            return new FInt(a.ToFloat() + b.ToFloat());
        }

        public static FInt operator +(FInt a, int b)
        {
            return new FInt(a.ToFloat() + (float)b);
        }

        public static FInt operator +(int a, FInt b)
        {
            return new FInt((float)a + b.ToFloat());
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
            return new FInt(a.ToFloat() - (float)b);
        }

        public static FInt operator -(int a, FInt b)
        {
            return new FInt((float)a - b.ToFloat());
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
            return a.storage == b.storage;
        }

        public static bool operator ==(FInt a, int b)
        {
            return a.storage == b * 100;
        }

        public static bool operator ==(int a, FInt b)
        {
            return a * 100 == b.storage;
        }

        public static bool operator ==(FInt a, float b)
        {
            return a.storage == new FInt(b).storage;
        }

        public static bool operator ==(float a, FInt b)
        {
            return new FInt(a).storage == b.storage;
        }

        public static bool operator !=(FInt a, FInt b)
        {
            return a.storage != b.storage;
        }

        public static bool operator !=(FInt a, int b)
        {
            return a.storage != b * 100;
        }

        public static bool operator !=(int a, FInt b)
        {
            return a * 100 != b.storage;
        }

        public static bool operator !=(FInt a, float b)
        {
            return a.storage != new FInt(b).storage;
        }

        public static bool operator !=(float a, FInt b)
        {
            return new FInt(a).storage != b.storage;
        }

        public static bool operator >=(FInt a, FInt b)
        {
            return a.storage >= b.storage;
        }

        public static bool operator >=(FInt a, int b)
        {
            return a.storage >= b * 100;
        }

        public static bool operator >=(int a, FInt b)
        {
            return a * 100 >= b.storage;
        }

        public static bool operator >=(FInt a, float b)
        {
            return a.storage >= new FInt(b).storage;
        }

        public static bool operator >=(float a, FInt b)
        {
            return new FInt(a).storage >= b.storage;
        }

        public static bool operator <=(FInt a, FInt b)
        {
            return a.storage <= b.storage;
        }

        public static bool operator <=(FInt a, int b)
        {
            return a.storage <= b * 100;
        }

        public static bool operator <=(int a, FInt b)
        {
            return a * 100 <= b.storage;
        }

        public static bool operator <=(FInt a, float b)
        {
            return a.storage <= new FInt(b).storage;
        }

        public static bool operator <=(float a, FInt b)
        {
            return new FInt(a).storage <= b.storage;
        }

        public static bool operator >(FInt a, FInt b)
        {
            return a.storage > b.storage;
        }

        public static bool operator >(FInt a, int b)
        {
            return a.storage > b * 100;
        }

        public static bool operator >(int a, FInt b)
        {
            return a * 100 > b.storage;
        }

        public static bool operator >(FInt a, float b)
        {
            return a.storage > new FInt(b).storage;
        }

        public static bool operator >(float a, FInt b)
        {
            return new FInt(a).storage > b.storage;
        }

        public static bool operator <(FInt a, FInt b)
        {
            return a.storage < b.storage;
        }

        public static bool operator <(FInt a, int b)
        {
            return a.storage < b * 100;
        }

        public static bool operator <(int a, FInt b)
        {
            return a * 100 < b.storage;
        }

        public static bool operator <(FInt a, float b)
        {
            return a.storage < new FInt(b).storage;
        }

        public static bool operator <(float a, FInt b)
        {
            return new FInt(a).storage < b.storage;
        }

        public override bool Equals(object obj)
        {
            if (obj is FInt)
            {
                return ((FInt)obj).storage == this.storage;
            }
            if (obj is int)
            {
                return this.storage == (int)obj * 100;
            }
            if (obj is float)
            {
                return this.storage == new FInt((float)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.storage.GetHashCode();
        }

        public string ToStringTMesh()
        {
            if (this.storage < 0)
            {
                return this.storage / 100 + "<#FFFFFF3F><size=70%>." + (-this.storage % 100).ToString("00") + "</size></color>";
            }
            return this.storage / 100 + "<#FFFFFF3F><size=70%>." + (this.storage % 100).ToString("00") + "</size></color>";
        }

        public override string ToString()
        {
            return this.ToString(2);
        }

        public string ToStringTryInt()
        {
            if (this.storage / 100 * 100 == this.storage)
            {
                return (this.storage / 100).ToString();
            }
            return this.ToString(2);
        }

        public string ToString(int decimalPlaces)
        {
            if (decimalPlaces >= 2)
            {
                if (this.storage < 0)
                {
                    if (this.storage < 100)
                    {
                        return "-" + this.storage / 100 + "." + (-this.storage % 100).ToString("00");
                    }
                    return this.storage / 100 + "." + (-this.storage % 100).ToString("00");
                }
                return this.storage / 100 + "." + (this.storage % 100).ToString("00");
            }
            if (decimalPlaces == 1)
            {
                if (this.storage < 0)
                {
                    if (this.storage < 100)
                    {
                        return "-" + this.storage / 100 + "." + (-this.storage / 10 % 10).ToString("0");
                    }
                    return this.storage / 100 + "." + (-this.storage / 10 % 10).ToString("0");
                }
                return this.storage / 100 + "." + (this.storage / 10 % 10).ToString("0");
            }
            return (this.storage / 100).ToString();
        }

        public int CompareTo(FInt fInt)
        {
            return this.storage.CompareTo(fInt.storage);
        }

        public int CompareTo(object obj)
        {
            if (obj is FInt)
            {
                return this.storage.CompareTo(((FInt)obj).storage);
            }
            return -1;
        }

        public void CutToInt(bool onlyIfZero = false)
        {
            if (!onlyIfZero || (this.storage < 100 && this.storage > -100))
            {
                int num = this.storage % 100;
                this.storage -= num;
            }
        }

        public static FInt Min(FInt a, FInt b)
        {
            if (a.storage < b.storage)
            {
                return a;
            }
            return b;
        }

        public static FInt Min(FInt a, float b)
        {
            if ((float)a.storage < b)
            {
                return a;
            }
            return new FInt(b);
        }

        public static FInt Min(float a, FInt b)
        {
            if (a < (float)b.storage)
            {
                return new FInt(a);
            }
            return b;
        }

        public static FInt Max(FInt a, FInt b)
        {
            if (a.storage > b.storage)
            {
                return a;
            }
            return b;
        }

        public static FInt Max(FInt a, float b)
        {
            if ((float)a.storage > b * 100f)
            {
                return a;
            }
            return new FInt(b);
        }

        public static FInt Max(float a, FInt b)
        {
            if (a * 100f > (float)b.storage)
            {
                return new FInt(a);
            }
            return b;
        }

        public static FInt Lerp(FInt from, FInt to, float progress)
        {
            float num = from.ToFloat();
            float num2 = to.ToFloat() - num;
            return new FInt(num + num2 * progress);
        }

        public static int Sort(FInt a, FInt b)
        {
            return a.storage.CompareTo(b.storage);
        }

        public static int SortNeg(FInt a, FInt b)
        {
            return -a.storage.CompareTo(b.storage);
        }
    }
}
