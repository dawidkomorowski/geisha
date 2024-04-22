using System.Diagnostics;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents vector of minimum translation of an object, that separates this object from the other overlapping one.
///     This is mainly used for collision resolution.
/// </summary>
public readonly struct MinimumTranslationVector
{
    /// <summary>
    ///     Creates new instance of <see cref="MinimumTranslationVector" /> with specified <paramref name="direction" /> and
    ///     <paramref name="length" />.
    /// </summary>
    /// <param name="direction">Direction of <see cref="MinimumTranslationVector" />.</param>
    /// <param name="length">Length of <see cref="MinimumTranslationVector" />.</param>
    public MinimumTranslationVector(Vector2 direction, double length)
    {
        Debug.Assert(GMath.AlmostEqual(direction.Length, 1d), "GMath.AlmostEqual(direction.Length, 1d)");

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
    ///     Gets a unit vector representing direction of <see cref="MinimumTranslationVector" />.
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