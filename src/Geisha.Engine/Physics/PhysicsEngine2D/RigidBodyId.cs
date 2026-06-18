namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct RigidBodyId
{
    private readonly int _value;

    public static RigidBodyId Invalid => default;

    public RigidBodyId(PhysicsSceneId physicsSceneId, int index, int version)
    {
        PhysicsSceneId = physicsSceneId;
        _value = index + 1;
        Version = version;
    }

    public PhysicsSceneId PhysicsSceneId { get; }
    public int Index => _value - 1;
    public int Version { get; }
    public bool IsValid => _value > 0;
}