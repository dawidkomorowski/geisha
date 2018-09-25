using System;
using System.Collections.Generic;

namespace Geisha.Common.Math
{
    /// <summary>
    ///     Represents 2D mathematical vector consisting of two components X and Y. It is also used as a point in 2D space.
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        /// <summary>
        ///     Returns <see cref="Vector2" /> that has all components set to zero.
        /// </summary>
        public static Vector2 Zero => new Vector2(0, 0);

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has all components set to one.
        /// </summary>
        public static Vector2 One => new Vector2(1, 1);

        /// <summary>
        ///     Returns unit <see cref="Vector2" /> directed along the X axis, that is <see cref="X" /> is set to one and
        ///     <see cref="Y" /> is set to zero.
        /// </summary>
        public static Vector2 VectorX => new Vector2(1, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector2" /> directed along the Y axis, that is <see cref="X" /> is set to zero and
        ///     <see cref="Y" /> is set to one.
        /// </summary>
        public static Vector2 VectorY => new Vector2(0, 1);

        /// <summary>
        ///     X component of <see cref="Vector2" />.
        /// </summary>
        public double X { get; }

        /// <summary>
        ///     Y component of <see cref="Vector2" />.
        /// </summary>
        public double Y { get; }

        /// <summary>
        ///     Length of <see cref="Vector2" />.
        /// </summary>
        public double Length => System.Math.Sqrt(X * X + Y * Y);

        // TODO Encapsulate near zero test in a common method to have the same logic across the codebase.
        /// <summary>
        ///     Returns unit vector out of this <see cref="Vector2" /> that is vector with the same direction but with length equal
        ///     one.
        /// </summary>
        /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
        public Vector2 Unit => Length > double.Epsilon ? new Vector2(X / Length, Y / Length) : Zero;

        /// <summary>
        ///     Returns vector opposite to this vector, that is vector with all components negated.
        /// </summary>
        public Vector2 Opposite => new Vector2(-X, -Y);

        /// <summary>
        ///     Returns normal (perpendicular unit vector) <see cref="Vector2" /> rotated 90 degrees counterclockwise.
        /// </summary>
        public Vector2 Normal => new Vector2(-Y, X).Unit;

        /// <summary>
        ///     Returns <see cref="Vector3" /> that is this <see cref="Vector2" /> in homogeneous coordinates.
        /// </summary>
        /// <remarks>
        ///     Homogeneous coordinates add additional component of value one therefore <see cref="Vector2" /> in homogeneous
        ///     coordinates is represented by <see cref="Vector3" /> with Z component equal one.
        /// </remarks>
        public Vector3 Homogeneous => new Vector3(X, Y, 1);

        /// <summary>
        ///     Creates new instance of <see cref="Vector2" /> given X and Y components values.
        /// </summary>
        /// <param name="x">X component value.</param>
        /// <param name="y">Y component value.</param>
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Vector2" /> given array of size two containing X and Y components values.
        /// </summary>
        /// <param name="array">Array of size two with X and Y components values.</param>
        public Vector2(IReadOnlyList<double> array)
        {
            if (array.Count != 2)
                throw new ArgumentException("Array must have length of 2 elements.");

            X = array[0];
            Y = array[1];
        }

        /// <summary>
        ///     Adds other vector to this vector.
        /// </summary>
        /// <param name="other">Other vector to add.</param>
        /// <returns><see cref="Vector2" /> that is sum of this vector with the other.</returns>
        public Vector2 Add(Vector2 other)
        {
            return new Vector2(X + other.X, Y + other.Y);
        }

        /// <summary>
        ///     Subtracts other vector from this vector.
        /// </summary>
        /// <param name="other">Other vector to subtract.</param>
        /// <returns><see cref="Vector2" /> that is difference between this vector and the other.</returns>
        public Vector2 Subtract(Vector2 other)
        {
            return new Vector2(X - other.X, Y - other.Y);
        }

        /// <summary>
        ///     Multiplies this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector2" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Vector2 Multiply(double scalar)
        {
            return new Vector2(X * scalar, Y * scalar);
        }

        /// <summary>
        ///     Divides this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector2" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Vector2 Divide(double scalar)
        {
            return new Vector2(X / scalar, Y / scalar);
        }

        /// <summary>
        ///     Calculates dot product of this vector with the other.
        /// </summary>
        /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
        /// <returns>Dot product of this vector with the other.</returns>
        public double Dot(Vector2 other)
        {
            return X * other.X + Y * other.Y;
        }

        /// <summary>
        ///     Calculates distance between point represented by this vector and point represented by other vector.
        /// </summary>
        /// <param name="other">Other vector representing a point.</param>
        /// <returns>Distance between points represented by this vector and the other.</returns>
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

        /// <summary>
        ///     Returns copy of this vector with X component set as specified.
        /// </summary>
        /// <param name="x">X component value of new vector.</param>
        /// <returns>Copy of this vector with X component set as specified.</returns>
        public Vector2 WithX(double x)
        {
            return new Vector2(x, Y);
        }

        /// <summary>
        ///     Returns copy of this vector with Y component set as specified.
        /// </summary>
        /// <param name="y">Y component value of new vector.</param>
        /// <returns>Copy of this vector with Y component set as specified.</returns>
        public Vector2 WithY(double y)
        {
            return new Vector2(X, y);
        }

        /// <summary>
        ///     Returns array that contains vector components in order X, Y.
        /// </summary>
        /// <returns>Array with vector components.</returns>
        public double[] ToArray()
        {
            return new[] {X, Y};
        }

        /// <summary>
        ///     Returns a value indicating whether the value of this instance is equal to the value of the specified
        ///     <see cref="Vector2" /> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="other" /> parameter equals the value of this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an instance of <see cref="Vector2" /> and equals the value of this
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2 other && Equals(other);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        ///     Converts the value of the current <see cref="Vector2" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Vector2" /> object.</returns>
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        /// <summary>
        ///     Returns <see cref="Vector3" /> that represents this <see cref="Vector2" />. Returned <see cref="Vector3" /> has the
        ///     same X and Y while its Z is set to zero.
        /// </summary>
        /// <returns><see cref="Vector3" /> that has the same X and Y to this <see cref="Vector2" /> while its Z is set to zero.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, 0);
        }

        /// <summary>
        ///     Returns <see cref="Vector4" /> that represents this <see cref="Vector2" />. Returned <see cref="Vector4" /> has the
        ///     same X and Y while its Z and W are set to zero.
        /// </summary>
        /// <returns>
        ///     <see cref="Vector4" /> that has the same X and Y to this <see cref="Vector2" /> while its Z and W are set to
        ///     zero.
        /// </returns>
        public Vector4 ToVector4()
        {
            return new Vector4(X, Y, 0, 0);
        }

        /// <summary>
        ///     Adds one vector to another.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return left.Add(right);
        }

        /// <summary>
        ///     Subtracts one vector from another.
        /// </summary>
        /// <param name="left">Vector to subtract from (the minuend).</param>
        /// <param name="right">Vector to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return left.Subtract(right);
        }

        /// <summary>
        ///     Multiplies specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector2" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Vector2 operator *(Vector2 left, double right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        ///     Divides specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="right">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector2" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Vector2 operator /(Vector2 left, double right)
        {
            return left.Divide(right);
        }

        /// <summary>
        ///     Returns vector opposite to the specified vector, that is vector with all components negated.
        /// </summary>
        /// <param name="right">Vector to be negated.</param>
        /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
        public static Vector2 operator -(Vector2 right)
        {
            return right.Opposite;
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector2" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Vector2" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector2" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Vector2" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }
    }
}