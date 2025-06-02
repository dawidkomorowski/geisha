namespace Geisha.Engine.Core.Math;

// TODO Add XML documentation comments to this struct.
public readonly record struct Size
{
    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; init; }
    public int Height { get; init; }
    public static Size Empty => new(0, 0);
}