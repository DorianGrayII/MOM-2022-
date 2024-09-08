using System.Collections.Generic;
using MHUtils;

namespace MOM
{
    public class StatDetails
    {
        public Dictionary<string, FInt> breakdown = new Dictionary<string, FInt>();

        public bool intMode;

        public void Add(string id, int amount, bool allowZero = false)
        {
            this.intMode = true;
            this.Add(id, new FInt(amount), allowZero);
        }

        public void Add(string id, FInt amount, bool allowZero = false)
        {
            if (allowZero || amount != 0)
            {
                FInt value = FInt.ZERO;
                this.breakdown.TryGetValue(id, out value);
                FInt fInt = value + amount;
                if (allowZero || fInt != 0)
                {
                    this.breakdown[id] = fInt;
                }
                else
                {
                    this.breakdown.Remove(id);
                }
            }
        }

        public void Clear()
        {
            this.breakdown.Clear();
        }
    }
}
