using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;
using Geisha.Engine.Core.Spatial;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// TODO: This is a big struct. It could be separated into hot/cold structs that are optimized for specific stages in physics pipeline.
//       For example, solver does not need many of those fields so it could utilize smaller part of the body optimized for solver.
internal struct RigidBodyData : IUnmanaged<RigidBodyData>
{
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

    // TODO: Dual collision shape representation grows the struct. It may be beneficial to collapse storage for shape and reuse the same memory depending on collider shape.
    public double CircleColliderRadius;
    public Circle TransformedCircleCollider;

    public SizeD RectangleColliderSize;
    public Rectangle TransformedRectangleCollider;

    // ReSharper disable once InconsistentNaming
    public AABB2D AABB;

    // Contacts
    public int ContactCount;
    public int FirstContactIndex;
    public int LastContactIndex;

    // Broad phase
    public SpatialGridProxyId SpatialProxyId;
    public AABB2D BroadPhaseAABB;

    public bool ContainsPoint(in Vector2 point) =>
        ColliderType switch
        {
            ColliderType.Circle => TransformedCircleCollider.Contains(point),
            ColliderType.Rectangle => AABB.Contains(point) && TransformedRectangleCollider.Contains(point),
            ColliderType.Tile => AABB.Contains(point),
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Overlaps(in AABB2D aabb) =>
        ColliderType switch
        {
            ColliderType.Circle => AABB.Overlaps(aabb) && TransformedCircleCollider.Overlaps(aabb.ToRectangle()),
            ColliderType.Rectangle => AABB.Overlaps(aabb) && TransformedRectangleCollider.Overlaps(aabb.ToRectangle()),
            ColliderType.Tile => AABB.Overlaps(aabb),
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Overlaps(in Circle circle) =>
        ColliderType switch
        {
            ColliderType.Circle => TransformedCircleCollider.Overlaps(circle),
            ColliderType.Rectangle => AABB.Overlaps(circle.ComputeAABB()) && TransformedRectangleCollider.Overlaps(circle),
            ColliderType.Tile => AABB.Overlaps(circle.ComputeAABB()) && TransformedRectangleCollider.Overlaps(circle),
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Overlaps(in Rectangle rectangle) =>
        ColliderType switch
        {
            ColliderType.Circle => AABB.Overlaps(rectangle.ComputeAABB()) && TransformedCircleCollider.Overlaps(rectangle),
            ColliderType.Rectangle => AABB.Overlaps(rectangle.ComputeAABB()) && TransformedRectangleCollider.Overlaps(rectangle),
            ColliderType.Tile => AABB.Overlaps(rectangle.ComputeAABB()) && TransformedRectangleCollider.Overlaps(rectangle),
            _ => throw new ArgumentOutOfRangeException()
        };

    internal void RecomputeCollider(ref PhysicsSceneData scene)
    {
        var transform = Matrix3x3.CreateTRS(Position, Rotation, Vector2.One);

        switch (ColliderType)
        {
            case ColliderType.Circle:
                TransformedCircleCollider = new Circle(CircleColliderRadius).Transform(transform);
                AABB = TransformedCircleCollider.ComputeAABB();
                break;
            case ColliderType.Rectangle:
            case ColliderType.Tile:
                TransformedRectangleCollider = new Rectangle(RectangleColliderSize).Transform(transform);
                AABB = TransformedRectangleCollider.ComputeAABB();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Update broad phase proxy.
        switch (Type)
        {
            case BodyType.Static:
                BroadPhaseAABB = AABB;
                scene.StaticGrid.MoveProxy(SpatialProxyId, BroadPhaseAABB);
                break;
            case BodyType.Kinematic:
                if (!BroadPhaseAABB.Contains(AABB))
                {
                    // Use twice as big AABB for broad phase to update it less often for moving bodies.
                    BroadPhaseAABB = AABB2D.FromCenterAndSize(AABB.Center, AABB.Size * 2);
                    scene.DynamicGrid.MoveProxy(SpatialProxyId, BroadPhaseAABB);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}