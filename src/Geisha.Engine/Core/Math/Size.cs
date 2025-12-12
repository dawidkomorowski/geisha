namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents the dimensions of a 2D object, defined by integer values for its width and height.
/// </summary>
/// <seealso cref="SizeD" />
public readonly record struct Size
{
    /// <summary>
    ///     Gets a <see cref="Size" /> struct that has both width and height set to zero.
    /// </summary>
    public static Size Empty => new(0, 0);

    /// <summary>
    ///     Initializes a new instance of the <see cref="Size" /> struct with the specified dimensions.
    /// </summary>
    /// <param name="width">The width component of the size.</param>
    /// <param name="height">The height component of the size.</param>
    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Gets the width component of this <see cref="Size" />.
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    ///     Gets the height component of this <see cref="Size" />.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    ///     Converts the <see cref="Size" /> instance to a <see cref="SizeD" /> structure.
    /// </summary>
    /// <returns>A <see cref="SizeD" /> representing the width and height of this instance.</returns>
    public SizeD ToSizeD() => new(Width, Height);

    /// <summary>
    ///     Converts the <see cref="Size" /> instance to a <see cref="Vector2" /> structure with the width and height as its
    ///     components.
    /// </summary>
    /// <returns>
    ///     A <see cref="Vector2" /> that has <see cref="Vector2.X" /> and <see cref="Vector2.Y" /> components set to the
    ///     <see cref="Width" /> and <see cref="Height" /> values, respectively.
    /// </returns>
    public Vector2 ToVector2() => new(Width, Height);
}