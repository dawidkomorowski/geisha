using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly ref struct SensorEvents
{
    public SensorEvents(ReadOnlySpan<SensorOverlapBeginEvent> beginEvents, ReadOnlySpan<SensorOverlapEndEvent> endEvents)
    {
        BeginEvents = beginEvents;
        EndEvents = endEvents;
    }

    public ReadOnlySpan<SensorOverlapBeginEvent> BeginEvents { get; }
    public ReadOnlySpan<SensorOverlapEndEvent> EndEvents { get; }
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