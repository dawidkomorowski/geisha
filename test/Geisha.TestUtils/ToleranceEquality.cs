using System;
using Geisha.Engine.Core.Math;

namespace Geisha.TestUtils;

public static class ToleranceEquality
{
    public static Func<Vector2, Vector2, bool> ForVector2(double tolerance) => (v1, v2) => AlmostEquals(v1, v2, tolerance);
    public static Func<Vector3, Vector3, bool> ForVector3(double tolerance) => (v1, v2) => AlmostEquals(v1, v2, tolerance);

    // ReSharper disable once InconsistentNaming
    public static Func<Matrix3x3, Matrix3x3, bool> ForMatrix3x3(double tolerance) => (m1, m2) => AlmostEquals(m1, m2, tolerance);
    public static Func<Transform2D, Transform2D, bool> ForTransform2D(double tolerance) => (t1, t2) => AlmostEquals(t1, t2, tolerance);

    private static bool AlmostEquals(double x, double y, double tolerance) => Math.Abs(x - y) <= tolerance;

    private static bool AlmostEquals(in Vector2 v1, in Vector2 v2, double tolerance) =>
        AlmostEquals(v1.X, v2.X, tolerance) && AlmostEquals(v1.Y, v2.Y, tolerance);

    private static bool AlmostEquals(in Vector3 v1, in Vector3 v2, double tolerance) =>
        AlmostEquals(v1.X, v2.X, tolerance) && AlmostEquals(v1.Y, v2.Y, tolerance) && AlmostEquals(v1.Z, v2.Z, tolerance);

    private static bool AlmostEquals(in Matrix3x3 m1, in Matrix3x3 m2, double tolerance) =>
        AlmostEquals(m1.M11, m2.M11, tolerance) && AlmostEquals(m1.M12, m2.M12, tolerance) && AlmostEquals(m1.M13, m2.M13, tolerance)
        && AlmostEquals(m1.M21, m2.M21, tolerance) && AlmostEquals(m1.M22, m2.M22, tolerance) && AlmostEquals(m1.M23, m2.M23, tolerance)
        && AlmostEquals(m1.M31, m2.M31, tolerance) && AlmostEquals(m1.M32, m2.M32, tolerance) && AlmostEquals(m1.M33, m2.M33, tolerance);

    private static bool AlmostEquals(in Transform2D t1, in Transform2D t2, double tolerance) =>
        AlmostEquals(t1.Translation, t2.Translation, tolerance) &&
        AlmostEquals(t1.Rotation, t2.Rotation, tolerance) &&
        AlmostEquals(t1.Scale, t2.Scale, tolerance);
}