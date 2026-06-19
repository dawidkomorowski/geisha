using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;
using Geisha.Engine.Physics.PhysicsEngine2D.Internal;
using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct PhysicsScene2D_V2 : IUnmanaged<PhysicsScene2D_V2>, IDisposable
{
    public static PhysicsScene2D_V2 Create()
    {
        var id = Physics2D.Scene.Create();
        return new PhysicsScene2D_V2(id);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private PhysicsScene2D_V2(PhysicsSceneId id)
    {
        Id = id;
    }

    public PhysicsSceneId Id { get; }

    public int Substeps
    {
        get => throw new NotImplementedException();
        set => Physics2D.Scene.SetSubsteps(Id, value);
    }

    public int VelocityIterations
    {
        get => throw new NotImplementedException();
        set => Physics2D.Scene.SetVelocityIterations(Id, value);
    }

    public int PositionIterations
    {
        get => throw new NotImplementedException();
        set => Physics2D.Scene.SetPositionIterations(Id, value);
    }

    public double PenetrationTolerance
    {
        get => throw new NotImplementedException();
        set => Physics2D.Scene.SetPenetrationTolerance(Id, value);
    }

    public RigidBody2D_V2 CreateBody(BodyType bodyType, double circleColliderRadius)
    {
        var id = Physics2D.Scene.CreateBody(Id, bodyType, circleColliderRadius);
        return new RigidBody2D_V2(id);
    }

    public RigidBody2D_V2 CreateBody(BodyType bodyType, in SizeD rectangleColliderSize)
    {
        var id = Physics2D.Scene.CreateBody(Id, bodyType, rectangleColliderSize);
        return new RigidBody2D_V2(id);
    }

    public RigidBody2D_V2 CreateTileBody()
    {
        var id = Physics2D.Scene.CreateTileBody(Id);
        return new RigidBody2D_V2(id);
    }

    public void Simulate(TimeSpan timeStep) => Physics2D.Scene.Simulate(Id, timeStep);
}