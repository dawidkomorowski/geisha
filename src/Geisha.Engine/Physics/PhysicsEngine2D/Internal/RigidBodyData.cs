using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct RigidBodyData : IUnmanaged<RigidBodyData>
{
    public RuntimeId RuntimeId;
    public RigidBodyId Id;

    public BodyType Type;
    public ColliderType ColliderType;
    public CollisionNormalFilter CollisionNormalFilter;


    public Vector2 Position;
    public double Rotation;
    public Vector2 LinearVelocity;
    public double AngularVelocity;

    public bool EnableCollisionDetection;
    public bool EnableCollisionResponse;

    public bool IsSensor;

    public uint CollisionLayer;
    public uint CollisionMask;

    public double CircleColliderRadius;
    public Circle TransformedCircleCollider;

    public SizeD RectangleColliderSize;
    public Rectangle TransformedRectangleCollider;

    public AxisAlignedRectangle BoundingRectangle;

    public bool ContainsPoint(in Vector2 point) =>
        ColliderType switch
        {
            ColliderType.Circle => TransformedCircleCollider.Contains(point),
            ColliderType.Rectangle => BoundingRectangle.Contains(point) && TransformedRectangleCollider.Contains(point),
            ColliderType.Tile => BoundingRectangle.Contains(point),
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Overlaps(in AxisAlignedRectangle axisAlignedRectangle) =>
        ColliderType switch
        {
            ColliderType.Circle => BoundingRectangle.Overlaps(axisAlignedRectangle) && TransformedCircleCollider.Overlaps(axisAlignedRectangle.ToRectangle()),
            ColliderType.Rectangle => BoundingRectangle.Overlaps(axisAlignedRectangle) &&
                                      TransformedRectangleCollider.Overlaps(axisAlignedRectangle.ToRectangle()),
            ColliderType.Tile => BoundingRectangle.Overlaps(axisAlignedRectangle),
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Overlaps(in Circle circle) =>
        ColliderType switch
        {
            ColliderType.Circle => TransformedCircleCollider.Overlaps(circle),
            ColliderType.Rectangle => BoundingRectangle.Overlaps(circle.GetBoundingRectangle()) && TransformedRectangleCollider.Overlaps(circle),
            ColliderType.Tile => BoundingRectangle.Overlaps(circle.GetBoundingRectangle()) && TransformedRectangleCollider.Overlaps(circle),
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Overlaps(in Rectangle rectangle) =>
        ColliderType switch
        {
            ColliderType.Circle => BoundingRectangle.Overlaps(rectangle.GetBoundingRectangle()) && TransformedCircleCollider.Overlaps(rectangle),
            ColliderType.Rectangle => BoundingRectangle.Overlaps(rectangle.GetBoundingRectangle()) && TransformedRectangleCollider.Overlaps(rectangle),
            ColliderType.Tile => BoundingRectangle.Overlaps(rectangle.GetBoundingRectangle()) && TransformedRectangleCollider.Overlaps(rectangle),
            _ => throw new ArgumentOutOfRangeException()
        };

    internal void RecomputeCollider()
    {
        var transform = new Transform2D(Position, Rotation, Vector2.One);

        switch (ColliderType)
        {
            case ColliderType.Circle:
                TransformedCircleCollider = new Circle(CircleColliderRadius).Transform(transform.ToMatrix());
                BoundingRectangle = TransformedCircleCollider.GetBoundingRectangle();
                break;
            case ColliderType.Rectangle:
            case ColliderType.Tile:
                TransformedRectangleCollider = new Rectangle(RectangleColliderSize).Transform(transform.ToMatrix());
                BoundingRectangle = TransformedRectangleCollider.GetBoundingRectangle();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}