namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct SensorOverlapEvent
{
    public SensorOverlapEvent(RigidBody2D sensor, RigidBody2D visitor, EventType type)
    {
        Sensor = sensor;
        Visitor = visitor;
        Type = type;
    }

    public RigidBody2D Sensor { get; }
    public RigidBody2D Visitor { get; }
    public EventType Type { get; }

    public enum EventType
    {
        Begin,
        End
    }
}