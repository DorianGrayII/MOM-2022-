namespace MOM
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class StatDetails
    {
        public Dictionary<string, FInt> breakdown = new Dictionary<string, FInt>();
        public bool intMode;

        public void Add(string id, FInt amount, bool allowZero)
        {
            if (allowZero || (amount != 0))
            {
                FInt zERO = FInt.ZERO;
                this.breakdown.TryGetValue(id, out zERO);
                FInt num2 = zERO + amount;
                if (allowZero || (num2 != 0))
                {
                    this.breakdown[id] = num2;
                }
                else
                {
                    this.breakdown.Remove(id);
                }
            }
        }

        public void Add(string id, int amount, bool allowZero)
        {
            this.intMode = true;
            this.Add(id, new FInt(amount), allowZero);
        }

        public void Clear()
        {
            this.breakdown.Clear();
        }
    }
}

