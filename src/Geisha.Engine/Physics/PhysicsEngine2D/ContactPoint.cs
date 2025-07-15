using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct ContactPoint
{
    public ContactPoint(in Vector2 worldPosition, in Vector2 localPosition1, in Vector2 localPosition2)
    {
        WorldPosition = worldPosition;
        LocalPosition1 = localPosition1;
        LocalPosition2 = localPosition2;
    }

    public Vector2 WorldPosition { get; }

    /// <summary>
    ///     Position of the contact point relative to the first body's position.
    /// </summary>
    public Vector2 LocalPosition1 { get; }

    /// <summary>
    ///     Position of the contact point relative to the second body's position.
    /// </summary>
    public Vector2 LocalPosition2 { get; }
}