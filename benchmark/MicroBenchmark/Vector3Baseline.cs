using System;
using System.Collections.Generic;

namespace MicroBenchmark
{
    /// <summary>
    ///     3D mathematical vector consisting of three components X, Y and Z. It is also used as a point in 3D space.
    /// </summary>
    public readonly struct Vector3Baseline : IEquatable<Vector3Baseline>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Vector3Baseline" /> that has all components set to zero.
        /// </summary>
        public static Vector3Baseline Zero => new(0, 0, 0);

        /// <summary>
        ///     Returns <see cref="Vector3Baseline" /> that has all components set to one.
        /// </summary>
        public static Vector3Baseline One => new(1, 1, 1);

        /// <summary>
        ///     Returns unit <see cref="Vector3Baseline" /> directed along the X axis, that is vector (1,0,0).
        /// </summary>
        public static Vector3Baseline UnitX => new(1, 0, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector3Baseline" /> directed along the Y axis, that is vector (0,1,0).
        /// </summary>
        public static Vector3Baseline UnitY => new(0, 1, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector3Baseline" /> directed along the Z axis, that is vector (0,0,1).
        /// </summary>
        public static Vector3Baseline UnitZ => new(0, 0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     X component of <see cref="Vector3Baseline" />.
        /// </summary>
        public double X { get; }

        /// <summary>
        ///     Y component of <see cref="Vector3Baseline" />.
        /// </summary>
        public double Y { get; }

        /// <summary>
        ///     Z component of <see cref="Vector3Baseline" />.
        /// </summary>
        public double Z { get; }

        /// <summary>
        ///     Length of <see cref="Vector3Baseline" />.
        /// </summary>
        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        ///     Returns unit vector out of this <see cref="Vector3Baseline" /> that is vector with the same direction but with length equal
        ///     one.
        /// </summary>
        /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
        public Vector3Baseline Unit => Length > double.Epsilon ? new Vector3Baseline(X / Length, Y / Length, Z / Length) : Zero;

        /// <summary>
        ///     Returns vector opposite to this vector, that is vector with all components negated.
        /// </summary>
        public Vector3Baseline Opposite => new(-X, -Y, -Z);

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Vector3Baseline" /> given X, Y and Z components values.
        /// </summary>
        /// <param name="x">X component value.</param>
        /// <param name="y">Y component value.</param>
        /// <param name="z">Z component value.</param>
        public Vector3Baseline(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Vector3Baseline" /> given array of size three containing X, Y and Z components values.
        /// </summary>
        /// <param name="array">Array of size three with X, Y and Z components values.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not three.</exception>
        public Vector3Baseline(IReadOnlyList<double> array)
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
        /// <returns><see cref="Vector3Baseline" /> that is sum of this vector with the other.</returns>
        public Vector3Baseline Add(Vector3Baseline other) => new(X + other.X, Y + other.Y, Z + other.Z);

        /// <summary>
        ///     Subtracts other vector from this vector.
        /// </summary>
        /// <param name="other">Other vector to subtract.</param>
        /// <returns><see cref="Vector3Baseline" /> that is difference between this vector and the other.</returns>
        public Vector3Baseline Subtract(Vector3Baseline other) => new(X - other.X, Y - other.Y, Z - other.Z);

        /// <summary>
        ///     Multiplies this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector3Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Vector3Baseline Multiply(double scalar) => new(X * scalar, Y * scalar, Z * scalar);

        /// <summary>
        ///     Divides this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector3Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Vector3Baseline Divide(double scalar) => new(X / scalar, Y / scalar, Z / scalar);

        /// <summary>
        ///     Calculates dot product of this vector with the other.
        /// </summary>
        /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
        /// <returns>Dot product of this vector with the other.</returns>
        public double Dot(Vector3Baseline other) => X * other.X + Y * other.Y + Z * other.Z;

        /// <summary>
        ///     Calculates distance between point represented by this vector and point represented by other vector.
        /// </summary>
        /// <param name="other">Other vector representing a point.</param>
        /// <returns>Distance between points represented by this vector and the other.</returns>
        public double Distance(Vector3Baseline other) => Subtract(other).Length;

        /// <summary>
        ///     Returns <see cref="Vector3Baseline" /> that has the same direction to this <see cref="Vector3Baseline" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector3Baseline" />.</param>
        /// <returns><see cref="Vector3Baseline" /> of given length.</returns>
        public Vector3Baseline OfLength(double length) => Unit * length;

        /// <summary>
        ///     Returns <see cref="Vector3Baseline" /> that has the same direction to this <see cref="Vector3Baseline" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector3Baseline" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector3Baseline" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector3Baseline" /> of given length.
        /// </returns>
        public Vector3Baseline Clamp(double maxLength) => Length > maxLength ? OfLength(maxLength) : this;

        /// <summary>
        ///     Returns <see cref="Vector3Baseline" /> that has the same direction to this <see cref="Vector3Baseline" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of given <paramref name="maxLength" />.
        /// </summary>
        /// <param name="minLength">Minimum allowed length of returned <see cref="Vector3Baseline" />.</param>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector3Baseline" />.</param>
        /// <returns>
        ///     <see cref="Vector3Baseline" /> of <paramref name="minLength" /> when this <see cref="Vector3Baseline" />
        ///     <see cref="Length" /> is lower than <paramref name="minLength" />. <see cref="Vector3Baseline" /> of
        ///     <paramref name="maxLength" /> when this <see cref="Vector3Baseline" /> <see cref="Length" /> is greater than
        ///     <paramref name="maxLength" />. Copy of this <see cref="Vector3Baseline" /> if its <see cref="Length" /> is grater or equal
        ///     to <paramref name="minLength" /> and lower or equal to <paramref name="maxLength" />.
        /// </returns>
        public Vector3Baseline Clamp(double minLength, double maxLength) => Length < minLength ? OfLength(minLength) : Clamp(maxLength);

        /// <summary>
        ///     Returns copy of this vector with X component set as specified.
        /// </summary>
        /// <param name="x">X component value of new vector.</param>
        /// <returns>Copy of this vector with X component set as specified.</returns>
        public Vector3Baseline WithX(double x) => new(x, Y, Z);

        /// <summary>
        ///     Returns copy of this vector with Y component set as specified.
        /// </summary>
        /// <param name="y">Y component value of new vector.</param>
        /// <returns>Copy of this vector with Y component set as specified.</returns>
        public Vector3Baseline WithY(double y) => new(X, y, Z);

        /// <summary>
        ///     Returns copy of this vector with Z component set as specified.
        /// </summary>
        /// <param name="z">Z component value of new vector.</param>
        /// <returns>Copy of this vector with Z component set as specified.</returns>
        public Vector3Baseline WithZ(double z) => new(X, Y, z);

        /// <summary>
        ///     Returns array that contains vector components in order X, Y, Z.
        /// </summary>
        /// <returns>Array with vector components.</returns>
        public double[] ToArray()
        {
            return new[] { X, Y, Z };
        }

        /// <inheritdoc />
        public bool Equals(Vector3Baseline other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Vector3Baseline other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        /// <summary>
        ///     Converts the value of the current <see cref="Vector3Baseline" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Vector3Baseline" /> object.</returns>
        public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one vector to another.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector3Baseline operator +(Vector3Baseline left, Vector3Baseline right) => left.Add(right);

        /// <summary>
        ///     Subtracts one vector from another.
        /// </summary>
        /// <param name="left">Vector to subtract from (the minuend).</param>
        /// <param name="right">Vector to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Vector3Baseline operator -(Vector3Baseline left, Vector3Baseline right) => left.Subtract(right);

        /// <summary>
        ///     Multiplies specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector3Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Vector3Baseline operator *(Vector3Baseline left, double right) => left.Multiply(right);

        /// <summary>
        ///     Divides specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="right">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector3Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Vector3Baseline operator /(Vector3Baseline left, double right) => left.Divide(right);

        /// <summary>
        ///     Returns vector opposite to the specified vector, that is vector with all components negated.
        /// </summary>
        /// <param name="right">Vector to be negated.</param>
        /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
        public static Vector3Baseline operator -(Vector3Baseline right) => right.Opposite;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector3Baseline" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Vector3Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Vector3Baseline left, Vector3Baseline right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector3Baseline" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Vector3Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Vector3Baseline left, Vector3Baseline right) => !left.Equals(right);

        #endregion
    }
}