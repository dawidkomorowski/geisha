using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class PhysicsScene2D
{
    private readonly List<RigidBody2D> _bodies = new();
    private readonly List<RigidBody2D> _staticBodies = new();
    private readonly List<RigidBody2D> _kinematicBodies = new();

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

    public IReadOnlyList<RigidBody2D> Bodies => _bodies;

    public RigidBody2D CreateBody(BodyType bodyType, Circle circleCollider)
    {
        var body = new RigidBody2D(this, bodyType, circleCollider);
        AddBodyToScene(body);
        return body;
    }

    public RigidBody2D CreateBody(BodyType bodyType, AxisAlignedRectangle rectangleCollider)
    {
        var body = new RigidBody2D(this, bodyType, rectangleCollider);
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
        if (body.ColliderType is ColliderType.Tile)
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

        for (var substep = 0; substep < Substeps; substep++)
        {
            // TODO Consider adding minimum velocity threshold to avoid solving constraints for very small velocities.
            // TODO SolveVelocityConstraints could return a boolean value indicating whether the velocity constraints were solved. Then further iterations could be stopped.
            for (var i = 0; i < VelocityIterations; i++)
            {
                ContactSolver.SolveVelocityConstraints(_kinematicBodies);
            }

            KinematicIntegration.IntegrateKinematicMotion(_kinematicBodies, deltaTimeSeconds);

            foreach (var kinematicBody in _kinematicBodies)
            {
                kinematicBody.RecomputeCollider();
            }

            CollisionDetection.DetectCollisions(_staticBodies, _kinematicBodies);

            // TODO SolvePositionConstraints could return a boolean value indicating whether the position constraints were solved. Then further iterations could be stopped.
            for (var i = 0; i < PositionIterations; i++)
            {
                ContactSolver.SolvePositionConstraints(_kinematicBodies, PenetrationTolerance);
            }
        }
    }

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
}