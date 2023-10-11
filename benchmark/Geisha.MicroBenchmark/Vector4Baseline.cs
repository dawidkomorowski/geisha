using System;
using System.Collections.Generic;

namespace Geisha.MicroBenchmark
{
    /// <summary>
    ///     4D mathematical vector consisting of four components X, Y, Z and W. It is also used as a point in 3D space in
    ///     homogeneous coordinates or as a point in 4D space.
    /// </summary>
    public readonly struct Vector4Baseline : IEquatable<Vector4Baseline>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Vector4Baseline" /> that has all components set to zero.
        /// </summary>
        public static Vector4Baseline Zero => new(0, 0, 0, 0);

        /// <summary>
        ///     Returns <see cref="Vector4Baseline" /> that has all components set to one.
        /// </summary>
        public static Vector4Baseline One => new(1, 1, 1, 1);

        /// <summary>
        ///     Returns unit <see cref="Vector4Baseline" /> directed along the X axis, that is vector (1,0,0,0).
        /// </summary>
        public static Vector4Baseline UnitX => new(1, 0, 0, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector4Baseline" /> directed along the Y axis, that is vector (0,1,0,0).
        /// </summary>
        public static Vector4Baseline UnitY => new(0, 1, 0, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector4Baseline" /> directed along the Z axis, that is vector (0,0,1,0).
        /// </summary>
        public static Vector4Baseline UnitZ => new(0, 0, 1, 0);

        /// <summary>
        ///     Returns unit <see cref="Vector4Baseline" /> directed along the W axis, that is vector (0,0,0,1).
        /// </summary>
        public static Vector4Baseline UnitW => new(0, 0, 0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     X component of <see cref="Vector4Baseline" />.
        /// </summary>
        public double X { get; }

        /// <summary>
        ///     Y component of <see cref="Vector4Baseline" />.
        /// </summary>
        public double Y { get; }

        /// <summary>
        ///     Z component of <see cref="Vector4Baseline" />.
        /// </summary>
        public double Z { get; }

        /// <summary>
        ///     W component of <see cref="Vector4Baseline" />.
        /// </summary>
        public double W { get; }

        /// <summary>
        ///     Length of <see cref="Vector4Baseline" />.
        /// </summary>
        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

        /// <summary>
        ///     Returns unit vector out of this <see cref="Vector4Baseline" /> that is vector with the same direction but with length equal
        ///     one.
        /// </summary>
        /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
        public Vector4Baseline Unit => Length > double.Epsilon ? new Vector4Baseline(X / Length, Y / Length, Z / Length, W / Length) : Zero;

        /// <summary>
        ///     Returns vector opposite to this vector, that is vector with all components negated.
        /// </summary>
        public Vector4Baseline Opposite => new(-X, -Y, -Z, -W);

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Vector4Baseline" /> given X, Y, Z and W components values.
        /// </summary>
        /// <param name="x">X component value.</param>
        /// <param name="y">Y component value.</param>
        /// <param name="z">Z component value.</param>
        /// <param name="w">W component value.</param>
        public Vector4Baseline(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Vector4Baseline" /> given array of size four containing X, Y, Z and W components values.
        /// </summary>
        /// <param name="array">Array of size four with X, Y, Z and W components values.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not four.</exception>
        public Vector4Baseline(IReadOnlyList<double> array)
        {
            if (array.Count != 4)
                throw new ArgumentException("Array must have length of 4 elements.");

            X = array[0];
            Y = array[1];
            Z = array[2];
            W = array[3];
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds other vector to this vector.
        /// </summary>
        /// <param name="other">Other vector to add.</param>
        /// <returns><see cref="Vector4Baseline" /> that is sum of this vector with the other.</returns>
        public Vector4Baseline Add(Vector4Baseline other) => new(X + other.X, Y + other.Y, Z + other.Z, W + other.W);

        /// <summary>
        ///     Subtracts other vector from this vector.
        /// </summary>
        /// <param name="other">Other vector to subtract.</param>
        /// <returns><see cref="Vector4Baseline" /> that is difference between this vector and the other.</returns>
        public Vector4Baseline Subtract(Vector4Baseline other) => new(X - other.X, Y - other.Y, Z - other.Z, W - other.W);

        /// <summary>
        ///     Multiplies this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector4Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Vector4Baseline Multiply(double scalar) => new(X * scalar, Y * scalar, Z * scalar, W * scalar);

        /// <summary>
        ///     Divides this vector by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector4Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Vector4Baseline Divide(double scalar) => new(X / scalar, Y / scalar, Z / scalar, W / scalar);

        /// <summary>
        ///     Calculates dot product of this vector with the other.
        /// </summary>
        /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
        /// <returns>Dot product of this vector with the other.</returns>
        public double Dot(Vector4Baseline other) => X * other.X + Y * other.Y + Z * other.Z + W * other.W;

        /// <summary>
        ///     Calculates distance between point represented by this vector and point represented by other vector.
        /// </summary>
        /// <param name="other">Other vector representing a point.</param>
        /// <returns>Distance between points represented by this vector and the other.</returns>
        public double Distance(Vector4Baseline other) => Subtract(other).Length;

        /// <summary>
        ///     Returns <see cref="Vector4Baseline" /> that has the same direction to this <see cref="Vector4Baseline" /> but is of given length.
        /// </summary>
        /// <param name="length">Length of returned <see cref="Vector4Baseline" />.</param>
        /// <returns><see cref="Vector4Baseline" /> of given length.</returns>
        public Vector4Baseline OfLength(double length) => Unit * length;

        /// <summary>
        ///     Returns <see cref="Vector4Baseline" /> that has the same direction to this <see cref="Vector4Baseline" /> but is, at most, of given
        ///     length.
        /// </summary>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector4Baseline" />.</param>
        /// <returns>
        ///     Copy of this <see cref="Vector4Baseline" /> if its <see cref="Length" /> is lower or equal to given length or
        ///     <see cref="Vector4Baseline" /> of given length.
        /// </returns>
        public Vector4Baseline Clamp(double maxLength) => Length > maxLength ? OfLength(maxLength) : this;

        /// <summary>
        ///     Returns <see cref="Vector4Baseline" /> that has the same direction to this <see cref="Vector4Baseline" /> but is at least of given
        ///     <paramref name="minLength" /> and at most of given <paramref name="maxLength" />.
        /// </summary>
        /// <param name="minLength">Minimum allowed length of returned <see cref="Vector4Baseline" />.</param>
        /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector4Baseline" />.</param>
        /// <returns>
        ///     <see cref="Vector4Baseline" /> of <paramref name="minLength" /> when this <see cref="Vector4Baseline" />
        ///     <see cref="Length" /> is lower than <paramref name="minLength" />. <see cref="Vector4Baseline" /> of
        ///     <paramref name="maxLength" /> when this <see cref="Vector4Baseline" /> <see cref="Length" /> is greater than
        ///     <paramref name="maxLength" />. Copy of this <see cref="Vector4Baseline" /> if its <see cref="Length" /> is grater or equal
        ///     to <paramref name="minLength" /> and lower or equal to <paramref name="maxLength" />.
        /// </returns>
        public Vector4Baseline Clamp(double minLength, double maxLength) => Length < minLength ? OfLength(minLength) : Clamp(maxLength);

        /// <summary>
        ///     Returns copy of this vector with X component set as specified.
        /// </summary>
        /// <param name="x">X component value of new vector.</param>
        /// <returns>Copy of this vector with X component set as specified.</returns>
        public Vector4Baseline WithX(double x) => new(x, Y, Z, W);

        /// <summary>
        ///     Returns copy of this vector with Y component set as specified.
        /// </summary>
        /// <param name="y">Y component value of new vector.</param>
        /// <returns>Copy of this vector with Y component set as specified.</returns>
        public Vector4Baseline WithY(double y) => new(X, y, Z, W);

        /// <summary>
        ///     Returns copy of this vector with Z component set as specified.
        /// </summary>
        /// <param name="z">Z component value of new vector.</param>
        /// <returns>Copy of this vector with Z component set as specified.</returns>
        public Vector4Baseline WithZ(double z) => new(X, Y, z, W);

        /// <summary>
        ///     Returns copy of this vector with W component set as specified.
        /// </summary>
        /// <param name="w">W component value of new vector.</param>
        /// <returns>Copy of this vector with W component set as specified.</returns>
        public Vector4Baseline WithW(double w) => new(X, Y, Z, w);

        /// <summary>
        ///     Returns array that contains vector components in order X, Y, Z, W.
        /// </summary>
        /// <returns>Array with vector components.</returns>
        public double[] ToArray()
        {
            return new[] { X, Y, Z, W };
        }

        /// <inheritdoc />
        public bool Equals(Vector4Baseline other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Vector4Baseline other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        /// <summary>
        ///     Converts the value of the current <see cref="Vector4Baseline" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Vector4Baseline" /> object.</returns>
        public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one vector to another.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector4Baseline operator +(Vector4Baseline left, Vector4Baseline right) => left.Add(right);

        /// <summary>
        ///     Subtracts one vector from another.
        /// </summary>
        /// <param name="left">Vector to subtract from (the minuend).</param>
        /// <param name="right">Vector to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Vector4Baseline operator -(Vector4Baseline left, Vector4Baseline right) => left.Subtract(right);

        /// <summary>
        ///     Multiplies specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of vector.</param>
        /// <returns><see cref="Vector4Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Vector4Baseline operator *(Vector4Baseline left, double right) => left.Multiply(right);

        /// <summary>
        ///     Divides specified vector by given scalar.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="right">Scalar value that is divisor of vector.</param>
        /// <returns><see cref="Vector4Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Vector4Baseline operator /(Vector4Baseline left, double right) => left.Divide(right);

        /// <summary>
        ///     Returns vector opposite to the specified vector, that is vector with all components negated.
        /// </summary>
        /// <param name="right">Vector to be negated.</param>
        /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
        public static Vector4Baseline operator -(Vector4Baseline right) => right.Opposite;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector4Baseline" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Vector4Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Vector4Baseline left, Vector4Baseline right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Vector4Baseline" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Vector4Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Vector4Baseline left, Vector4Baseline right) => !left.Equals(right);

        #endregion
    }
}