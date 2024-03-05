using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class PhysicsScene2D
{
    private readonly List<RigidBody2D> _staticBodies = new();
    private readonly List<RigidBody2D> _kinematicBodies = new();

    public RigidBody2D CreateBody(BodyType bodyType)
    {
        var body = new RigidBody2D(this, bodyType);

        AddBodyToScene(body);

        return body;
    }

    public void RemoveBody(RigidBody2D body)
    {
        switch (body.Type)
        {
            case BodyType.Static:
                _staticBodies.Remove(body);
                break;
            case BodyType.Kinematic:
                _kinematicBodies.Remove(body);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Simulate(TimeSpan timeStep)
    {
        throw new NotImplementedException();
    }

    private void AddBodyToScene(RigidBody2D body)
    {
        switch (body.Type)
        {
            case BodyType.Static:
                _staticBodies.Add(body);
                break;
            case BodyType.Kinematic:
                _kinematicBodies.Add(body);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

internal enum BodyType
{
    Static,
    Kinematic
}

internal sealed class RigidBody2D
{
    private readonly PhysicsScene2D _scene;
    private BodyType _type;

    public RigidBody2D(PhysicsScene2D scene, BodyType type)
    {
        _scene = scene;
        _type = type;
    }

    public BodyType Type
    {
        get => _type;
        set => _type = value;
    }

    public Vector2 Position { get; set; }
    public double Rotation { get; set; }
    public Vector2 LinearVelocity { get; set; }
    public double AngularVelocity { get; set; }
}