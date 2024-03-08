using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

// TODO Should it be public? Add documentation?
public readonly struct ContactPoint
{
    public ContactPoint(in Vector2 worldPosition, in Vector2 localPositionA, in Vector2 localPositionB, in Vector2 collisionNormal, double separationDepth)
    {
        WorldPosition = worldPosition;
        LocalPositionA = localPositionA;
        LocalPositionB = localPositionB;
        CollisionNormal = collisionNormal;
        SeparationDepth = separationDepth;
    }

    public Vector2 WorldPosition { get; }
    public Vector2 LocalPositionA { get; }
    public Vector2 LocalPositionB { get; }
    public Vector2 CollisionNormal { get; }
    public double SeparationDepth { get; }
}