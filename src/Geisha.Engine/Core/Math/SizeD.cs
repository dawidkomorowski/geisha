namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents the dimensions of a 2D object, defined by double-precision floating-point values for its width and
///     height.
/// </summary>
/// <seealso cref="Size" />
public readonly record struct SizeD
{
    /// <summary>
    ///     Gets a <see cref="SizeD" /> struct that has both width and height set to zero.
    /// </summary>
    public static SizeD Empty => new(0.0, 0.0);

    /// <summary>
    ///     Initializes a new instance of the <see cref="SizeD" /> struct with the specified dimensions.
    /// </summary>
    /// <param name="width">The width component of the size.</param>
    /// <param name="height">The height component of the size.</param>
    public SizeD(double width, double height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Gets the width component of this <see cref="SizeD" />.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    ///     Gets the height component of this <see cref="SizeD" />.
    /// </summary>
    public double Height { get; init; }
}