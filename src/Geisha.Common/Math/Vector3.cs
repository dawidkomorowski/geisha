using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Geisha.Common.Math
{
    /// <summary>
    ///     3D mathematical vector consisting of three components X, Y and Z. It is also used as a point in 3D space.
    /// </summary>
    public readonly struct Vector3 : IEquatable<Vector3>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Vector3" /> that has all components set to zero.
        /// </summary>
        public static Vector3 Zero => new Vector3(0, 0, 0);

        /// <summary>
        ///     Returns <see cref="Vector3" /> that has all components set to one.
        /// </summary>
        public static Vector3 One => new Vector3(1, 1, 1);

        /// <summary>
        ///     Returns unit <see cref="Vector3" /> directed along the X axis, that is vector (1,0,0).
        /// </summary>
        public static Vector3 UnitX => new Vector3(1, 0, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector3" /> directed along the Y axis, that is vector (0,1,0).
        /// </summary>
        public static Vector3 UnitY => new Vector3(0, 1, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector3" /> directed along the Z axis, that is vector (0,0,1).
        /// </summary>
        public static Vector3 UnitZ => new Vector3(0, 0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     X component of <see cref="Vector3" />.
        /// </summary>
        public double X { get; }

        /// <summary>
        ///     Y component of <see cref="Vector3" />.
        /// </summary>
        public double Y { get; }

        /// <summary>
        ///     Z component of <see cref="Vector3" />.
        /// </summary>
        public double Z { get; }

        /// <summary>
        ///     Length of <see cref="Vector3" />.
        /// </summary>
        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        ///     Returns unit vector out of this <see cref="Vector3" /> that is vector with the same direction but with length equal
        ///     one.
        /// </summary>
        /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
        public Vector3 Unit => Length > double.Epsilon ? new Vector3(X / Length, Y / Length, Z / Length) : Zero;

        /// <summary>
        ///     Returns vector opposite to this vector, that is vector with all components negated.
        /// </summary>
        public Vector3 Opposite => new Vector3(-X, -Y, -Z);

        /// <summary>
        ///     Returns <see cref="Vector4" /> that is this <see cref="Vector3" /> in homogeneous coordinates.
        /// </summary>
        /// <remarks>
        ///     Homogeneous coordinates add additional component of value one therefore <see cref="Vector3" /> in homogeneous
        ///     coordinates is represented by <see cref="Vector4" /> with W component equal one.
        /// </remarks>
        public Vector4 Homogeneous => new Vector4(X, Y, Z, 1);

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Vector3" /> given X, Y and Z components values.
        /// </summary>
        /// <param name="x">X component value.</param>
        /// <param name="y">Y component value.</param>
        /// <param name="z">Z component value.</param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Vector3" /> given array of size three containing X, Y and Z components values.
        /// </summary>
        /// <param name="array">Array of size three with X, Y and Z components values.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not three.</exception>
        public Vector3(IReadOnlyList<double> array)
        {
            if (array.Count != 3)
                throw new ArgumentException("Array must have length of 3 elements.");

            X = array[0];
            Y = array[1];
            Z = array[2];
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds other vector to this vector.
        /// </summary>
        /// <param name="other">Other vector to add.</param>
        /// <returns><see cref="Vector3" /> that is sum of this vector with the other.</returns>
        public Vector3 Add(Vector3 other)
        {
            return new Vector3(X + other.X, Y + other.Y, Z + other.Z);
        }

        /// <summary>
        ///     Subtracts other vector from this vector.
        /// </summary>
        /// <param name="other">Other vector to subtract.</param>
        /// <returns><see cref="Vector3" /> that is difference between this vector and the other.</returns>
        public Vector3 Subtract(Vector3 other)
        {
            return new Vector3(X - other.X, Y - other.Y, Z - other.Z);
        }

        /// <summary>
        ///     Multiplies this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector3" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Vector3 Multiply(double scalar)
        {
            return new Vector3(X * scalar, Y * scalar, Z * scalar);
        }

        /// <summary>
        ///     Divides this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector3" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Vector3 Divide(double scalar)
        {
            return new Vector3(X / scalar, Y / scalar, Z / scalar);
        }

        /// <summary>
        ///     Calculates dot product of this vector with the other.
        /// </summary>
        /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
        /// <returns>Dot product of this vector with the other.</returns>
        public double Dot(Vector3 other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        /// <summary>
        ///     Calculates distance between point represented by this vector and point represented by other vector.
        /// </summary>
        /// <param name="other">Other vector representing a point.</param>
        /// <returns>Distance between points represented by this vector and the other.</returns>
        public double Distance(Vector3 other)
        {
            return Subtract(other).Length;
        }

        /// <summary>
        ///     Returns <see cref="Vector3" /> that has the same direction to this <see cref="Vector3" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector3" />.</param>
        /// <returns><see cref="Vector3" /> of given length.</returns>
        public Vector3 OfLength(double length)
        {
            return Unit * length;
        }

        /// <summary>
        ///     Returns <see cref="Vector3" /> that has the same direction to this <see cref="Vector3" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector3" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector3" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector3" /> of given length.
        /// </returns>
        public Vector3 Clamp(double maxLength)
        {
            return Length > maxLength ? OfLength(maxLength) : this;
        }

        /// <summary>
        ///     Returns <see cref="Vector3" /> that has the same direction to this <see cref="Vector3" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of given <paramref name="maxLength" />.
        /// </summary>
        /// <param name="minLength">Minimum allowed length of returned <see cref="Vector3" />.</param>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector3" />.</param>
        /// <returns>
        ///     <see cref="Vector3" /> of <paramref name="minLength" /> when this <see cref="Vector3" />
        ///     <see cref="Length" /> is lower than <paramref name="minLength" />. <see cref="Vector3" /> of
        ///     <paramref name="maxLength" /> when this <see cref="Vector3" /> <see cref="Length" /> is greater than
        ///     <paramref name="maxLength" />. Copy of this <see cref="Vector3" /> if its <see cref="Length" /> is grater or equal
        ///     to <paramref name="minLength" /> and lower or equal to <paramref name="maxLength" />.
        /// </returns>
        public Vector3 Clamp(double minLength, double maxLength)
        {
            return Length < minLength ? OfLength(minLength) : Clamp(maxLength);
        }

        /// <summary>
        ///     Returns copy of this vector with X component set as specified.
        /// </summary>
        /// <param name="x">X component value of new vector.</param>
        /// <returns>Copy of this vector with X component set as specified.</returns>
        public Vector3 WithX(double x)
        {
            return new Vector3(x, Y, Z);
        }

        /// <summary>
        ///     Returns copy of this vector with Y component set as specified.
        /// </summary>
        /// <param name="y">Y component value of new vector.</param>
        /// <returns>Copy of this vector with Y component set as specified.</returns>
        public Vector3 WithY(double y)
        {
            return new Vector3(X, y, Z);
        }

        /// <summary>
        ///     Returns copy of this vector with Z component set as specified.
        /// </summary>
        /// <param name="z">Z component value of new vector.</param>
        /// <returns>Copy of this vector with Z component set as specified.</returns>
        public Vector3 WithZ(double z)
        {
            return new Vector3(X, Y, z);
        }

        /// <summary>
        ///     Returns array that contains vector components in order X, Y, Z.
        /// </summary>
        /// <returns>Array with vector components.</returns>
        public double[] ToArray()
        {
            return new[] {X, Y, Z};
        }

        /// <summary>
        ///     Returns a value indicating whether the value of this instance is equal to the value of the specified
        ///     <see cref="Vector3" /> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="other" /> parameter equals the value of this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Vector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an instance of <see cref="Vector3" /> and equals the value of this
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3 other && Equals(other);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
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

        /// <summary>
        ///     Converts the value of the current <see cref="Vector3" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Vector3" /> object.</returns>
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        }

        /// <summary>
        ///     Returns <see cref="Vector2" /> that represents this <see cref="Vector3" />. Returned <see cref="Vector2" /> has the
        ///     same X and Y while this <see cref="Vector3" /> Z is truncated.
        /// </summary>
        /// <returns><see cref="Vector2" /> that has the same X and Y to this <see cref="Vector3" />.</returns>
        [Pure]
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        ///     Returns <see cref="Vector4" /> that represents this <see cref="Vector3" />. Returned <see cref="Vector4" /> has the
        ///     same X, Y and Z while its W is set to zero.
        /// </summary>
        /// <returns><see cref="Vector4" /> that has the same X, Y and Z to this <see cref="Vector3" /> while its W is set to zero.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(X, Y, Z, 0);
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one vector to another.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector3 operator +(Vector3 left, Vector3 right)
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
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return left.Subtract(right);
        }

        /// <summary>
        ///     Multiplies specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector3" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Vector3 operator *(Vector3 left, double right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        ///     Divides specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="right">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector3" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Vector3 operator /(Vector3 left, double right)
        {
            return left.Divide(right);
        }

        /// <summary>
        ///     Returns vector opposite to the specified vector, that is vector with all components negated.
        /// </summary>
        /// <param name="right">Vector to be negated.</param>
        /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
        public static Vector3 operator -(Vector3 right)
        {
            return right.Opposite;
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector3" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Vector3" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector3" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Vector3" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}