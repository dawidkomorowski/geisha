using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class RigidBody2D
{
    // TODO This could be replaced with field keyword in .NET 10 (C# 14).
    private Vector2 _position;
    private double _rotation;
    private Vector2 _linearVelocity;
    private double _angularVelocity;
    private bool _enableCollisionDetection = true;

    public RigidBody2D(PhysicsScene2D scene)
    {
        Scene = scene;
        Type = BodyType.Static;
        SetTileCollider();
    }

    public RigidBody2D(PhysicsScene2D scene, BodyType type, double circleColliderRadius)
    {
        Scene = scene;
        Type = type;
        SetCircleCollider(circleColliderRadius);
    }

    public RigidBody2D(PhysicsScene2D scene, BodyType type, in SizeD rectangleColliderSize)
    {
        Scene = scene;
        Type = type;
        SetRectangleCollider(rectangleColliderSize);
    }

    public BodyType Type { get; }
    public ColliderType ColliderType { get; private set; }
    public CollisionNormalFilter CollisionNormalFilter { get; internal set; } = CollisionNormalFilter.None;

    public PhysicsScene2D Scene { get; }

    public Vector2 Position
    {
        get => _position;
        set
        {
            if (ColliderType is ColliderType.Tile)
            {
                _position = EnableCollisionDetection
                    ? Scene.TileMap.UpdateTile(this, _position, value)
                    : Scene.TileMap.AlignPosition(value);
            }
            else
            {
                _position = value;
            }
        }
    }

    public double Rotation
    {
        get => _rotation;
        set => _rotation = ColliderType is ColliderType.Tile ? 0 : value;
    }

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

    public bool EnableCollisionDetection
    {
        get => _enableCollisionDetection;
        set
        {
            if (_enableCollisionDetection != value && ColliderType is ColliderType.Tile)
            {
                if (value)
                {
                    Scene.TileMap.CreateTile(this);
                }
                else
                {
                    Scene.TileMap.RemoveTile(this);
                }
            }

            _enableCollisionDetection = value;
        }
    }

    public bool EnableCollisionResponse { get; set; }

    public double CircleColliderRadius { get; private set; }
    public Circle TransformedCircleCollider { get; private set; }

    public SizeD RectangleColliderSize { get; private set; }
    public Rectangle TransformedRectangleCollider { get; private set; }

    public AxisAlignedRectangle BoundingRectangle { get; private set; }

    public List<Contact> Contacts { get; } = new();

    public PhysicsBodyProxy? Proxy { get; set; }

    public void SetCircleCollider(double radius)
    {
        if (ColliderType is ColliderType.Tile && EnableCollisionDetection)
        {
            Scene.TileMap.RemoveTile(this);
        }

        ColliderType = ColliderType.Circle;
        CircleColliderRadius = radius;
        RectangleColliderSize = default;
        RecomputeCollider();
    }

    public void SetRectangleCollider(in SizeD size)
    {
        if (ColliderType is ColliderType.Tile && EnableCollisionDetection)
        {
            Scene.TileMap.RemoveTile(this);
        }

        ColliderType = ColliderType.Rectangle;
        CircleColliderRadius = 0;
        RectangleColliderSize = size;
        RecomputeCollider();
    }

    public void SetTileCollider()
    {
        if (Type is BodyType.Kinematic)
        {
            throw new InvalidOperationException("Kinematic body cannot be Tile collider.");
        }

        if (ColliderType is not ColliderType.Tile)
        {
            ColliderType = ColliderType.Tile;
            _position = Scene.TileMap.AlignPosition(_position);

            if (EnableCollisionDetection)
            {
                Scene.TileMap.CreateTile(this);
            }
        }

        CircleColliderRadius = 0;
        RectangleColliderSize = Scene.TileSize;
        RecomputeCollider();
    }

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