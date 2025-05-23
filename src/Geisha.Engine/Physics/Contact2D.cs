﻿using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics;

// TODO Add documentation.
public class Contact2D
{
    public Contact2D(
        Collider2DComponent thisCollider,
        Collider2DComponent otherCollider,
        in Vector2 collisionNormal,
        double penetrationDepth,
        in ReadOnlyFixedList2<ContactPoint2D> contactPoints
    )
    {
        ThisCollider = thisCollider;
        OtherCollider = otherCollider;
        CollisionNormal = collisionNormal;
        PenetrationDepth = penetrationDepth;
        ContactPoints = contactPoints;
    }

    public Collider2DComponent ThisCollider { get; }
    public Collider2DComponent OtherCollider { get; }
    public Vector2 CollisionNormal { get; } // TODO Is it from This to Other or from Other to This?
    public double PenetrationDepth { get; }
    public ReadOnlyFixedList2<ContactPoint2D> ContactPoints { get; }
}