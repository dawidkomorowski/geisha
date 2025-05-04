using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics;

// TODO Add documentation.
public readonly record struct ContactPoint2D
{
    public ContactPoint2D(in Vector2 worldPosition, in Vector2 thisLocalPosition, in Vector2 otherLocalPosition)
    {
        WorldPosition = worldPosition;
        ThisLocalPosition = thisLocalPosition;
        OtherLocalPosition = otherLocalPosition;
    }

    public Vector2 WorldPosition { get; }
    public Vector2 ThisLocalPosition { get; }
    public Vector2 OtherLocalPosition { get; }
}