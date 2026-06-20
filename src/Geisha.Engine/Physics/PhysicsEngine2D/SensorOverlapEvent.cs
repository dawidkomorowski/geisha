namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct SensorOverlapEvent
{
    public SensorOverlapEvent(RigidBodyId sensorId, RigidBodyId visitorId, EventType type)
    {
        SensorId = sensorId;
        VisitorId = visitorId;
        Type = type;
    }

    public RigidBodyId SensorId { get; }
    public RigidBodyId VisitorId { get; }
    public EventType Type { get; }

    public enum EventType
    {
        Begin,
        End
    }
}