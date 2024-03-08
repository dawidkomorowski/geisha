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
        KinematicIntegrator.IntegrateKinematicMotion(_kinematicBodies, timeStep.TotalSeconds);
        CollisionDetection.DetectCollisions(_staticBodies, _kinematicBodies);
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