using System.Collections.Generic;
using UnityEngine;

namespace MOM
{
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

        public void ResetWinPerRank()
        {
            this.aWinPerRank = new int[5];
            this.dWinPerRank = new int[5];
        }

        public void RegisterWinByRank(int rank, bool attacker)
        {
            if (attacker)
            {
                this.aWinPerRank[rank - 1]++;
            }
            else
            {
                this.dWinPerRank[rank - 1]++;
            }
        }

        public int GetAttackerWinAveraged()
        {
            if (this.aWinAveraged == 0)
            {
                int num = 0;
                int num2 = 0;
                int[] array = this.aWinPerRank;
                for (int i = 0; i < array.Length; i++)
                {
                    num += array[i];
                    num2 += (i + 1) * array[i];
                }
                float f = (float)num2 / (float)num;
                this.aWinAveraged = Mathf.RoundToInt(f);
            }
            return this.aWinAveraged;
        }

        public int GetDefenderWinAveraged()
        {
            if (this.dWinAveraged == 0)
            {
                int num = 0;
                int num2 = 0;
                int[] array = this.dWinPerRank;
                for (int i = 0; i < array.Length; i++)
                {
                    num += array[i];
                    num2 += (i + 1) * array[i];
                }
                float f = (float)num2 / (float)num;
                this.dWinAveraged = Mathf.RoundToInt(f);
            }
            return this.dWinAveraged;
        }

        public int GetResultRank(List<BattleUnit> aOrigList, List<BattleUnit> dOrigList)
        {
            int num = 0;
            int num2 = 0;
            foreach (BattleUnit v2 in aOrigList)
            {
                if (this.attacker == null)
                {
                    num2 += 5;
                }
                BattleUnit battleUnit = this.attacker.Find((BattleUnit o) => o.ID == v2.ID);
                if (battleUnit == null || battleUnit.figureCount == 0)
                {
                    num2 += 5;
                }
                else if (v2.figureCount != 0)
                {
                    float num3 = Mathf.Clamp01((float)battleUnit.figureCount / (float)v2.figureCount);
                    num2 += (int)(5f * (1f - num3));
                }
            }
            foreach (BattleUnit v in dOrigList)
            {
                if (this.defender == null)
                {
                    num += 5;
                }
                BattleUnit battleUnit2 = this.defender.Find((BattleUnit o) => o.ID == v.ID);
                if (battleUnit2 == null || battleUnit2.figureCount == 0)
                {
                    num += 5;
                }
                else if (v.figureCount != 0)
                {
                    float num4 = Mathf.Clamp01((float)battleUnit2.figureCount / (float)v.figureCount);
                    num += (int)(5f * (1f - num4));
                }
            }
            float num5 = (float)num2 / (float)(aOrigList.Count * 5);
            float num6 = (float)num / (float)(dOrigList.Count * 5);
            float num7 = Mathf.Abs(num6 - num5);
            int num8 = ((num6 > num5) ? 1 : (-1));
            if (num7 > 0.9f)
            {
                return 5 * num8;
            }
            if (num7 > 0.5f)
            {
                return 4 * num8;
            }
            if (num7 > 0.25f)
            {
                return 3 * num8;
            }
            if (num7 > 0f)
            {
                return 2 * num8;
            }
            return num8;
        }
    }
}
