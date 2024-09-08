namespace MOM
{
    using DBDef;
    using System;

    public class BudgetSample
    {
        public static float Sample(TurnToBudget[] points, int turn)
        {
            if (turn <= points[0].turn)
            {
                return (float) points[0].budget;
            }
            if (turn >= points[points.Length - 1].turn)
            {
                return (float) points[points.Length - 1].budget;
            }
            int index = -1;
            int num6 = points.Length - 1;
            while (true)
            {
                if (num6 > 0)
                {
                    if (points[num6].turn > turn)
                    {
                        num6--;
                        continue;
                    }
                    index = num6;
                }
                if (index < 0)
                {
                    return (float) points[0].budget;
                }
                int num2 = index + 1;
                if (num2 >= points.Length)
                {
                    return (float) points[points.Length - 1].budget;
                }
                int num3 = points[index].turn;
                int num4 = points[num2].turn - num3;
                if (num4 == 0)
                {
                    return (float) points[index].budget;
                }
                float num5 = ((float) (turn - num3)) / ((float) num4);
                return ((points[index].budget * (1f - num5)) + (points[num2].budget * num5));
            }
        }
    }
}

