namespace Geisha.Engine.Core.Math;

// TODO Add documentation;
public readonly struct SeparationInfo
{
    public SeparationInfo(Vector2 normal, double depth)
    {
        Normal = normal;
        Depth = depth;
    }

    public Vector2 Normal { get; }
    public double Depth { get; }
}