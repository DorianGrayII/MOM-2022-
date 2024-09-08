namespace MOM
{
    using System;

    public class AdventureChange
    {
        public string image;
        public int value;

        public enum Change
        {
            MoneyGain,
            MoneyLost,
            MoneyGainTurn,
            MoneyLostTurn
        }
    }
}

