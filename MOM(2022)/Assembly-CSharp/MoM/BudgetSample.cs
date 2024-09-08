using DBDef;

namespace MOM
{
    public class BudgetSample
    {
        public static float Sample(TurnToBudget[] points, int turn)
        {
            if (turn <= points[0].turn)
            {
                return points[0].budget;
            }
            if (turn >= points[points.Length - 1].turn)
            {
                return points[points.Length - 1].budget;
            }
            int num = -1;
            for (int num2 = points.Length - 1; num2 > 0; num2--)
            {
                if (points[num2].turn <= turn)
                {
                    num = num2;
                    break;
                }
            }
            if (num < 0)
            {
                return points[0].budget;
            }
            int num3 = num + 1;
            if (num3 >= points.Length)
            {
                return points[points.Length - 1].budget;
            }
            int turn2 = points[num].turn;
            int num4 = points[num3].turn - turn2;
            if (num4 == 0)
            {
                return points[num].budget;
            }
            float num5 = (float)(turn - turn2) / (float)num4;
            return (float)points[num].budget * (1f - num5) + (float)points[num3].budget * num5;
        }
    }
}
