using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;
using System;
using Geisha.Engine.Physics.PhysicsEngine2D.Internal;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct RigidBody2D_V2 : IUnmanaged<RigidBody2D_V2>
{
    public RigidBody2D_V2(RigidBodyId id)
    {
        Id = id;
    }

    public RigidBodyId Id { get; }

    public Vector2 Position
    {
        get => throw new System.NotImplementedException();
        set => Physics2D.Body.SetPosition(Id, value);
    }

    public double Rotation
    {
        get => throw new System.NotImplementedException();
        set => Physics2D.Body.SetRotation(Id, value);
    }

    public Vector2 LinearVelocity
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public double AngularVelocity
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public bool EnableCollisionDetection
    {
        get => throw new System.NotImplementedException();
        set => Physics2D.Body.SetEnableCollisionDetection(Id, value);
    }

    public bool EnableCollisionResponse
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public bool IsSensor
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public uint CollisionLayer
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public uint CollisionMask
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public void SetCircleCollider(double radius)
    {
        throw new System.NotImplementedException();
    }

    public void SetRectangleCollider(in SizeD size)
    {
        throw new System.NotImplementedException();
    }

    public void SetTileCollider()
    {
        throw new System.NotImplementedException();
    }
}