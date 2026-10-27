namespace Geisha.Engine.Rendering;

/// <summary>
///     Represents font size encapsulating particular value together with unit.
/// </summary>
public readonly record struct FontSize
{
    /// <summary>
    ///     Font size in points unit.
    /// </summary>
    public double Points { get; }

    /// <summary>
    ///     Font size in device-independent pixels unit.
    /// </summary>
    public double Dips => Points / 72.0d * 96.0d; // TODO Is this calculation correct? Shouldn't DPI be taken from OS?

    private FontSize(double points)
    {
        Points = points;
    }

    /// <summary>
    ///     Creates new instance of <see cref="FontSize" /> with size given in points.
    /// </summary>
    /// <param name="points">Size of the font in points.</param>
    /// <returns><see cref="FontSize" /> instance with specified size.</returns>
    public static FontSize FromPoints(double points) => new(points);

    /// <summary>
    ///     Creates new instance of <see cref="FontSize" /> with size given in device-independent pixels.
    /// </summary>
    /// <param name="dips">Size of the font in device-independent pixels.</param>
    /// <returns><see cref="FontSize" /> instance with specified size.</returns>
    public static FontSize FromDips(double dips) => new(dips / 96.0d * 72.0d); // TODO Is this calculation correct? Shouldn't DPI be taken from OS?
}