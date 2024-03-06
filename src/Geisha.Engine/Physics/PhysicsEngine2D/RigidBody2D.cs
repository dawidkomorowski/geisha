using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class RigidBody2D
{
    private BodyType _type;

    public RigidBody2D(PhysicsScene2D scene, BodyType type, Circle circleCollider)
    {
        Scene = scene;
        _type = type;
        CircleCollider = circleCollider;
        IsCircleCollider = true;
        RecomputeCollider();
    }

    public RigidBody2D(PhysicsScene2D scene, BodyType type, AxisAlignedRectangle rectangleCollider)
    {
        Scene = scene;
        _type = type;
        RectangleCollider = rectangleCollider;
        IsRectangleCollider = true;
        RecomputeCollider();
    }

    public BodyType Type
    {
        get => _type;
        set => _type = value;
    }

    public PhysicsScene2D Scene { get; }

    public Vector2 Position { get; set; }
    public double Rotation { get; set; }
    public Vector2 LinearVelocity { get; set; }
    public double AngularVelocity { get; set; }

    public bool IsCircleCollider { get; }
    public Circle CircleCollider { get; private set; }
    public Circle TransformedCircleCollider { get; private set; }

    public bool IsRectangleCollider { get; }
    public AxisAlignedRectangle RectangleCollider { get; private set; }
    public Rectangle TransformedRectangleCollider { get; private set; }

    public AxisAlignedRectangle BoundingRectangle { get; private set; }

    // TODO Should be public in its current form?
    public List<Contact> Contacts { get; } = new();

    private void RecomputeCollider()
    {
        var transform = new Transform2D(Position, Rotation, Vector2.One);

        if (IsCircleCollider)
        {
            TransformedCircleCollider = CircleCollider.Transform(transform.ToMatrix());
            BoundingRectangle = TransformedCircleCollider.GetBoundingRectangle();
        }
        else if (IsRectangleCollider)
        {
            TransformedRectangleCollider = RectangleCollider.ToRectangle().Transform(transform.ToMatrix());
            BoundingRectangle = TransformedRectangleCollider.GetBoundingRectangle();
        }
    }
}