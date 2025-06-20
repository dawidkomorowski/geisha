using System.Diagnostics;
using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct Contact
{
    public Contact(
        RigidBody2D body1,
        RigidBody2D body2,
        in Vector2 collisionNormal,
        double penetrationDepth,
        in ReadOnlyFixedList2<ContactPoint> contactPoints
    )
    {
        Debug.Assert(GMath.AlmostEqual(collisionNormal.Length, 1d), "GMath.AlmostEqual(collisionNormal.Length, 1d)");

        Body1 = body1;
        Body2 = body2;
        CollisionNormal = collisionNormal;
        PenetrationDepth = penetrationDepth;
        ContactPoints = contactPoints;
    }

    public RigidBody2D Body1 { get; }
    public RigidBody2D Body2 { get; }

    /// <summary>
    ///     Gets the collision normal vector pointing from <see cref="Body2" /> to <see cref="Body1" />.
    /// </summary>
    public Vector2 CollisionNormal { get; }

    public double PenetrationDepth { get; }
    public ReadOnlyFixedList2<ContactPoint> ContactPoints { get; }
}