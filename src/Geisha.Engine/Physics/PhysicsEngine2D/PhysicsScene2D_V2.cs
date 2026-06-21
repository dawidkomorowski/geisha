using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;
using Geisha.Engine.Physics.PhysicsEngine2D.Internal;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct PhysicsScene2DDefinition
{
    public SizeD TileSize { get; init; }
    public int Substeps { get; init; }
    public int VelocityIterations { get; init; }
    public int PositionIterations { get; init; }
    public double PenetrationTolerance { get; init; }
}

internal readonly record struct PhysicsScene2D_V2 : IUnmanaged<PhysicsScene2D_V2>, IDisposable
{
    public static PhysicsScene2D_V2 Create(in PhysicsScene2DDefinition sceneDefinition)
    {
        var id = Physics2D.Scene.Create(sceneDefinition);
        return new PhysicsScene2D_V2(id);
    }

    public void Dispose()
    {
        // TODO: Does it make sense to implement IDisposable? Or maybe some other type of API?
        throw new NotImplementedException();
    }

    private PhysicsScene2D_V2(PhysicsSceneId id)
    {
        Id = id;
        // TODO: Construct it on the fly to avoid storing?
        Bodies = new BodiesView(id);
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

    public BodiesView Bodies { get; }

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

    public void DestroyBody(RigidBody2D_V2 body)
    {
        if (Id != body.Id.PhysicsSceneId)
        {
            throw new ArgumentException("Body does not belong to this scene.");
        }

        Physics2D.Scene.DestroyBody(body.Id);
    }

    public void Simulate(TimeSpan timeStep) => Physics2D.Scene.Simulate(Id, timeStep);

    public void QueryPoint<TQueryHandler>(in Vector2 point, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler => Physics2D.Scene.QueryPoint(Id, in point, ref handler);

    public void QueryBounds<TQueryHandler>(in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler => Physics2D.Scene.QueryBounds(Id, in axisAlignedRectangle, ref handler);

    public void QueryOverlap<TQueryHandler>(in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler => Physics2D.Scene.QueryOverlap(Id, in axisAlignedRectangle, ref handler);

    public void QueryOverlap<TQueryHandler>(in Circle circle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler => Physics2D.Scene.QueryOverlap(Id, in circle, ref handler);

    public void QueryOverlap<TQueryHandler>(in Rectangle rectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler => Physics2D.Scene.QueryOverlap(Id, in rectangle, ref handler);

    public ReadOnlySpan<SensorOverlapEvent> GetSensorOverlapEvents() => Physics2D.Scene.GetSensorOverlapEvents(Id);

    public readonly struct BodiesView
    {
        public BodiesView(PhysicsSceneId physicsSceneId)
        {
            PhysicsSceneId = physicsSceneId;
        }

        public PhysicsSceneId PhysicsSceneId { get; }

        public int Count => Physics2D.Scene.GetBodyCount(PhysicsSceneId);

        public RigidBody2D_V2 this[int index] => new(Physics2D.Scene.GetBodyByRawIndex(PhysicsSceneId, index));
    }
}