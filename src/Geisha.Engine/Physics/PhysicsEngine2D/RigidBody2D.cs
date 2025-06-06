using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class RigidBody2D
{
    private Vector2 _linearVelocity;
    private double _angularVelocity;

    public RigidBody2D(PhysicsScene2D scene)
    {
        Scene = scene;
        Type = BodyType.Static;
        SetTileCollider();
    }

    public RigidBody2D(PhysicsScene2D scene, BodyType type, Circle circleCollider)
    {
        Scene = scene;
        Type = type;
        SetCollider(circleCollider);
    }

    public RigidBody2D(PhysicsScene2D scene, BodyType type, AxisAlignedRectangle rectangleCollider)
    {
        Scene = scene;
        Type = type;
        SetCollider(rectangleCollider);
    }

    // TODO Should it allow to change the type?
    // TODO If body type is changed it should update internal data structures of PhysicsEngine2D.
    public BodyType Type { get; }
    public ColliderType ColliderType { get; private set; }

    public PhysicsScene2D Scene { get; }

    public Vector2 Position { get; set; }
    public double Rotation { get; set; }

    public Vector2 LinearVelocity
    {
        get => _linearVelocity;
        set
        {
            if (Type == BodyType.Static)
            {
                return;
            }

            _linearVelocity = value;
        }
    }

    public double AngularVelocity
    {
        get => _angularVelocity;
        set
        {
            if (Type == BodyType.Static)
            {
                return;
            }

            _angularVelocity = value;
        }
    }

    public bool EnableCollisionResponse { get; set; }

    public Circle CircleCollider { get; private set; }
    public Circle TransformedCircleCollider { get; private set; }

    public AxisAlignedRectangle RectangleCollider { get; private set; }
    public Rectangle TransformedRectangleCollider { get; private set; }

    public AxisAlignedRectangle BoundingRectangle { get; private set; }

    // TODO Should be public in its current form?
    public List<Contact> Contacts { get; } = new();

    public object? CustomData { get; set; }

    public void SetCollider(Circle circleCollider)
    {
        ColliderType = ColliderType.Circle;
        CircleCollider = circleCollider;
        RectangleCollider = default;
        RecomputeCollider();
    }

    public void SetCollider(AxisAlignedRectangle rectangleCollider)
    {
        ColliderType = ColliderType.Rectangle;
        CircleCollider = default;
        RectangleCollider = rectangleCollider;
        RecomputeCollider();
    }

    public void SetTileCollider()
    {
        ColliderType = ColliderType.Tile;
        CircleCollider = default;
        RectangleCollider = default;
        RecomputeCollider();
    }

    internal void RecomputeCollider()
    {
        var transform = new Transform2D(Position, Rotation, Vector2.One);

        switch (ColliderType)
        {
            case ColliderType.Circle:
                TransformedCircleCollider = CircleCollider.Transform(transform.ToMatrix());
                BoundingRectangle = TransformedCircleCollider.GetBoundingRectangle();
                break;
            case ColliderType.Rectangle:
                TransformedRectangleCollider = RectangleCollider.ToRectangle().Transform(transform.ToMatrix());
                BoundingRectangle = TransformedRectangleCollider.GetBoundingRectangle();
                break;
            case ColliderType.Tile:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}