using System.Collections.Generic;
using Geisha.Common.Math;

namespace Geisha.Common.TestUtils
{
    // TODO Could it be replaced with DefaultFloatingPointToleranceAttribute?
    public static class VectorEqualityComparer
    {
        public static IEqualityComparer<Vector2> Vector2(double tolerance)
        {
            return new Vector2EqualityComparer(tolerance);
        }

        public static IEqualityComparer<Vector3> Vector3(double tolerance)
        {
            return new Vector3EqualityComparer(tolerance);
        }

        private class DoubleWithinToleranceEqualityComparer
        {
            private readonly double _tolerance;

            protected DoubleWithinToleranceEqualityComparer(double tolerance)
            {
                _tolerance = tolerance;
            }

            protected bool DoubleEquals(double x, double y)
            {
                return System.Math.Abs(x - y) <= _tolerance;
            }
        }

        private class Vector2EqualityComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Vector2>
        {
            public Vector2EqualityComparer(double tolerance) : base(tolerance)
            {
            }

            public bool Equals(Vector2 v1, Vector2 v2)
            {
                return DoubleEquals(v1.X, v2.X) && DoubleEquals(v1.Y, v2.Y);
            }

            public int GetHashCode(Vector2 obj)
            {
                return obj.GetHashCode();
            }
        }

        private class Vector3EqualityComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Vector3>
        {
            public Vector3EqualityComparer(double tolerance) : base(tolerance)
            {
            }

            public bool Equals(Vector3 v1, Vector3 v2)
            {
                return DoubleEquals(v1.X, v2.X) && DoubleEquals(v1.Y, v2.Y) && DoubleEquals(v1.Z, v2.Z);
            }

            public int GetHashCode(Vector3 obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}