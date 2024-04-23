using System;
using System.Diagnostics;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents vector of minimum translation of an object, that separates this object from the other overlapping one.
///     This is mainly used for collision resolution.
/// </summary>
public readonly struct MinimumTranslationVector : IEquatable<MinimumTranslationVector>
{
    /// <summary>
    ///     Creates new instance of <see cref="MinimumTranslationVector" /> with specified <paramref name="direction" /> and
    ///     <paramref name="length" />.
    /// </summary>
    /// <param name="direction">
    ///     Direction of <see cref="MinimumTranslationVector" />. The expected value is either zero vector or unit vector.
    /// </param>
    /// <param name="length">Length of <see cref="MinimumTranslationVector" />.</param>
    public MinimumTranslationVector(Vector2 direction, double length)
    {
        Debug.Assert(GMath.AlmostEqual(direction.Length, 1d) || GMath.AlmostEqual(direction.Length, 0d),
            "GMath.AlmostEqual(direction.Length, 1d) || GMath.AlmostEqual(direction.Length, 0d)");

        Direction = direction;
        Length = length;
    }

    /// <summary>
    ///     Creates new instance of <see cref="MinimumTranslationVector" /> out of specified <see cref="Vector2" />.
    /// </summary>
    /// <param name="mtv"><see cref="Vector2" /> representing <see cref="MinimumTranslationVector" />.</param>
    public MinimumTranslationVector(Vector2 mtv)
    {
        Direction = mtv.Unit;
        Length = mtv.Length;
    }

    /// <summary>
    ///     Gets a vector representing direction of <see cref="MinimumTranslationVector" />. The value is either zero vector or
    ///     unit vector.
    /// </summary>
    public Vector2 Direction { get; }

    /// <summary>
    ///     Gets length of <see cref="MinimumTranslationVector" />.
    /// </summary>
    public double Length { get; }

    /// <summary>
    ///     Converts the value of the current <see cref="MinimumTranslationVector" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="MinimumTranslationVector" /> object.</returns>
    public override string ToString()
    {
        return $"{nameof(Direction)}: {Direction}, {nameof(Length)}: {Length}";
    }

    /// <inheritdoc />
    public bool Equals(MinimumTranslationVector other) => Direction.Equals(other.Direction) && Length.Equals(other.Length);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is MinimumTranslationVector other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Direction, Length);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="MinimumTranslationVector" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="MinimumTranslationVector" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(MinimumTranslationVector left, MinimumTranslationVector right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="MinimumTranslationVector" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="MinimumTranslationVector" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(MinimumTranslationVector left, MinimumTranslationVector right) => !left.Equals(right);
}

// TODO Replace with MTV.
public readonly struct SeparationInfo
{
    public SeparationInfo(Vector2 normal, double depth)
    {
        Normal = normal;
        Depth = depth;
    }

    public Vector2 Normal { get; }
    public double Depth { get; }

    public override string ToString()
    {
        return $"{nameof(Normal)}: {Normal}, {nameof(Depth)}: {Depth}";
    }
}