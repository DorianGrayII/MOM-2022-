namespace MOM
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class BattleResult
    {
        public List<BattleUnit> attacker;
        public List<BattleUnit> defender;
        public int[] aWinPerRank;
        public int[] dWinPerRank;
        public int aWinAveraged;
        public int dWinAveraged;
        public int aManaLeft;
        public int dManaLeft;

        public BattleResult()
        {
            this.ResetWinPerRank();
        }

        public int GetAttackerWinAveraged()
        {
            if (this.aWinAveraged == 0)
            {
                int num = 0;
                int num2 = 0;
                int[] aWinPerRank = this.aWinPerRank;
                int index = 0;
                while (true)
                {
                    if (index >= aWinPerRank.Length)
                    {
                        float f = ((float) num2) / ((float) num);
                        this.aWinAveraged = Mathf.RoundToInt(f);
                        break;
                    }
                    num += aWinPerRank[index];
                    num2 += (index + 1) * aWinPerRank[index];
                    index++;
                }
            }
            return this.aWinAveraged;
        }

        public int GetDefenderWinAveraged()
        {
            if (this.dWinAveraged == 0)
            {
                int num = 0;
                int num2 = 0;
                int[] dWinPerRank = this.dWinPerRank;
                int index = 0;
                while (true)
                {
                    if (index >= dWinPerRank.Length)
                    {
                        float f = ((float) num2) / ((float) num);
                        this.dWinAveraged = Mathf.RoundToInt(f);
                        break;
                    }
                    num += dWinPerRank[index];
                    num2 += (index + 1) * dWinPerRank[index];
                    index++;
                }
            }
            return this.dWinAveraged;
        }

        public int GetResultRank(List<BattleUnit> aOrigList, List<BattleUnit> dOrigList)
        {
            int num = 0;
            int num2 = 0;
            foreach (BattleUnit v in aOrigList)
            {
                if (this.attacker == null)
                {
                    num2 += 5;
                }
                BattleUnit unit = this.attacker.Find(o => o.ID == v.ID);
                if ((unit == null) || (unit.figureCount == 0))
                {
                    num2 += 5;
                }
                else if (v.figureCount != 0)
                {
                    num2 += (int) (5f * (1f - Mathf.Clamp01(((float) unit.figureCount) / ((float) v.figureCount))));
                }
            }
            foreach (BattleUnit unit1 in dOrigList)
            {
                if (this.defender == null)
                {
                    num += 5;
                }
                BattleUnit unit2 = this.defender.Find(o => o.ID == unit1.ID);
                if ((unit2 == null) || (unit2.figureCount == 0))
                {
                    num += 5;
                }
                else if (unit1.figureCount != 0)
                {
                    num += (int) (5f * (1f - Mathf.Clamp01(((float) unit2.figureCount) / ((float) unit1.figureCount))));
                }
            }
            float num3 = ((float) num2) / ((float) (aOrigList.Count * 5));
            float single1 = ((float) num) / ((float) (dOrigList.Count * 5));
            float num4 = Mathf.Abs((float) (single1 - num3));
            int num5 = (single1 > num3) ? 1 : -1;
            return ((num4 <= 0.9f) ? ((num4 <= 0.5f) ? ((num4 <= 0.25f) ? ((num4 <= 0f) ? num5 : (2 * num5)) : (3 * num5)) : (4 * num5)) : (5 * num5));
        }

        public unsafe void RegisterWinByRank(int rank, bool attacker)
        {
            if (attacker)
            {
                int* numPtr1 = &(this.aWinPerRank[rank - 1]);
                numPtr1[0]++;
            }
            else
            {
                int* numPtr2 = &(this.dWinPerRank[rank - 1]);
                numPtr2[0]++;
            }
        }

        public void ResetWinPerRank()
        {
            this.aWinPerRank = new int[5];
            this.dWinPerRank = new int[5];
        }
    }
}

