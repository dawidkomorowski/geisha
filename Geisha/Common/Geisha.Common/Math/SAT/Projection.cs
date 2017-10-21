using System.Diagnostics;

namespace Geisha.Common.Math.SAT
{
    public struct Projection
    {
        private readonly double _min;
        private readonly double _max;

        public Projection(double min, double max)
        {
            Debug.Assert(min <= max, $"min < max: {min} < {max}");
            _min = min;
            _max = max;
        }

        public bool Overlaps(Projection other)
        {
            return System.Math.Abs(((_min + _max) - (other._min + other._max)) / 2) < ((_max - _min) + (other._max - other._min)) / 2;
        }
    }
}