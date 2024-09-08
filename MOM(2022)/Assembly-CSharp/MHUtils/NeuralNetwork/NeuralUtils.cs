namespace MHUtils.NeuralNetwork
{
    using System;

    public class NeuralUtils
    {
        public static double Clamp(double val)
        {
            return ((val >= -1.0) ? ((val <= 1.0) ? val : 1.0) : -1.0);
        }

        public static double Clamp(double val, double scale)
        {
            return ((val >= (-1.0 * scale)) ? ((val <= (1.0 * scale)) ? val : (1.0 * scale)) : (-1.0 * scale));
        }
    }
}

