using System;
using System.Collections.Generic;

namespace Geisha.Common.Math
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 One => new Vector2(1, 1);
        public static Vector2 VectorX => new Vector2(1, 0);
        public static Vector2 VectorY => new Vector2(0, 1);

        public double X { get; }
        public double Y { get; }

        public double Length => System.Math.Sqrt(X * X + Y * Y);
        public Vector2 Unit => Length > double.Epsilon ? new Vector2(X / Length, Y / Length) : Zero;
        public Vector2 Opposite => new Vector2(-X, -Y);
        public Vector3 Homogeneous => new Vector3(X, Y, 1);
        public double[] Array => new[] {X, Y};

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2(IReadOnlyList<double> array)
        {
            if (array.Count != 2)
                throw new ArgumentException("Array must have the length of 2 elements.");

            X = array[0];
            Y = array[1];
        }

        public Vector2 Add(Vector2 other)
        {
            return new Vector2(X + other.X, Y + other.Y);
        }

        public Vector2 Subtract(Vector2 other)
        {
            return new Vector2(X - other.X, Y - other.Y);
        }

        public Vector2 Multiply(double scalar)
        {
            return new Vector2(X * scalar, Y * scalar);
        }

        public Vector2 Divide(double scalar)
        {
            return new Vector2(X / scalar, Y / scalar);
        }

        public double Dot(Vector2 other)
        {
            return X * other.X + Y * other.Y;
        }

        public double Distance(Vector2 other)
        {
            return Subtract(other).Length;
        }

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has the same direction to this <see cref="Vector2" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector2" />.</param>
        /// <returns><see cref="Vector2" /> of given length.</returns>
        public Vector2 OfLength(double length)
        {
            return Unit * length;
        }

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has the same direction to this <see cref="Vector2" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector2" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector2" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector2" /> of given length.
        /// </returns>
        public Vector2 Clamp(double maxLength)
        {
            return Length > maxLength ? OfLength(maxLength) : this;
        }

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has the same direction to this <see cref="Vector2" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of given <paramref name="maxLength" />.
        /// </summary>
        /// <param name="minLength">Minimum allowed length of returned <see cref="Vector2" />.</param>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector2" />.</param>
        /// <returns>
        ///     <see cref="Vector2" /> of <paramref name="minLength" /> when this <see cref="Vector2" />
        ///     <see cref="Length" /> is lower than <paramref name="minLength" />. <see cref="Vector2" /> of
        ///     <paramref name="maxLength" /> when this <see cref="Vector2" /> <see cref="Length" /> is greater than
        ///     <paramref name="maxLength" />. Copy of this <see cref="Vector2" /> if its <see cref="Length" /> is grater or equal
        ///     to <paramref name="minLength" /> and lower or equal to <paramref name="maxLength" />.
        /// </returns>
        public Vector2 Clamp(double minLength, double maxLength)
        {
            return Length < minLength ? OfLength(minLength) : Clamp(maxLength);
        }

        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return left.Add(right);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return left.Subtract(right);
        }

        public static Vector2 operator *(Vector2 left, double right)
        {
            return left.Multiply(right);
        }

        public static Vector2 operator /(Vector2 left, double right)
        {
            return left.Divide(right);
        }

        public static Vector2 operator -(Vector2 right)
        {
            return right.Opposite;
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }
    }
}