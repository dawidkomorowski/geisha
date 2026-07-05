using System.Diagnostics;
using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct Contact
{
    public Contact(RigidBodyId body1Id, RigidBodyId body2Id, in ContactManifold contactManifold)
    {
        Body1Id = body1Id;
        Body2Id = body2Id;
        ContactManifold = contactManifold;
    }

    public RigidBodyId Body1Id { get; }
    public RigidBodyId Body2Id { get; }
    public ContactManifold ContactManifold { get; }
}

internal readonly struct ContactManifold
{
    public ContactManifold(in Vector2 collisionNormal, double penetrationDepth, in ReadOnlyFixedList2<ContactPoint> contactPoints)
    {
        Debug.Assert(MathEx.AlmostEqual(collisionNormal.Length, 1d), "Collision normal must be a unit vector");

        CollisionNormal = collisionNormal;
        PenetrationDepth = penetrationDepth;
        ContactPoints = contactPoints;
    }

    /// <summary>
    ///     Gets the collision normal vector pointing from body 2 to body 1.
    /// </summary>
    public Vector2 CollisionNormal { get; }

    public double PenetrationDepth { get; }
    public ReadOnlyFixedList2<ContactPoint> ContactPoints { get; }
}

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