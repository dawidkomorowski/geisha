namespace Geisha.Engine.Core.Math
{
    // TODO Add documentation.
    public static class GMath
    {
        public static bool AlmostEqual(double a, double b, double maxDiff = double.Epsilon, double maxRelativeDiff = 1e-15)
        {
            var diff = System.Math.Abs(a - b);

            if (diff <= maxDiff)
            {
                return true;
            }

            var largerMagnitude = System.Math.Max(System.Math.Abs(a), System.Math.Abs(b));

            return diff <= largerMagnitude * maxRelativeDiff;
        }

        public static double Lerp(double a, double b, double alpha)
        {
            return a * (1 - alpha) + b * alpha;
        }
    }
}