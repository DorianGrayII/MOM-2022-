namespace MHUtils.NeuralNetwork
{
    public class NeuralUtils
    {
        public static double Clamp(double val)
        {
            if (val < -1.0)
            {
                return -1.0;
            }
            if (val > 1.0)
            {
                return 1.0;
            }
            return val;
        }

        public static double Clamp(double val, double scale)
        {
            if (val < -1.0 * scale)
            {
                return -1.0 * scale;
            }
            if (val > 1.0 * scale)
            {
                return 1.0 * scale;
            }
            return val;
        }
    }
}
