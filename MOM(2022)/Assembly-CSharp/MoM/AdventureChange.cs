namespace MOM
{
    public class AdventureChange
    {
        public enum Change
        {
            MoneyGain = 0,
            MoneyLost = 1,
            MoneyGainTurn = 2,
            MoneyLostTurn = 3
        }

        public string image;

        public int value;
    }
}
