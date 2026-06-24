using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct SensorOverlapEvent : IUnmanaged<SensorOverlapEvent>
{
    public SensorOverlapEvent(RigidBodyId body1Id, RigidBodyId body2Id, EventType type)
    {
        Body1Id = body1Id;
        Body2Id = body2Id;
        Type = type;
    }

    public RigidBodyId Body1Id { get; }
    public RigidBodyId Body2Id { get; }
    public EventType Type { get; }

    public enum EventType
    {
        Begin,
        End
    }
}