using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct Contact
{
    public Contact(
        RigidBody2D body1,
        RigidBody2D body2,
        in Vector2 collisionNormal,
        double separationDepth,
        in ReadOnlyFixedList2<ContactPoint> contactPoints
    )
    {
        Body1 = body1;
        Body2 = body2;
        CollisionNormal = collisionNormal;
        SeparationDepth = separationDepth;
        ContactPoints = contactPoints;
    }

    public RigidBody2D Body1 { get; }
    public RigidBody2D Body2 { get; }
    public Vector2 CollisionNormal { get; } // TODO Is it from This to Other or from Other to This?
    public double SeparationDepth { get; } // TODO SeparationDepth or Separation or something else?
    public ReadOnlyFixedList2<ContactPoint> ContactPoints { get; }
}