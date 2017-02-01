using System;
using System.Collections.Generic;

namespace Geisha.Common.Geometry
{
    public struct Vector4 : IEquatable<Vector4>
    {
        public static Vector4 Zero => new Vector4(0, 0, 0, 0);
        public static Vector4 One => new Vector4(1, 1, 1, 1);

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double W { get; }

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        public Vector4 Unit => new Vector4(X / Length, Y / Length, Z / Length, W / Length);
        public Vector4 Opposite => new Vector4(-X, -Y, -Z, -W);
        public double[] Array => new[] {X, Y, Z, W};

        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(IReadOnlyList<double> array)
        {
            if (array.Count != 4)
            {
                throw new ArgumentException("Array must be the length of 4 elements.");
            }

            X = array[0];
            Y = array[1];
            Z = array[2];
            W = array[3];
        }

        public Vector4 Add(Vector4 other)
        {
            return new Vector4(X + other.X, Y + other.Y, Z + other.Z, W + other.W);
        }

        public Vector4 Subtract(Vector4 other)
        {
            return new Vector4(X - other.X, Y - other.Y, Z - other.Z, W - other.W);
        }

        public Vector4 Multiply(double scalar)
        {
            return new Vector4(X * scalar, Y * scalar, Z * scalar, W * scalar);
        }

        public Vector4 Divide(double scalar)
        {
            return new Vector4(X / scalar, Y / scalar, Z / scalar, W / scalar);
        }

        public double Dot(Vector4 other)
        {
            return X * other.X + Y * other.Y + Z * other.Z + W * other.W;
        }

        public double Distance(Vector4 other)
        {
            return Subtract(other).Length;
        }

        public bool Equals(Vector4 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector4 && Equals((Vector4) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return left.Add(right);
        }

        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return left.Subtract(right);
        }

        public static Vector4 operator *(Vector4 left, double right)
        {
            return left.Multiply(right);
        }

        public static Vector4 operator /(Vector4 left, double right)
        {
            return left.Divide(right);
        }

        public static Vector4 operator -(Vector4 right)
        {
            return right.Opposite;
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !left.Equals(right);
        }
    }
}