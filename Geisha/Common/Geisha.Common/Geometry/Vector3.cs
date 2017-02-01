using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Geisha.Common.Geometry
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public static Vector3 Zero => new Vector3(0, 0, 0);
        public static Vector3 One => new Vector3(1, 1, 1);

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
        public Vector3 Unit => new Vector3(X / Length, Y / Length, Z / Length);
        public Vector3 Opposite => new Vector3(-X, -Y, -Z);
        public Vector4 Homogeneous => new Vector4(X, Y, Z, 1);
        public double[] Array => new[] {X, Y, Z};

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(IReadOnlyList<double> array)
        {
            if (array.Count != 3)
            {
                throw new ArgumentException("Array must be the length of 3 elements.");
            }

            X = array[0];
            Y = array[1];
            Z = array[2];
        }

        public Vector3 Add(Vector3 other)
        {
            return new Vector3(X + other.X, Y + other.Y, Z + other.Z);
        }

        public Vector3 Subtract(Vector3 other)
        {
            return new Vector3(X - other.X, Y - other.Y, Z - other.Z);
        }

        public Vector3 Multiply(double scalar)
        {
            return new Vector3(X * scalar, Y * scalar, Z * scalar);
        }

        public Vector3 Divide(double scalar)
        {
            return new Vector3(X / scalar, Y / scalar, Z / scalar);
        }

        public double Dot(Vector3 other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public double Distance(Vector3 other)
        {
            return Subtract(other).Length;
        }

        public bool Equals(Vector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3 && Equals((Vector3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        }

        [Pure]
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return left.Add(right);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return left.Subtract(right);
        }

        public static Vector3 operator *(Vector3 left, double right)
        {
            return left.Multiply(right);
        }

        public static Vector3 operator /(Vector3 left, double right)
        {
            return left.Divide(right);
        }

        public static Vector3 operator -(Vector3 right)
        {
            return right.Opposite;
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.Equals(right);
        }
    }
}