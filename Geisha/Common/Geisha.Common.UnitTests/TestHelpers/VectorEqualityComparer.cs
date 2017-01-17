using System;
using System.Collections.Generic;
using Geisha.Common.Geometry;

namespace Geisha.Common.UnitTests.TestHelpers
{
    public static class VectorEqualityComparer
    {
        public static IEqualityComparer<Vector2> Vector2()
        {
            return new Vector2EqualityComparer();
        }

        public static IEqualityComparer<Vector2> Vector2(double tolerance)
        {
            return new Vector2EqualityComparer(tolerance);
        }

        private class DoubleWithinToleranceEqualityComparer
        {
            private readonly double _tolerance = double.Epsilon;

            protected DoubleWithinToleranceEqualityComparer()
            {
            }

            protected DoubleWithinToleranceEqualityComparer(double tolerance)
            {
                _tolerance = tolerance;
            }

            protected bool DoubleEquals(double x, double y)
            {
                return Math.Abs(x - y) <= _tolerance;
            }
        }

        private class Vector2EqualityComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Vector2>
        {
            public Vector2EqualityComparer()
            {
            }

            public Vector2EqualityComparer(double tolerance) : base(tolerance)
            {
            }

            public bool Equals(Vector2 v1, Vector2 v2)
            {
                return DoubleEquals(v1.X, v2.X) && DoubleEquals(v1.Y, v2.Y);
            }

            public int GetHashCode(Vector2 obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}