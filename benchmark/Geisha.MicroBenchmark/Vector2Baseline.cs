using System;
using System.Collections.Generic;

namespace Geisha.MicroBenchmark
{
    /// <summary>
    ///     2D mathematical vector consisting of two components X and Y. It is also used as a point in 2D space.
    /// </summary>
    public readonly struct Vector2Baseline : IEquatable<Vector2Baseline>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Vector2Baseline" /> that has all components set to zero.
        /// </summary>
        public static Vector2Baseline Zero => new Vector2Baseline(0, 0);

        /// <summary>
        ///     Returns <see cref="Vector2Baseline" /> that has all components set to one.
        /// </summary>
        public static Vector2Baseline One => new Vector2Baseline(1, 1);

        /// <summary>
        ///     Returns unit <see cref="Vector2Baseline" /> directed along the X axis, that is vector (1,0).
        /// </summary>
        public static Vector2Baseline UnitX => new Vector2Baseline(1, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector2Baseline" /> directed along the Y axis, that is vector (0,1).
        /// </summary>
        public static Vector2Baseline UnitY => new Vector2Baseline(0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     X component of <see cref="Vector2Baseline" />.
        /// </summary>
        public double X { get; }

        /// <summary>
        ///     Y component of <see cref="Vector2Baseline" />.
        /// </summary>
        public double Y { get; }

        /// <summary>
        ///     Length of <see cref="Vector2Baseline" />.
        /// </summary>
        public double Length => System.Math.Sqrt(X * X + Y * Y);

        /// <summary>
        ///     Returns unit vector out of this <see cref="Vector2Baseline" /> that is vector with the same direction but with length equal
        ///     one.
        /// </summary>
        /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
        public Vector2Baseline Unit => Length > double.Epsilon ? new Vector2Baseline(X / Length, Y / Length) : Zero;

        /// <summary>
        ///     Returns vector opposite to this vector, that is vector with all components negated.
        /// </summary>
        public Vector2Baseline Opposite => new Vector2Baseline(-X, -Y);

        /// <summary>
        ///     Returns normal (perpendicular unit vector) <see cref="Vector2Baseline" /> rotated 90 degrees counterclockwise.
        /// </summary>
        public Vector2Baseline Normal => new Vector2Baseline(-Y, X).Unit;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Vector2Baseline" /> given X and Y components values.
        /// </summary>
        /// <param name="x">X component value.</param>
        /// <param name="y">Y component value.</param>
        public Vector2Baseline(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Vector2Baseline" /> given array of size two containing X and Y components values.
        /// </summary>
        /// <param name="array">Array of size two with X and Y components values.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not two.</exception>
        public Vector2Baseline(IReadOnlyList<double> array)
        {
            if (array.Count != 2)
                throw new ArgumentException("Array must have length of 2 elements.");

            X = array[0];
            Y = array[1];
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Returns the <see cref="Vector2Baseline" /> that X and Y components are maximum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </summary>
        /// <param name="v1">First <see cref="Vector2Baseline" />.</param>
        /// <param name="v2">Second <see cref="Vector2Baseline" />.</param>
        /// <returns>
        ///     <see cref="Vector2Baseline" /> that X and Y components are maximum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </returns>
        /// <remarks>For vector (10, 5) and vector (8, 6) the maximum is vector (10, 6).</remarks>
        public static Vector2Baseline Max(in Vector2Baseline v1, in Vector2Baseline v2) => new Vector2Baseline(System.Math.Max(v1.X, v2.X), System.Math.Max(v1.Y, v2.Y));

        /// <summary>
        ///     Returns the <see cref="Vector2Baseline" /> that X and Y components are minimum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </summary>
        /// <param name="v1">First <see cref="Vector2Baseline" />.</param>
        /// <param name="v2">Second <see cref="Vector2Baseline" />.</param>
        /// <returns>
        ///     <see cref="Vector2Baseline" /> that X and Y components are minimum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </returns>
        /// <remarks>For vector (10, 5) and vector (8, 6) the minimum is vector (8, 5).</remarks>
        public static Vector2Baseline Min(in Vector2Baseline v1, in Vector2Baseline v2) => new Vector2Baseline(System.Math.Min(v1.X, v2.X), System.Math.Min(v1.Y, v2.Y));

        #endregion

        #region Methods

        /// <summary>
        ///     Adds other vector to this vector.
        /// </summary>
        /// <param name="other">Other vector to add.</param>
        /// <returns><see cref="Vector2Baseline" /> that is sum of this vector with the other.</returns>
        public Vector2Baseline Add(Vector2Baseline other) => new(X + other.X, Y + other.Y);

        /// <summary>
        ///     Subtracts other vector from this vector.
        /// </summary>
        /// <param name="other">Other vector to subtract.</param>
        /// <returns><see cref="Vector2Baseline" /> that is difference between this vector and the other.</returns>
        public Vector2Baseline Subtract(Vector2Baseline other) => new(X - other.X, Y - other.Y);

        /// <summary>
        ///     Multiplies this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector2Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Vector2Baseline Multiply(double scalar) => new(X * scalar, Y * scalar);

        /// <summary>
        ///     Divides this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector2Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Vector2Baseline Divide(double scalar) => new(X / scalar, Y / scalar);

        /// <summary>
        ///     Calculates dot product of this vector with the other.
        /// </summary>
        /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
        /// <returns>Dot product of this vector with the other.</returns>
        public double Dot(Vector2Baseline other) => X * other.X + Y * other.Y;

        /// <summary>
        ///     Calculates distance between point represented by this vector and point represented by other vector.
        /// </summary>
        /// <param name="other">Other vector representing a point.</param>
        /// <returns>Distance between points represented by this vector and the other.</returns>
        public double Distance(Vector2Baseline other) => Subtract(other).Length;

        /// <summary>
        ///     Returns <see cref="Vector2Baseline" /> that has the same direction to this <see cref="Vector2Baseline" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector2Baseline" />.</param>
        /// <returns><see cref="Vector2Baseline" /> of given length.</returns>
        public Vector2Baseline OfLength(double length) => Unit * length;

        /// <summary>
        ///     Returns <see cref="Vector2Baseline" /> that has the same direction to this <see cref="Vector2Baseline" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector2Baseline" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector2Baseline" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector2Baseline" /> of given length.
        /// </returns>
        public Vector2Baseline Clamp(double maxLength) => Length > maxLength ? OfLength(maxLength) : this;

        /// <summary>
        ///     Returns <see cref="Vector2Baseline" /> that has the same direction to this <see cref="Vector2Baseline" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of given <paramref name="maxLength" />.
        /// </summary>
        /// <param name="minLength">Minimum allowed length of returned <see cref="Vector2Baseline" />.</param>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector2Baseline" />.</param>
        /// <returns>
        ///     <see cref="Vector2Baseline" /> of <paramref name="minLength" /> when this <see cref="Vector2Baseline" />
        ///     <see cref="Length" /> is lower than <paramref name="minLength" />. <see cref="Vector2Baseline" /> of
        ///     <paramref name="maxLength" /> when this <see cref="Vector2Baseline" /> <see cref="Length" /> is greater than
        ///     <paramref name="maxLength" />. Copy of this <see cref="Vector2Baseline" /> if its <see cref="Length" /> is grater or equal
        ///     to <paramref name="minLength" /> and lower or equal to <paramref name="maxLength" />.
        /// </returns>
        public Vector2Baseline Clamp(double minLength, double maxLength) => Length < minLength ? OfLength(minLength) : Clamp(maxLength);

        /// <summary>
        ///     Returns copy of this vector with X component set as specified.
        /// </summary>
        /// <param name="x">X component value of new vector.</param>
        /// <returns>Copy of this vector with X component set as specified.</returns>
        public Vector2Baseline WithX(double x) => new(x, Y);

        /// <summary>
        ///     Returns copy of this vector with Y component set as specified.
        /// </summary>
        /// <param name="y">Y component value of new vector.</param>
        /// <returns>Copy of this vector with Y component set as specified.</returns>
        public Vector2Baseline WithY(double y) => new(X, y);

        /// <summary>
        ///     Returns array that contains vector components in order X, Y.
        /// </summary>
        /// <returns>Array with vector components.</returns>
        public double[] ToArray() => new[] { X, Y };

        /// <inheritdoc />
        public bool Equals(Vector2Baseline other) => X.Equals(other.X) && Y.Equals(other.Y);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Vector2Baseline other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        ///     Converts the value of the current <see cref="Vector2Baseline" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Vector2Baseline" /> object.</returns>
        public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one vector to another.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector2Baseline operator +(Vector2Baseline left, Vector2Baseline right) => left.Add(right);

        /// <summary>
        ///     Subtracts one vector from another.
        /// </summary>
        /// <param name="left">Vector to subtract from (the minuend).</param>
        /// <param name="right">Vector to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Vector2Baseline operator -(Vector2Baseline left, Vector2Baseline right) => left.Subtract(right);

        /// <summary>
        ///     Multiplies specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector2Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Vector2Baseline operator *(Vector2Baseline left, double right) => left.Multiply(right);

        /// <summary>
        ///     Divides specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="right">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector2Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Vector2Baseline operator /(Vector2Baseline left, double right) => left.Divide(right);

        /// <summary>
        ///     Returns vector opposite to the specified vector, that is vector with all components negated.
        /// </summary>
        /// <param name="right">Vector to be negated.</param>
        /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
        public static Vector2Baseline operator -(Vector2Baseline right) => right.Opposite;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector2Baseline" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Vector2Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Vector2Baseline left, Vector2Baseline right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector2Baseline" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Vector2Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Vector2Baseline left, Vector2Baseline right) => !left.Equals(right);

        #endregion
    }
}