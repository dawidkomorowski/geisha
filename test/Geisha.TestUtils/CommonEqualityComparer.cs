using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.TestUtils;

public static class CommonEqualityComparer
{
    public static IEqualityComparer<Vector2> Vector2(double tolerance) => new Vector2EqualityComparer(tolerance);
    public static IEqualityComparer<Vector3> Vector3(double tolerance) => new Vector3EqualityComparer(tolerance);

    // ReSharper disable once InconsistentNaming
    public static IEqualityComparer<Matrix3x3> Matrix3x3(double tolerance) => new Matrix3x3EqualityComparer(tolerance);
    public static IEqualityComparer<Transform2D> Transform2D(double tolerance) => new Transform2DComparer(tolerance);

    private class DoubleWithinToleranceEqualityComparer
    {
        private readonly double _tolerance;

        protected DoubleWithinToleranceEqualityComparer(double tolerance)
        {
            _tolerance = tolerance;
        }

        protected bool DoubleEquals(double x, double y) => System.Math.Abs(x - y) <= _tolerance;
    }

    private sealed class Vector2EqualityComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Vector2>
    {
        public Vector2EqualityComparer(double tolerance) : base(tolerance)
        {
        }

        public bool Equals(Vector2 v1, Vector2 v2) => DoubleEquals(v1.X, v2.X) && DoubleEquals(v1.Y, v2.Y);

        public int GetHashCode(Vector2 obj) => obj.GetHashCode();
    }

    private sealed class Vector3EqualityComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Vector3>
    {
        public Vector3EqualityComparer(double tolerance) : base(tolerance)
        {
        }

        public bool Equals(Vector3 v1, Vector3 v2) => DoubleEquals(v1.X, v2.X) && DoubleEquals(v1.Y, v2.Y) && DoubleEquals(v1.Z, v2.Z);

        public int GetHashCode(Vector3 obj) => obj.GetHashCode();
    }

    // ReSharper disable once InconsistentNaming
    private sealed class Matrix3x3EqualityComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Matrix3x3>
    {
        public Matrix3x3EqualityComparer(double tolerance) : base(tolerance)
        {
        }

        public bool Equals(Matrix3x3 x, Matrix3x3 y) =>
            DoubleEquals(x.M11, y.M11) && DoubleEquals(x.M12, y.M12) && DoubleEquals(x.M13, y.M13)
            && DoubleEquals(x.M21, y.M21) && DoubleEquals(x.M22, y.M22) && DoubleEquals(x.M23, y.M23)
            && DoubleEquals(x.M31, y.M31) && DoubleEquals(x.M32, y.M32) && DoubleEquals(x.M33, y.M33);

        public int GetHashCode(Matrix3x3 obj) => obj.GetHashCode();
    }

    private sealed class Transform2DComparer : DoubleWithinToleranceEqualityComparer, IEqualityComparer<Transform2D>
    {
        private readonly IEqualityComparer<Vector2> _vector2Comparer;

        public Transform2DComparer(double tolerance) : base(tolerance)
        {
            _vector2Comparer = Vector2(tolerance);
        }

        public bool Equals(Transform2D x, Transform2D y)
        {
            return _vector2Comparer.Equals(x.Translation, y.Translation) && DoubleEquals(x.Rotation, y.Rotation) &&
                   _vector2Comparer.Equals(x.Scale, y.Scale);
        }

        public int GetHashCode(Transform2D obj) => obj.GetHashCode();
    }
}