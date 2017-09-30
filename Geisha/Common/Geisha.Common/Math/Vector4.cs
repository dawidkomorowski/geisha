using System;
using System.Collections.Generic;

namespace Geisha.Common.Math
{
    public struct Vector4 : IEquatable<Vector4>
    {
        public static Vector4 Zero => new Vector4(0, 0, 0, 0);
        public static Vector4 One => new Vector4(1, 1, 1, 1);
        public static Vector4 VectorX => new Vector4(1, 0, 0, 0);
        public static Vector4 VectorY => new Vector4(0, 1, 0, 0);
        public static Vector4 VectorZ => new Vector4(0, 0, 1, 0);
        public static Vector4 VectorW => new Vector4(0, 0, 0, 1);

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double W { get; }

        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public Vector4 Unit => Length > double.Epsilon ? new Vector4(X / Length, Y / Length, Z / Length, W / Length) : Zero;

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
                throw new ArgumentException("Array must have length of 4 elements.");

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

        /// <summary>
        ///     Returns <see cref="Vector4" /> that has the same direction to this <see cref="Vector4" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector4" />.</param>
        /// <returns><see cref="Vector4" /> of given length.</returns>
        public Vector4 OfLength(double length)
        {
            return Unit * length;
        }

        /// <summary>
        ///     Returns <see cref="Vector4" /> that has the same direction to this <see cref="Vector4" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector4" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector4" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector4" /> of given length.
        /// </returns>
        public Vector4 Clamp(double maxLength)
        {
            return Length > maxLength ? OfLength(maxLength) : this;
        }

        /// <summary>
        ///     Returns <see cref="Vector4" /> that has the same direction to this <see cref="Vector4" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of given <paramref name="maxLength" />.
        /// </summary>
        /// <param name="minLength">Minimum allowed length of returned <see cref="Vector4" />.</param>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector4" />.</param>
        /// <returns>
        ///     <see cref="Vector4" /> of <paramref name="minLength" /> when this <see cref="Vector4" />
        ///     <see cref="Length" /> is lower than <paramref name="minLength" />. <see cref="Vector4" /> of
        ///     <paramref name="maxLength" /> when this <see cref="Vector4" /> <see cref="Length" /> is greater than
        ///     <paramref name="maxLength" />. Copy of this <see cref="Vector4" /> if its <see cref="Length" /> is grater or equal
        ///     to <paramref name="minLength" /> and lower or equal to <paramref name="maxLength" />.
        /// </returns>
        public Vector4 Clamp(double minLength, double maxLength)
        {
            return Length < minLength ? OfLength(minLength) : Clamp(maxLength);
        }

        public bool Equals(Vector4 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector4 other && Equals(other);
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