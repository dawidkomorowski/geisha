using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class PhysicsScene2D
{
    private readonly List<RigidBody2D> _bodies = new();
    private readonly List<RigidBody2D> _staticBodies = new();
    private readonly List<RigidBody2D> _kinematicBodies = new();

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

    public void RemoveBody(RigidBody2D body)
    {
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
        KinematicIntegration.IntegrateKinematicMotion(_kinematicBodies, timeStep.TotalSeconds);

        foreach (var kinematicBody in _kinematicBodies)
        {
            kinematicBody.RecomputeCollider();
        }

        CollisionDetection.DetectCollisions(_staticBodies, _kinematicBodies);

        // TODO Constant of 6 is how many times position constraints are iteratively solved. It is arbitrary value.
        // TODO It may require further research to find optimal value. Also, it may require to be configurable.
        // TODO SolvePositionConstraints could return a boolean value indicating whether the position constraints were solved. Then further iterations could be stopped.
        // TODO Research it further when working on https://github.com/dawidkomorowski/geisha/issues/324.
        for (var i = 0; i < 6; i++)
        {
            ContactSolver.SolvePositionConstraints(_kinematicBodies);
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
                throw new ArgumentOutOfRangeException();
        }
    }
}