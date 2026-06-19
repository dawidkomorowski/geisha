using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;
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
        get => throw new NotImplementedException();
        set => Physics2D.Body.SetPosition(Id, value);
    }

    public double Rotation
    {
        get => throw new NotImplementedException();
        set => Physics2D.Body.SetRotation(Id, value);
    }

    public Vector2 LinearVelocity
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public double AngularVelocity
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public bool EnableCollisionDetection
    {
        get => throw new NotImplementedException();
        set => Physics2D.Body.SetEnableCollisionDetection(Id, value);
    }

    public bool EnableCollisionResponse
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public bool IsSensor
    {
        get => throw new NotImplementedException();
        set => Physics2D.Body.SetIsSensor(Id, value);
    }

    public uint CollisionLayer
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public uint CollisionMask
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public void SetCircleCollider(double radius)
    {
        throw new NotImplementedException();
    }

    public void SetRectangleCollider(in SizeD size)
    {
        throw new NotImplementedException();
    }

    public void SetTileCollider()
    {
        throw new NotImplementedException();
    }
}