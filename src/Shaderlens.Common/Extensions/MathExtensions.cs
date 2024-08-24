namespace Shaderlens.Extensions
{
    public static class MathExtensions
    {
        public const double Tau = 6.283185307179586476925;

        public static double Clamp(double value, double minValue, double maxValue)
        {
            return value < minValue ? minValue :
                   value > maxValue ? maxValue : value;
        }

        public static double Cbrt(double value)
        {
            return Math.Pow(value, 1.0 / 3.0);
        }
    }
}
