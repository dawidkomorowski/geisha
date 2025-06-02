namespace Geisha.Engine.Core.Math;

// TODO Add XML documentation comments to this struct.
public readonly record struct SizeD
{
    public SizeD(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Width { get; init; }
    public double Height { get; init; }
    public static SizeD Empty => new(0.0, 0.0);
}