using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     2D mathematical vector consisting of two components X and Y. It is also used as a point in 2D space.
    /// </summary>
    public readonly struct Vector2 : IEquatable<Vector2>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has all components set to zero.
        /// </summary>
        public static Vector2 Zero => new(0, 0);

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has all components set to one.
        /// </summary>
        public static Vector2 One => new(1, 1);

        /// <summary>
        ///     Returns unit <see cref="Vector2" /> directed along the X axis, that is vector (1,0).
        /// </summary>
        public static Vector2 UnitX => new(1, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector2" /> directed along the Y axis, that is vector (0,1).
        /// </summary>
        public static Vector2 UnitY => new(0, 1);

        #endregion

        #region Properties

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

        /// <summary>
        ///     Returns unit vector out of this <see cref="Vector2" /> that is vector with the same direction but with length equal
        ///     one.
        /// </summary>
        /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
        public Vector2 Unit => Length > double.Epsilon ? new Vector2(X / Length, Y / Length) : Zero;

        /// <summary>
        ///     Returns vector opposite to this vector, that is vector with all components negated.
        /// </summary>
        public Vector2 Opposite => new(-X, -Y);

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
        public Vector3 Homogeneous => new(X, Y, 1);

        #endregion

        #region Constructors

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
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not two.</exception>
        public Vector2(IReadOnlyList<double> array)
        {
            if (array.Count != 2)
                throw new ArgumentException("Array must have length of 2 elements.");

            X = array[0];
            Y = array[1];
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Linearly interpolates from <see cref="Vector2" /> <paramref name="v1" /> to <see cref="Vector2" />
        ///     <paramref name="v2" /> proportionally to factor <paramref name="alpha" />.
        /// </summary>
        /// <param name="v1">Source value for <see cref="Vector2" /> interpolation.</param>
        /// <param name="v2">Target value for <see cref="Vector2" /> interpolation.</param>
        /// <param name="alpha">Interpolation factor in range from <c>0.0</c> to <c>1.0</c>.</param>
        /// <returns>Interpolated value of <see cref="Vector2" />.</returns>
        /// <remarks>
        ///     When <paramref name="alpha" /> value is <c>0.0</c> the returned value is equal to <paramref name="v1" />. When
        ///     <paramref name="alpha" /> value is <c>1.0</c> the returned value is equal to <paramref name="v2" />.
        /// </remarks>
        public static Vector2 Lerp(in Vector2 v1, in Vector2 v2, double alpha) =>
            new(
                GMath.Lerp(v1.X, v2.X, alpha),
                GMath.Lerp(v1.Y, v2.Y, alpha)
            );

        /// <summary>
        ///     Returns the <see cref="Vector2" /> that X and Y components are maximum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </summary>
        /// <param name="v1">First <see cref="Vector2" />.</param>
        /// <param name="v2">Second <see cref="Vector2" />.</param>
        /// <returns>
        ///     <see cref="Vector2" /> that X and Y components are maximum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </returns>
        /// <remarks>For vector (10, 5) and vector (8, 6) the maximum is vector (10, 6).</remarks>
        public static Vector2 Max(in Vector2 v1, in Vector2 v2) => new(System.Math.Max(v1.X, v2.X), System.Math.Max(v1.Y, v2.Y));

        /// <summary>
        ///     Returns the <see cref="Vector2" /> that X and Y components are minimum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </summary>
        /// <param name="v1">First <see cref="Vector2" />.</param>
        /// <param name="v2">Second <see cref="Vector2" />.</param>
        /// <returns>
        ///     <see cref="Vector2" /> that X and Y components are minimum of corresponding X and Y components of
        ///     <paramref name="v1" /> and <paramref name="v2" />.
        /// </returns>
        /// <remarks>For vector (10, 5) and vector (8, 6) the minimum is vector (8, 5).</remarks>
        public static Vector2 Min(in Vector2 v1, in Vector2 v2) => new(System.Math.Min(v1.X, v2.X), System.Math.Min(v1.Y, v2.Y));

        #endregion

        #region Methods

        /// <summary>
        ///     Adds other vector to this vector.
        /// </summary>
        /// <param name="other">Other vector to add.</param>
        /// <returns><see cref="Vector2" /> that is sum of this vector with the other.</returns>
        public Vector2 Add(in Vector2 other) => new(X + other.X, Y + other.Y);

        /// <summary>
        ///     Subtracts other vector from this vector.
        /// </summary>
        /// <param name="other">Other vector to subtract.</param>
        /// <returns><see cref="Vector2" /> that is difference between this vector and the other.</returns>
        public Vector2 Subtract(in Vector2 other) => new(X - other.X, Y - other.Y);

        /// <summary>
        ///     Multiplies this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector2" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Vector2 Multiply(double scalar) => new(X * scalar, Y * scalar);

        /// <summary>
        ///     Divides this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector2" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Vector2 Divide(double scalar) => new(X / scalar, Y / scalar);

        /// <summary>
        ///     Calculates dot product of this vector with the other.
        /// </summary>
        /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
        /// <returns>Dot product of this vector with the other.</returns>
        public double Dot(in Vector2 other) => X * other.X + Y * other.Y;

        // TODO Add documentation and tests.
        public double Cross(in Vector2 other) => X * other.Y - Y * other.X;

        /// <summary>
        ///     Calculates distance between point represented by this vector and point represented by other vector.
        /// </summary>
        /// <param name="other">Other vector representing a point.</param>
        /// <returns>Distance between points represented by this vector and the other.</returns>
        public double Distance(in Vector2 other) => Subtract(other).Length;

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has the same direction to this <see cref="Vector2" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector2" />.</param>
        /// <returns><see cref="Vector2" /> of given length.</returns>
        public Vector2 OfLength(double length) => Unit * length;

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has the same direction to this <see cref="Vector2" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector2" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector2" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector2" /> of given length.
        /// </returns>
        public Vector2 Clamp(double maxLength) => Length > maxLength ? OfLength(maxLength) : this;

        /// <summary>
        ///     Returns <see cref="Vector2" /> that has the same direction to this <see cref="Vector2" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of the given <paramref name="maxLength" />.
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
        public Vector2 Clamp(double minLength, double maxLength) => Length < minLength ? OfLength(minLength) : Clamp(maxLength);

        /// <summary>
        ///     Computes midpoint between this <see cref="Vector2" /> point and other <see cref="Vector2" /> point.
        /// </summary>
        /// <param name="other">Other <see cref="Vector2" /> representing a point.</param>
        /// <returns><see cref="Vector2" /> representing the midpoint.</returns>
        public Vector2 Midpoint(in Vector2 other) => (this + other) * 0.5;

        // TODO Add documentation and tests.
        // TODO Consider angle computed with Atan2 (this implementation may want to use Cross product of vectors).
        // TODO Especially test these cases that can produce NaN
        // Case1
        //double v1x = -0.8012515883831227;
        //double v1y = 0.5983275792686839;
        //double v2x = -0.8012515883831226;
        //double v2y = 0.5983275792686837;
        // Case2
        //double v1x = -0.8788150639679754;
        //double v1y = 0.4771625334652371;
        //double v2x = -0.8788150639679754;
        //double v2y = 0.4771625334652371;
        public double Angle(in Vector2 other) => System.Math.Acos(System.Math.Clamp(Dot(other) / (Length * other.Length), -1, 1));

        // TODO Introduce LengthSquared and DistanceSquared for increased performance.
        // TODO Then some existing code could be probably optimized.
        // TODO Angle could be optimized to sqrt(LengthSquared * other.LengthSquared) instead of (Length * other.Length).

        /// <summary>
        ///     Returns copy of this vector with X component set as specified.
        /// </summary>
        /// <param name="x">X component value of new vector.</param>
        /// <returns>Copy of this vector with X component set as specified.</returns>
        public Vector2 WithX(double x) => new(x, Y);

        /// <summary>
        ///     Returns copy of this vector with Y component set as specified.
        /// </summary>
        /// <param name="y">Y component value of new vector.</param>
        /// <returns>Copy of this vector with Y component set as specified.</returns>
        public Vector2 WithY(double y) => new(X, y);

        /// <summary>
        ///     Returns array that contains vector components in order X, Y.
        /// </summary>
        /// <returns>Array with vector components.</returns>
        public double[] ToArray() => new[] { X, Y };

        /// <inheritdoc />
        public bool Equals(Vector2 other) => X.Equals(other.X) && Y.Equals(other.Y);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Vector2 other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        ///     Converts the value of the current <see cref="Vector2" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Vector2" /> object.</returns>
        public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";

        /// <summary>
        ///     Returns <see cref="Vector3" /> that represents this <see cref="Vector2" />. Returned <see cref="Vector3" /> has the
        ///     same X and Y while its Z is set to zero.
        /// </summary>
        /// <returns><see cref="Vector3" /> that has the same X and Y to this <see cref="Vector2" /> while its Z is set to zero.</returns>
        public Vector3 ToVector3() => new(X, Y, 0);

        /// <summary>
        ///     Returns <see cref="Vector4" /> that represents this <see cref="Vector2" />. Returned <see cref="Vector4" /> has the
        ///     same X and Y while its Z and W are set to zero.
        /// </summary>
        /// <returns>
        ///     <see cref="Vector4" /> that has the same X and Y to this <see cref="Vector2" /> while its Z and W are set to
        ///     zero.
        /// </returns>
        public Vector4 ToVector4() => new(X, Y, 0, 0);

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one vector to another.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector2 operator +(in Vector2 left, in Vector2 right) => left.Add(right);

        /// <summary>
        ///     Subtracts one vector from another.
        /// </summary>
        /// <param name="left">Vector to subtract from (the minuend).</param>
        /// <param name="right">Vector to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Vector2 operator -(in Vector2 left, in Vector2 right) => left.Subtract(right);

        /// <summary>
        ///     Multiplies specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector2" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Vector2 operator *(in Vector2 left, double right) => left.Multiply(right);

        /// <summary>
        ///     Divides specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="right">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector2" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Vector2 operator /(in Vector2 left, double right) => left.Divide(right);

        /// <summary>
        ///     Returns vector opposite to the specified vector, that is vector with all components negated.
        /// </summary>
        /// <param name="right">Vector to be negated.</param>
        /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
        public static Vector2 operator -(in Vector2 right) => right.Opposite;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector2" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Vector2" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in Vector2 left, in Vector2 right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector2" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Vector2" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in Vector2 left, in Vector2 right) => !left.Equals(right);

        #endregion
    }
}