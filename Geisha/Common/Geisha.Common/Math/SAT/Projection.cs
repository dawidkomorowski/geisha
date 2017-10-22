﻿using System.Diagnostics;

namespace Geisha.Common.Math.SAT
{
    public struct Projection
    {
        public readonly double Min;
        public readonly double Max;

        public Projection(double min, double max)
        {
            Debug.Assert(min <= max, $"min < max: {min} < {max}");
            Min = min;
            Max = max;
        }

        public bool Overlaps(Projection other)
        {
            return System.Math.Abs(((Min + Max) - (other.Min + other.Max)) / 2) < ((Max - Min) + (other.Max - other.Min)) / 2;
        }
    }
}