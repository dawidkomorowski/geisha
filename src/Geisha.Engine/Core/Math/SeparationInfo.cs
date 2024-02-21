namespace Geisha.Engine.Core.Math;

// TODO Add documentation;
// TODO Should Normal be from A to B or from B to A?
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