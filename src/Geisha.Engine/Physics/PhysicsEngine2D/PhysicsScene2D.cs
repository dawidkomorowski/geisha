using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class PhysicsScene2D
{
    public RigidBody2D CreateBody(BodyType bodyType)
    {
        throw new NotImplementedException();
    }

    public void RemoveBody(RigidBody2D body)
    {
        throw new NotImplementedException();
    }

    public void Simulate(TimeSpan timeStep)
    {
        throw new NotImplementedException();
    }
}

internal enum BodyType
{
    Static,
    Kinematic
}

internal sealed class RigidBody2D
{
}