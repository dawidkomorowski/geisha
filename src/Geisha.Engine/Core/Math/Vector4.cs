using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     4D mathematical vector consisting of four components X, Y, Z and W. It is also used as a point in 3D space in
///     homogeneous coordinates or as a point in 4D space.
/// </summary>
public readonly struct Vector4 : IEquatable<Vector4>
{
    #region Static properties

    /// <summary>
    ///     Returns <see cref="Vector4" /> that has all components set to zero.
    /// </summary>
    public static Vector4 Zero => new(0, 0, 0, 0);

    /// <summary>
    ///     Returns <see cref="Vector4" /> that has all components set to one.
    /// </summary>
    public static Vector4 One => new(1, 1, 1, 1);

    /// <summary>
    ///     Returns unit <see cref="Vector4" /> directed along the X axis, that is vector (1,0,0,0).
    /// </summary>
    public static Vector4 UnitX => new(1, 0, 0, 0);

    /// <summary>
    ///     Returns unit <see cref="Vector4" /> directed along the Y axis, that is vector (0,1,0,0).
    /// </summary>
    public static Vector4 UnitY => new(0, 1, 0, 0);

    /// <summary>
    ///     Returns unit <see cref="Vector4" /> directed along the Z axis, that is vector (0,0,1,0).
    /// </summary>
    public static Vector4 UnitZ => new(0, 0, 1, 0);

    /// <summary>
    ///     Returns unit <see cref="Vector4" /> directed along the W axis, that is vector (0,0,0,1).
    /// </summary>
    public static Vector4 UnitW => new(0, 0, 0, 1);

    #endregion

    #region Properties

    /// <summary>
    ///     X component of <see cref="Vector4" />.
    /// </summary>
    public double X { get; }

    /// <summary>
    ///     Y component of <see cref="Vector4" />.
    /// </summary>
    public double Y { get; }

    /// <summary>
    ///     Z component of <see cref="Vector4" />.
    /// </summary>
    public double Z { get; }

    /// <summary>
    ///     W component of <see cref="Vector4" />.
    /// </summary>
    public double W { get; }

    /// <summary>
    ///     Length of <see cref="Vector4" />.
    /// </summary>
    public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

    /// <summary>
    ///     Returns unit vector out of this <see cref="Vector4" /> that is vector with the same direction but with length equal
    ///     one.
    /// </summary>
    /// <remarks>For vector with near zero length this property returns zero vector.</remarks>
    public Vector4 Unit => Length > double.Epsilon ? new Vector4(X / Length, Y / Length, Z / Length, W / Length) : Zero;

    /// <summary>
    ///     Returns vector opposite to this vector, that is vector with all components negated.
    /// </summary>
    public Vector4 Opposite => new(-X, -Y, -Z, -W);

    #endregion

    #region Constructors

    /// <summary>
    ///     Creates new instance of <see cref="Vector4" /> given X, Y, Z and W components values.
    /// </summary>
    /// <param name="x">X component value.</param>
    /// <param name="y">Y component value.</param>
    /// <param name="z">Z component value.</param>
    /// <param name="w">W component value.</param>
    public Vector4(double x, double y, double z, double w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    ///     Creates new instance of <see cref="Vector4" /> given array of size four containing X, Y, Z and W components values.
    /// </summary>
    /// <param name="array">Array of size four with X, Y, Z and W components values.</param>
    /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not four.</exception>
    public Vector4(IReadOnlyList<double> array)
    {
        if (array.Count != 4)
            throw new ArgumentException("Array must have length of 4 elements.");

        X = array[0];
        Y = array[1];
        Z = array[2];
        W = array[3];
    }

    #endregion

    #region Static methods

    /// <summary>
    ///     Linearly interpolates from <see cref="Vector4" /> <paramref name="v1" /> to <see cref="Vector4" />
    ///     <paramref name="v2" /> proportionally to factor <paramref name="alpha" />.
    /// </summary>
    /// <param name="v1">Source value for <see cref="Vector4" /> interpolation.</param>
    /// <param name="v2">Target value for <see cref="Vector4" /> interpolation.</param>
    /// <param name="alpha">Interpolation factor in range from <c>0.0</c> to <c>1.0</c>.</param>
    /// <returns>Interpolated value of <see cref="Vector4" />.</returns>
    /// <remarks>
    ///     When <paramref name="alpha" /> value is <c>0.0</c> the returned value is equal to <paramref name="v1" />. When
    ///     <paramref name="alpha" /> value is <c>1.0</c> the returned value is equal to <paramref name="v2" />.
    /// </remarks>
    public static Vector4 Lerp(in Vector4 v1, in Vector4 v2, double alpha) =>
        new(
            GMath.Lerp(v1.X, v2.X, alpha),
            GMath.Lerp(v1.Y, v2.Y, alpha),
            GMath.Lerp(v1.Z, v2.Z, alpha),
            GMath.Lerp(v1.W, v2.W, alpha)
        );

    #endregion

    #region Methods

    /// <summary>
    ///     Adds other vector to this vector.
    /// </summary>
    /// <param name="other">Other vector to add.</param>
    /// <returns><see cref="Vector4" /> that is sum of this vector with the other.</returns>
    public Vector4 Add(in Vector4 other) => new(X + other.X, Y + other.Y, Z + other.Z, W + other.W);

    /// <summary>
    ///     Subtracts other vector from this vector.
    /// </summary>
    /// <param name="other">Other vector to subtract.</param>
    /// <returns><see cref="Vector4" /> that is difference between this vector and the other.</returns>
    public Vector4 Subtract(in Vector4 other) => new(X - other.X, Y - other.Y, Z - other.Z, W - other.W);

    /// <summary>
    ///     Multiplies this vector by given scalar.
    /// </summary>
    /// <param name="scalar">Scalar value that is multiplier of vector.</param>
    /// <returns><see cref="Vector4" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
    public Vector4 Multiply(double scalar) => new(X * scalar, Y * scalar, Z * scalar, W * scalar);

    /// <summary>
    ///     Divides this vector by given scalar.
    /// </summary>
    /// <param name="scalar">Scalar value that is divisor of vector.</param>
    /// <returns><see cref="Vector4" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
    public Vector4 Divide(double scalar) => new(X / scalar, Y / scalar, Z / scalar, W / scalar);

    /// <summary>
    ///     Calculates dot product of this vector with the other.
    /// </summary>
    /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
    /// <returns>Dot product of this vector with the other.</returns>
    public double Dot(in Vector4 other) => X * other.X + Y * other.Y + Z * other.Z + W * other.W;

    /// <summary>
    ///     Calculates distance between point represented by this vector and point represented by other vector.
    /// </summary>
    /// <param name="other">Other vector representing a point.</param>
    /// <returns>Distance between points represented by this vector and the other.</returns>
    public double Distance(in Vector4 other) => Subtract(other).Length;

    /// <summary>
    ///     Returns <see cref="Vector4" /> that has the same direction to this <see cref="Vector4" /> but is of given length.
    /// </summary>
    /// <param name="length">Length of returned <see cref="Vector4" />.</param>
    /// <returns><see cref="Vector4" /> of given length.</returns>
    public Vector4 OfLength(double length) => Unit * length;

    /// <summary>
    ///     Returns <see cref="Vector4" /> that has the same direction to this <see cref="Vector4" /> but is, at most, of given
    ///     length.
    /// </summary>
    /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector4" />.</param>
    /// <returns>
    ///     Copy of this <see cref="Vector4" /> if its <see cref="Length" /> is lower or equal to given length or
    ///     <see cref="Vector4" /> of given length.
    /// </returns>
    public Vector4 Clamp(double maxLength) => Length > maxLength ? OfLength(maxLength) : this;

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
    public Vector4 Clamp(double minLength, double maxLength) => Length < minLength ? OfLength(minLength) : Clamp(maxLength);

    /// <summary>
    ///     Returns copy of this vector with X component set as specified.
    /// </summary>
    /// <param name="x">X component value of new vector.</param>
    /// <returns>Copy of this vector with X component set as specified.</returns>
    public Vector4 WithX(double x) => new(x, Y, Z, W);

    /// <summary>
    ///     Returns copy of this vector with Y component set as specified.
    /// </summary>
    /// <param name="y">Y component value of new vector.</param>
    /// <returns>Copy of this vector with Y component set as specified.</returns>
    public Vector4 WithY(double y) => new(X, y, Z, W);

    /// <summary>
    ///     Returns copy of this vector with Z component set as specified.
    /// </summary>
    /// <param name="z">Z component value of new vector.</param>
    /// <returns>Copy of this vector with Z component set as specified.</returns>
    public Vector4 WithZ(double z) => new(X, Y, z, W);

    /// <summary>
    ///     Returns copy of this vector with W component set as specified.
    /// </summary>
    /// <param name="w">W component value of new vector.</param>
    /// <returns>Copy of this vector with W component set as specified.</returns>
    public Vector4 WithW(double w) => new(X, Y, Z, w);

    /// <summary>
    ///     Returns array that contains vector components in order X, Y, Z, W.
    /// </summary>
    /// <returns>Array with vector components.</returns>
    public double[] ToArray()
    {
        return new[] { X, Y, Z, W };
    }

    /// <inheritdoc />
    public bool Equals(Vector4 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Vector4 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    /// <summary>
    ///     Converts the value of the current <see cref="Vector4" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="Vector4" /> object.</returns>
    public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";

    /// <summary>
    ///     Returns <see cref="Vector2" /> that represents this <see cref="Vector4" />. Returned <see cref="Vector2" /> has the
    ///     same X and Y while this <see cref="Vector4" /> Z and W are truncated.
    /// </summary>
    /// <returns><see cref="Vector2" /> that has the same X and Y to this <see cref="Vector4" />.</returns>
    public Vector2 ToVector2() => new(X, Y);

    /// <summary>
    ///     Returns <see cref="Vector3" /> that represents this <see cref="Vector4" />. Returned <see cref="Vector3" /> has the
    ///     same X, Y and Z while this <see cref="Vector4" /> W is truncated.
    /// </summary>
    /// <returns><see cref="Vector3" /> that has the same X, Y and Z to this <see cref="Vector4" />.</returns>
    public Vector3 ToVector3() => new(X, Y, Z);

    #endregion

    #region Operators

    /// <summary>
    ///     Adds one vector to another.
    /// </summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
    public static Vector4 operator +(in Vector4 left, in Vector4 right) => left.Add(right);

    /// <summary>
    ///     Subtracts one vector from another.
    /// </summary>
    /// <param name="left">Vector to subtract from (the minuend).</param>
    /// <param name="right">Vector to subtract (the subtrahend).</param>
    /// <returns>
    ///     An object that is the result of the value of <paramref name="left" /> minus the value of
    ///     <paramref name="right" />.
    /// </returns>
    public static Vector4 operator -(in Vector4 left, in Vector4 right) => left.Subtract(right);

    /// <summary>
    ///     Multiplies specified vector by given scalar.
    /// </summary>
    /// <param name="left">Vector to be multiplied.</param>
    /// <param name="right">Scalar value that is multiplier of vector.</param>
    /// <returns><see cref="Vector4" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
    public static Vector4 operator *(in Vector4 left, double right) => left.Multiply(right);

    /// <summary>
    ///     Divides specified vector by given scalar.
    /// </summary>
    /// <param name="left">Vector to be divided.</param>
    /// <param name="right">Scalar value that is divisor of vector.</param>
    /// <returns><see cref="Vector4" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
    public static Vector4 operator /(in Vector4 left, double right) => left.Divide(right);

    /// <summary>
    ///     Returns vector opposite to the specified vector, that is vector with all components negated.
    /// </summary>
    /// <param name="right">Vector to be negated.</param>
    /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
    public static Vector4 operator -(in Vector4 right) => right.Opposite;

    /// <summary>
    ///     Determines whether two specified instances of <see cref="Vector4" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="Vector4" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(in Vector4 left, in Vector4 right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="Vector4" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="Vector4" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(in Vector4 left, in Vector4 right) => !left.Equals(right);

    #endregion
}