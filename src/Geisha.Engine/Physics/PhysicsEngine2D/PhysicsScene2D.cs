using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class PhysicsScene2D
{
    private readonly List<RigidBody2D> _bodies = new();
    private readonly List<RigidBody2D> _staticBodies = new();
    private readonly List<RigidBody2D> _kinematicBodies = new();
    private readonly SensorOverlapCache _sensorOverlapCache = new();
    private readonly List<SensorOverlapBeginEvent> _sensorOverlapBeginEvents = new();
    private readonly List<SensorOverlapEndEvent> _sensorOverlapEndEvents = new();

    public PhysicsScene2D(SizeD tileSize)
    {
        TileSize = tileSize;
        TileMap = new TileMap(TileSize);
    }

    internal TileMap TileMap { get; }

    public int Substeps { get; set; } = 1;
    public int VelocityIterations { get; set; } = 4;
    public int PositionIterations { get; set; } = 4;
    public double PenetrationTolerance { get; set; } = 0.01;
    public SizeD TileSize { get; }

    public ReadOnlySpan<RigidBody2D> Bodies => CollectionsMarshal.AsSpan(_bodies);

    public RigidBody2D CreateBody(BodyType bodyType, double circleColliderRadius)
    {
        var body = new RigidBody2D(this, bodyType, circleColliderRadius);
        AddBodyToScene(body);
        return body;
    }

    public RigidBody2D CreateBody(BodyType bodyType, in SizeD rectangleColliderSize)
    {
        var body = new RigidBody2D(this, bodyType, rectangleColliderSize);
        AddBodyToScene(body);
        return body;
    }

    public RigidBody2D CreateTileBody()
    {
        var body = new RigidBody2D(this);
        AddBodyToScene(body);
        return body;
    }

    public void RemoveBody(RigidBody2D body)
    {
        if (body.ColliderType is ColliderType.Tile && body.EnableCollisionDetection)
        {
            TileMap.RemoveTile(body);
        }

        switch (body.Type)
        {
            case BodyType.Static:
                _bodies.Remove(body);
                _staticBodies.Remove(body);
                break;
            case BodyType.Kinematic:
                _bodies.Remove(body);
                _kinematicBodies.Remove(body);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Simulate(TimeSpan timeStep)
    {
        var deltaTimeSeconds = timeStep.TotalSeconds / Substeps;

        var staticBodies = GetStaticBodiesAsSpan();
        var kinematicBodies = GetKinematicBodiesAsSpan();

        ClearEvents();

        for (var substep = 0; substep < Substeps; substep++)
        {
            // TODO Consider adding minimum velocity threshold to avoid solving constraints for very small velocities.
            // TODO SolveVelocityConstraints could return a boolean value indicating whether the velocity constraints were solved. Then further iterations could be stopped.
            for (var i = 0; i < VelocityIterations; i++)
            {
                ContactSolver.SolveVelocityConstraints(kinematicBodies);
            }

            KinematicIntegration.IntegrateKinematicMotion(kinematicBodies, deltaTimeSeconds);

            foreach (var kinematicBody in kinematicBodies)
            {
                kinematicBody.RecomputeCollider();
            }

            CollisionDetection.DetectCollisions(staticBodies, kinematicBodies, _sensorOverlapCache);

            // TODO SolvePositionConstraints could return a boolean value indicating whether the position constraints were solved. Then further iterations could be stopped.
            for (var i = 0; i < PositionIterations; i++)
            {
                ContactSolver.SolvePositionConstraints(kinematicBodies, PenetrationTolerance);
            }

            GenerateEvents();
        }
    }

    public void QueryPoint<TQueryHandler>(in Vector2 point, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyQueryHandler
    {
        foreach (var body in Bodies)
        {
            if (body.ContainsPoint(point))
            {
                if (!handler.Handle(body))
                {
                    return;
                }
            }
        }
    }

    public void QueryBounds<TQueryHandler>(in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyQueryHandler
    {
        foreach (var body in Bodies)
        {
            if (body.BoundingRectangle.Overlaps(axisAlignedRectangle))
            {
                if (!handler.Handle(body))
                {
                    return;
                }
            }
        }
    }

    public void QueryOverlap<TQueryHandler>(in AxisAlignedRectangle axisAlignedRectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyQueryHandler
    {
        foreach (var body in Bodies)
        {
            if (body.Overlaps(axisAlignedRectangle))
            {
                if (!handler.Handle(body))
                {
                    return;
                }
            }
        }
    }

    public void QueryOverlap<TQueryHandler>(in Circle circle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyQueryHandler
    {
        foreach (var body in Bodies)
        {
            if (body.Overlaps(circle))
            {
                if (!handler.Handle(body))
                {
                    return;
                }
            }
        }
    }

    public void QueryOverlap<TQueryHandler>(in Rectangle rectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyQueryHandler
    {
        foreach (var body in Bodies)
        {
            if (body.Overlaps(rectangle))
            {
                if (!handler.Handle(body))
                {
                    return;
                }
            }
        }
    }

    public SensorEvents GetSensorEvents()
    {
        return new SensorEvents(CollectionsMarshal.AsSpan(_sensorOverlapBeginEvents), CollectionsMarshal.AsSpan(_sensorOverlapEndEvents));
    }

    private ReadOnlySpan<RigidBody2D> GetStaticBodiesAsSpan() => CollectionsMarshal.AsSpan(_staticBodies);
    private ReadOnlySpan<RigidBody2D> GetKinematicBodiesAsSpan() => CollectionsMarshal.AsSpan(_kinematicBodies);

    private void AddBodyToScene(RigidBody2D body)
    {
        switch (body.Type)
        {
            case BodyType.Static:
                _bodies.Add(body);
                _staticBodies.Add(body);
                break;
            case BodyType.Kinematic:
                _bodies.Add(body);
                _kinematicBodies.Add(body);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(body), "Unsupported body type.");
        }
    }

    private void ClearEvents()
    {
        _sensorOverlapBeginEvents.Clear();
        _sensorOverlapEndEvents.Clear();
    }

    private void GenerateEvents()
    {
        foreach (var sensorOverlap in _sensorOverlapCache.GetOverlaps())
        {
            RigidBody2D sensor;
            RigidBody2D visitor;

            if (sensorOverlap.Body1.IsSensor)
            {
                sensor = sensorOverlap.Body1;
                visitor = sensorOverlap.Body2;
            }
            else
            {
                sensor = sensorOverlap.Body2;
                visitor = sensorOverlap.Body1;
            }

            switch (sensorOverlap.CacheStatus)
            {
                case CacheStatus.New:
                    _sensorOverlapBeginEvents.Add(new SensorOverlapBeginEvent(sensor, visitor));
                    break;
                case CacheStatus.Updated:
                    break;
                case CacheStatus.Stale:
                    _sensorOverlapEndEvents.Add(new SensorOverlapEndEvent(sensor, visitor));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}