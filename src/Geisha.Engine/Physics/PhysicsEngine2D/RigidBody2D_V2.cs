using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;
using Geisha.Engine.Physics.PhysicsEngine2D.Internal;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct RigidBody2D_V2(RigidBodyId Id) : IUnmanaged<RigidBody2D_V2>
{
    public Vector2 Position
    {
        get => Physics2D.Body.GetPosition(Id);
        set => Physics2D.Body.SetPosition(Id, value);
    }

    public double Rotation
    {
        get => Physics2D.Body.GetRotation(Id);
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
        set => Physics2D.Body.SetCollisionLayer(Id, value);
    }

    public uint CollisionMask
    {
        get => throw new NotImplementedException();
        set => Physics2D.Body.SetCollisionMask(Id, value);
    }

    public void SetCircleCollider(double radius) => Physics2D.Body.SetCircleCollider(Id, radius);
    public void SetRectangleCollider(in SizeD size) => Physics2D.Body.SetRectangleCollider(Id, size);
    public void SetTileCollider() => Physics2D.Body.SetTileCollider(Id);
}