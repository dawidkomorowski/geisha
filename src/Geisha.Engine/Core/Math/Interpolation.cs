namespace Geisha.Engine.Core.Math
{
    public static class Interpolation
    {
        public static double Lerp(double a, double b, double alpha)
        {
            return a * (1 - alpha) + b * alpha;
        }
    }
}