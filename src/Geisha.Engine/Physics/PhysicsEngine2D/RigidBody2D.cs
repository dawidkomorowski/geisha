﻿using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

// TODO Complex setters and getters impact performance in tight loops.
// Probably some intermediate data structure should be used to store position, rotation, linear and angular velocities and then
// apply them to RigidBody2D in a single step.
internal sealed class RigidBody2D
{
    // TODO This could be replaced with field keyword in .NET 10 (C# 14).
    private Vector2 _position;
    private double _rotation;
    private Vector2 _linearVelocity;
    private double _angularVelocity;

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
        set => _position = ColliderType is ColliderType.Tile ? Scene.TileMap.UpdateTile(this, _position, value) : value;
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
        ColliderType = ColliderType.Circle;
        CircleColliderRadius = radius;
        RectangleColliderSize = default;
        RecomputeCollider();
    }

    public void SetRectangleCollider(in SizeD size)
    {
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
            Scene.TileMap.CreateTile(this);
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