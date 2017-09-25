using System;
using System.Collections.Generic;

namespace Geisha.Common.Math
{
    public struct Matrix4 : IEquatable<Matrix4>
    {
        public static Matrix4 Zero => new Matrix4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static Matrix4 Identity => new Matrix4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        public double M11 { get; }
        public double M12 { get; }
        public double M13 { get; }
        public double M14 { get; }
        public double M21 { get; }
        public double M22 { get; }
        public double M23 { get; }
        public double M24 { get; }
        public double M31 { get; }
        public double M32 { get; }
        public double M33 { get; }
        public double M34 { get; }
        public double M41 { get; }
        public double M42 { get; }
        public double M43 { get; }
        public double M44 { get; }

        public Matrix4 Opposite
            =>
                new Matrix4(-M11, -M12, -M13, -M14, -M21, -M22, -M23, -M24, -M31, -M32, -M33, -M34, -M41, -M42, -M43,
                    -M44);

        public double[] Array => new[] {M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44};

        public double this[int row, int column]
        {
            get
            {
                var index = row * 4 + column;

                switch (index)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M14;
                    case 4:
                        return M21;
                    case 5:
                        return M22;
                    case 6:
                        return M23;
                    case 7:
                        return M24;
                    case 8:
                        return M31;
                    case 9:
                        return M32;
                    case 10:
                        return M33;
                    case 11:
                        return M34;
                    case 12:
                        return M41;
                    case 13:
                        return M42;
                    case 14:
                        return M43;
                    case 15:
                        return M44;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Matrix4(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        public Matrix4(IReadOnlyList<double> array)
        {
            if (array.Count != 16)
            {
                throw new ArgumentException("Array must be the length of 16 elements.");
            }

            M11 = array[0];
            M12 = array[1];
            M13 = array[2];
            M14 = array[3];

            M21 = array[4];
            M22 = array[5];
            M23 = array[6];
            M24 = array[7];

            M31 = array[8];
            M32 = array[9];
            M33 = array[10];
            M34 = array[11];

            M41 = array[12];
            M42 = array[13];
            M43 = array[14];
            M44 = array[15];
        }

        public Matrix4 Add(Matrix4 other)
        {
            return new Matrix4(
                M11 + other.M11,
                M12 + other.M12,
                M13 + other.M13,
                M14 + other.M14,
                M21 + other.M21,
                M22 + other.M22,
                M23 + other.M23,
                M24 + other.M24,
                M31 + other.M31,
                M32 + other.M32,
                M33 + other.M33,
                M34 + other.M34,
                M41 + other.M41,
                M42 + other.M42,
                M43 + other.M43,
                M44 + other.M44
            );
        }

        public Matrix4 Subtract(Matrix4 other)
        {
            return new Matrix4(
                M11 - other.M11,
                M12 - other.M12,
                M13 - other.M13,
                M14 - other.M14,
                M21 - other.M21,
                M22 - other.M22,
                M23 - other.M23,
                M24 - other.M24,
                M31 - other.M31,
                M32 - other.M32,
                M33 - other.M33,
                M34 - other.M34,
                M41 - other.M41,
                M42 - other.M42,
                M43 - other.M43,
                M44 - other.M44
            );
        }

        public Matrix4 Multiply(double scalar)
        {
            return new Matrix4(
                M11 * scalar,
                M12 * scalar,
                M13 * scalar,
                M14 * scalar,
                M21 * scalar,
                M22 * scalar,
                M23 * scalar,
                M24 * scalar,
                M31 * scalar,
                M32 * scalar,
                M33 * scalar,
                M34 * scalar,
                M41 * scalar,
                M42 * scalar,
                M43 * scalar,
                M44 * scalar
            );
        }

        public Matrix4 Multiply(Matrix4 other)
        {
            return new Matrix4(
                M11 * other.M11 + M12 * other.M21 + M13 * other.M31 + M14 * other.M41,
                M11 * other.M12 + M12 * other.M22 + M13 * other.M32 + M14 * other.M42,
                M11 * other.M13 + M12 * other.M23 + M13 * other.M33 + M14 * other.M43,
                M11 * other.M14 + M12 * other.M24 + M13 * other.M34 + M14 * other.M44,
                M21 * other.M11 + M22 * other.M21 + M23 * other.M31 + M24 * other.M41,
                M21 * other.M12 + M22 * other.M22 + M23 * other.M32 + M24 * other.M42,
                M21 * other.M13 + M22 * other.M23 + M23 * other.M33 + M24 * other.M43,
                M21 * other.M14 + M22 * other.M24 + M23 * other.M34 + M24 * other.M44,
                M31 * other.M11 + M32 * other.M21 + M33 * other.M31 + M34 * other.M41,
                M31 * other.M12 + M32 * other.M22 + M33 * other.M32 + M34 * other.M42,
                M31 * other.M13 + M32 * other.M23 + M33 * other.M33 + M34 * other.M43,
                M31 * other.M14 + M32 * other.M24 + M33 * other.M34 + M34 * other.M44,
                M41 * other.M11 + M42 * other.M21 + M43 * other.M31 + M44 * other.M41,
                M41 * other.M12 + M42 * other.M22 + M43 * other.M32 + M44 * other.M42,
                M41 * other.M13 + M42 * other.M23 + M43 * other.M33 + M44 * other.M43,
                M41 * other.M14 + M42 * other.M24 + M43 * other.M34 + M44 * other.M44
            );
        }

        public Vector4 Multiply(Vector4 vector)
        {
            return new Vector4(
                M11 * vector.X + M12 * vector.Y + M13 * vector.Z + M14 * vector.W,
                M21 * vector.X + M22 * vector.Y + M23 * vector.Z + M24 * vector.W,
                M31 * vector.X + M32 * vector.Y + M33 * vector.Z + M34 * vector.W,
                M41 * vector.X + M42 * vector.Y + M43 * vector.Z + M44 * vector.W
            );
        }

        public Matrix4 Divide(double scalar)
        {
            return new Matrix4(
                M11 / scalar,
                M12 / scalar,
                M13 / scalar,
                M14 / scalar,
                M21 / scalar,
                M22 / scalar,
                M23 / scalar,
                M24 / scalar,
                M31 / scalar,
                M32 / scalar,
                M33 / scalar,
                M34 / scalar,
                M41 / scalar,
                M42 / scalar,
                M43 / scalar,
                M44 / scalar
            );
        }

        public bool Equals(Matrix4 other)
        {
            return M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M14.Equals(other.M14) &&
                   M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M24.Equals(other.M24) &&
                   M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33) && M34.Equals(other.M34) &&
                   M41.Equals(other.M41) && M42.Equals(other.M42) && M43.Equals(other.M43) && M44.Equals(other.M44);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Matrix4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = M11.GetHashCode();
                hashCode = (hashCode * 397) ^ M12.GetHashCode();
                hashCode = (hashCode * 397) ^ M13.GetHashCode();
                hashCode = (hashCode * 397) ^ M14.GetHashCode();
                hashCode = (hashCode * 397) ^ M21.GetHashCode();
                hashCode = (hashCode * 397) ^ M22.GetHashCode();
                hashCode = (hashCode * 397) ^ M23.GetHashCode();
                hashCode = (hashCode * 397) ^ M24.GetHashCode();
                hashCode = (hashCode * 397) ^ M31.GetHashCode();
                hashCode = (hashCode * 397) ^ M32.GetHashCode();
                hashCode = (hashCode * 397) ^ M33.GetHashCode();
                hashCode = (hashCode * 397) ^ M34.GetHashCode();
                hashCode = (hashCode * 397) ^ M41.GetHashCode();
                hashCode = (hashCode * 397) ^ M42.GetHashCode();
                hashCode = (hashCode * 397) ^ M43.GetHashCode();
                hashCode = (hashCode * 397) ^ M44.GetHashCode();
                return hashCode;
            }
        }

        public static Matrix4 Translation(Vector3 translation) => new Matrix4(
            1, 0, 0, translation.X,
            0, 1, 0, translation.Y,
            0, 0, 1, translation.Z,
            0, 0, 0, 1
        );

        public static Matrix4 RotationX(double angle) => new Matrix4(
            1, 0, 0, 0,
            0, System.Math.Cos(angle), -System.Math.Sin(angle), 0,
            0, System.Math.Sin(angle), System.Math.Cos(angle), 0,
            0, 0, 0, 1
        );

        public static Matrix4 RotationY(double angle) => new Matrix4(
            System.Math.Cos(angle), 0, System.Math.Sin(angle), 0,
            0, 1, 0, 0,
            -System.Math.Sin(angle), 0, System.Math.Cos(angle), 0,
            0, 0, 0, 1
        );

        public static Matrix4 RotationZ(double angle) => new Matrix4(
            System.Math.Cos(angle), -System.Math.Sin(angle), 0, 0,
            System.Math.Sin(angle), System.Math.Cos(angle), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        public static Matrix4 RotationZXY(Vector3 rotation)
            => RotationY(rotation.Y) * RotationX(rotation.X) * RotationZ(rotation.Z);

        public static Matrix4 Scale(Vector3 scale) => new Matrix4(
            scale.X, 0, 0, 0,
            0, scale.Y, 0, 0,
            0, 0, scale.Z, 0,
            0, 0, 0, 1
        );

        public static Matrix4 operator +(Matrix4 left, Matrix4 right)
        {
            return left.Add(right);
        }

        public static Matrix4 operator -(Matrix4 left, Matrix4 right)
        {
            return left.Subtract(right);
        }

        public static Matrix4 operator *(Matrix4 left, double right)
        {
            return left.Multiply(right);
        }

        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            return left.Multiply(right);
        }

        public static Vector4 operator *(Matrix4 left, Vector4 right)
        {
            return left.Multiply(right);
        }

        public static Matrix4 operator /(Matrix4 left, double right)
        {
            return left.Divide(right);
        }

        public static Matrix4 operator -(Matrix4 right)
        {
            return right.Opposite;
        }

        public static bool operator ==(Matrix4 left, Matrix4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix4 left, Matrix4 right)
        {
            return !left.Equals(right);
        }
    }
}