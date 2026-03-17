using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     3D mathematical vector consisting of three components X, Y and Z. It is also used as a point in 3D space.
/// </summary>
public readonly struct Vector3 : IEquatable<Vector3>
{
    #region Static properties

    /// <summary>
    ///     Returns <see cref="Vector3" /> that has all components set to zero.
    /// </summary>
    public static Vector3 Zero => new(0, 0, 0);

    /// <summary>
    ///     Returns <see cref="Vector3" /> that has all components set to one.
    /// </summary>
    public static Vector3 One => new(1, 1, 1);

    /// <summary>
    ///     Gets the unit vector along the X axis (1,0,0).
    /// </summary>
    public static Vector3 UnitX => new(1, 0, 0);

    /// <summary>
    ///     Gets the unit vector along the Y axis (0,1,0).
    /// </summary>
    public static Vector3 UnitY => new(0, 1, 0);

    /// <summary>
    ///     Gets the unit vector along the Z axis (0,0,1).
    /// </summary>
    public static Vector3 UnitZ => new(0, 0, 1);

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
    ///     Length of <see cref="Vector3" /> squared. <see cref="LengthSquared" /> is cheaper to calculate than
    ///     <see cref="Length" />.
    /// </summary>
    public double LengthSquared => X * X + Y * Y + Z * Z;

    /// <summary>
    ///     Gets the normalized form of this vector (unit vector), which has the same direction
    ///     but a length of one.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Normalized vectors are commonly used when only direction matters, such as for movement directions,
    ///         surface normals, and various vector operations. Normalization preserves the direction while scaling
    ///         the magnitude to exactly one.
    ///     </para>
    ///     <para>
    ///         If this vector has near-zero length, this property returns <see cref="Zero" /> to avoid division by zero.
    ///         For axis-aligned unit vectors, consider using the static properties <see cref="UnitX" />,
    ///         <see cref="UnitY" />, and <see cref="UnitZ" /> instead, which provide better performance.
    ///     </para>
    /// </remarks>
    public Vector3 Unit => MathEx.IsNearZero(Length) ? Zero : new Vector3(X / Length, Y / Length, Z / Length);

    /// <summary>
    ///     Returns vector opposite to this vector, that is vector with all components negated.
    /// </summary>
    public Vector3 Opposite => new(-X, -Y, -Z);

    /// <summary>
    ///     Returns <see cref="Vector4" /> that is this <see cref="Vector3" /> in homogeneous coordinates.
    /// </summary>
    /// <remarks>
    ///     Homogeneous coordinates add additional component of value one therefore <see cref="Vector3" /> in homogeneous
    ///     coordinates is represented by <see cref="Vector4" /> with W component equal one.
    /// </remarks>
    public Vector4 Homogeneous => new(X, Y, Z, 1);

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

    #region Static methods

    /// <summary>
    ///     Linearly interpolates from <see cref="Vector3" /> <paramref name="v1" /> to <see cref="Vector3" />
    ///     <paramref name="v2" /> proportionally to factor <paramref name="alpha" />.
    /// </summary>
    /// <param name="v1">Source value for <see cref="Vector3" /> interpolation.</param>
    /// <param name="v2">Target value for <see cref="Vector3" /> interpolation.</param>
    /// <param name="alpha">Interpolation factor in range from <c>0.0</c> to <c>1.0</c>.</param>
    /// <returns>Interpolated value of <see cref="Vector3" />.</returns>
    /// <remarks>
    ///     When <paramref name="alpha" /> value is <c>0.0</c> the returned value is equal to <paramref name="v1" />. When
    ///     <paramref name="alpha" /> value is <c>1.0</c> the returned value is equal to <paramref name="v2" />.
    /// </remarks>
    public static Vector3 Lerp(in Vector3 v1, in Vector3 v2, double alpha) =>
        new(
            MathEx.Lerp(v1.X, v2.X, alpha),
            MathEx.Lerp(v1.Y, v2.Y, alpha),
            MathEx.Lerp(v1.Z, v2.Z, alpha)
        );

    /// <summary>
    ///     Returns the normalized form of the specified vector (unit vector), which has the same direction
    ///     but a length of one.
    /// </summary>
    /// <param name="vector">The vector to normalize.</param>
    /// <returns>
    ///     A <see cref="Vector3" /> with the same direction as <paramref name="vector" /> but with a length of one,
    ///     or <see cref="Zero" /> if <paramref name="vector" /> has near-zero length.
    /// </returns>
    /// <remarks>
    ///     This method is equivalent to accessing the <see cref="Unit" /> property of the vector. For axis-aligned
    ///     unit vectors, consider using <see cref="UnitX" />, <see cref="UnitY" />, or <see cref="UnitZ" /> instead
    ///     for better performance.
    /// </remarks>
    public static Vector3 Normalize(in Vector3 vector) => vector.Unit;

    #endregion

    #region Methods

    /// <summary>
    ///     Adds other vector to this vector.
    /// </summary>
    /// <param name="other">Other vector to add.</param>
    /// <returns><see cref="Vector3" /> that is sum of this vector with the other.</returns>
    public Vector3 Add(in Vector3 other) => new(X + other.X, Y + other.Y, Z + other.Z);

    /// <summary>
    ///     Subtracts other vector from this vector.
    /// </summary>
    /// <param name="other">Other vector to subtract.</param>
    /// <returns><see cref="Vector3" /> that is difference between this vector and the other.</returns>
    public Vector3 Subtract(in Vector3 other) => new(X - other.X, Y - other.Y, Z - other.Z);

    /// <summary>
    ///     Multiplies this vector by given scalar.
    /// </summary>
    /// <param name="scalar">Scalar value that is multiplier of vector.</param>
    /// <returns><see cref="Vector3" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
    public Vector3 Multiply(double scalar) => new(X * scalar, Y * scalar, Z * scalar);

    /// <summary>
    ///     Divides this vector by given scalar.
    /// </summary>
    /// <param name="scalar">Scalar value that is divisor of vector.</param>
    /// <returns><see cref="Vector3" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
    public Vector3 Divide(double scalar) => new(X / scalar, Y / scalar, Z / scalar);

    /// <summary>
    ///     Calculates dot product of this vector with the other.
    /// </summary>
    /// <param name="other">Other vector that is part of dot product calculation as a second parameter.</param>
    /// <returns>Dot product of this vector with the other.</returns>
    public double Dot(in Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;

    /// <summary>
    ///     Calculates distance between point represented by this <see cref="Vector3" /> and point represented by other
    ///     <see cref="Vector3" />.
    /// </summary>
    /// <param name="other">Other <see cref="Vector3" /> representing a point.</param>
    /// <returns>Distance between points.</returns>
    public double Distance(in Vector3 other) => Subtract(other).Length;

    /// <summary>
    ///     Calculates distance squared between point represented by this <see cref="Vector3" /> and point represented by other
    ///     <see cref="Vector3" />. <see cref="DistanceSquared" /> is cheaper to calculate than <see cref="Distance" />.
    /// </summary>
    /// <param name="other">Other <see cref="Vector3" /> representing a point.</param>
    /// <returns>Distance between points squared.</returns>
    public double DistanceSquared(in Vector3 other) => Subtract(other).LengthSquared;

    /// <summary>
    ///     Returns <see cref="Vector3" /> that has the same direction to this <see cref="Vector3" /> but is of given length.
    /// </summary>
    /// <param name="length">Length of returned <see cref="Vector3" />.</param>
    /// <returns><see cref="Vector3" /> of given length.</returns>
    public Vector3 OfLength(double length) => Unit * length;

    /// <summary>
    ///     Returns <see cref="Vector3" /> that has the same direction to this <see cref="Vector3" /> but is, at most, of given
    ///     length.
    /// </summary>
    /// <param name="maxLength">Maximum allowed length of returned <see cref="Vector3" />.</param>
    /// <returns>
    ///     Copy of this <see cref="Vector3" /> if its <see cref="Length" /> is lower or equal to given length or
    ///     <see cref="Vector3" /> of given length.
    /// </returns>
    public Vector3 Clamp(double maxLength) => Length > maxLength ? OfLength(maxLength) : this;

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
    public Vector3 Clamp(double minLength, double maxLength) => Length < minLength ? OfLength(minLength) : Clamp(maxLength);

    /// <summary>
    ///     Returns copy of this vector with X component set as specified.
    /// </summary>
    /// <param name="x">X component value of new vector.</param>
    /// <returns>Copy of this vector with X component set as specified.</returns>
    public Vector3 WithX(double x) => new(x, Y, Z);

    /// <summary>
    ///     Returns copy of this vector with Y component set as specified.
    /// </summary>
    /// <param name="y">Y component value of new vector.</param>
    /// <returns>Copy of this vector with Y component set as specified.</returns>
    public Vector3 WithY(double y) => new(X, y, Z);

    /// <summary>
    ///     Returns copy of this vector with Z component set as specified.
    /// </summary>
    /// <param name="z">Z component value of new vector.</param>
    /// <returns>Copy of this vector with Z component set as specified.</returns>
    public Vector3 WithZ(double z) => new(X, Y, z);

    /// <summary>
    ///     Returns array that contains vector components in order X, Y, Z.
    /// </summary>
    /// <returns>Array with vector components.</returns>
    public double[] ToArray()
    {
        return new[] { X, Y, Z };
    }

    /// <inheritdoc />
    public bool Equals(Vector3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Vector3 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    /// <summary>
    ///     Converts the value of the current <see cref="Vector3" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="Vector3" /> object.</returns>
    public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";

    /// <summary>
    ///     Returns <see cref="Vector2" /> that represents this <see cref="Vector3" />. Returned <see cref="Vector2" /> has the
    ///     same X and Y while this <see cref="Vector3" /> Z is truncated.
    /// </summary>
    /// <returns><see cref="Vector2" /> that has the same X and Y to this <see cref="Vector3" />.</returns>
    public Vector2 ToVector2() => new(X, Y);

    /// <summary>
    ///     Returns <see cref="Vector4" /> that represents this <see cref="Vector3" />. Returned <see cref="Vector4" /> has the
    ///     same X, Y and Z while its W is set to zero.
    /// </summary>
    /// <returns><see cref="Vector4" /> that has the same X, Y and Z to this <see cref="Vector3" /> while its W is set to zero.</returns>
    public Vector4 ToVector4() => new(X, Y, Z, 0);

    #endregion

    #region Operators

    /// <summary>
    ///     Adds one vector to another.
    /// </summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
    public static Vector3 operator +(in Vector3 left, in Vector3 right) => left.Add(right);

    /// <summary>
    ///     Subtracts one vector from another.
    /// </summary>
    /// <param name="left">Vector to subtract from (the minuend).</param>
    /// <param name="right">Vector to subtract (the subtrahend).</param>
    /// <returns>
    ///     An object that is the result of the value of <paramref name="left" /> minus the value of
    ///     <paramref name="right" />.
    /// </returns>
    public static Vector3 operator -(in Vector3 left, in Vector3 right) => left.Subtract(right);

    /// <summary>
    ///     Multiplies specified vector by given scalar.
    /// </summary>
    /// <param name="left">Vector to be multiplied.</param>
    /// <param name="right">Scalar value that is multiplier of vector.</param>
    /// <returns><see cref="Vector3" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
    public static Vector3 operator *(in Vector3 left, double right) => left.Multiply(right);

    /// <summary>
    ///     Divides specified vector by given scalar.
    /// </summary>
    /// <param name="left">Vector to be divided.</param>
    /// <param name="right">Scalar value that is divisor of vector.</param>
    /// <returns><see cref="Vector3" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
    public static Vector3 operator /(in Vector3 left, double right) => left.Divide(right);

    /// <summary>
    ///     Returns vector opposite to the specified vector, that is vector with all components negated.
    /// </summary>
    /// <param name="right">Vector to be negated.</param>
    /// <returns>Vector opposite to the specified vector, that is vector with all components negated.</returns>
    public static Vector3 operator -(in Vector3 right) => right.Opposite;

    /// <summary>
    ///     Determines whether two specified instances of <see cref="Vector3" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="Vector3" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(in Vector3 left, in Vector3 right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="Vector3" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="Vector3" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(in Vector3 left, in Vector3 right) => !left.Equals(right);

    #endregion
}