using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

// TODO: How to approach span in struct in older .NET version?
internal readonly struct SensorEvents
{
    public Span<SensorOverlapBeginEvent> BeginEvents { get; }
}

internal readonly struct SensorOverlapBeginEvent
{
    public SensorOverlapBeginEvent(RigidBody2D sensor, RigidBody2D visitor)
    {
        Sensor = sensor;
        Visitor = visitor;
    }

    public RigidBody2D Sensor { get; }
    public RigidBody2D Visitor { get; }
}

internal readonly struct SensorOverlapEndEvent
{
    public SensorOverlapEndEvent(RigidBody2D sensor, RigidBody2D visitor)
    {
        Sensor = sensor;
        Visitor = visitor;
    }

    public RigidBody2D Sensor { get; }
    public RigidBody2D Visitor { get; }
}