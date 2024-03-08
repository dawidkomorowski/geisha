using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct Contact
{
    public Contact(RigidBody2D body1, RigidBody2D body2, in ContactPoint point) : this()
    {
        Body1 = body1;
        Body2 = body2;
        Point1 = point;
        Point2 = default;
        PointsCount = 1;
    }

    public Contact(RigidBody2D body1, RigidBody2D body2, in ContactPoint point1, in ContactPoint point2)
    {
        Body1 = body1;
        Body2 = body2;
        Point1 = point1;
        Point2 = point2;
        PointsCount = 2;
    }

    public RigidBody2D Body1 { get; }
    public RigidBody2D Body2 { get; }
    public ContactPoint Point1 { get; }
    public ContactPoint Point2 { get; }
    public int PointsCount { get; }

    public ContactPoint PointAt(int index) => index switch
    {
        0 => Point1,
        1 => Point2,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Valid range is [0,1].")
    };
}