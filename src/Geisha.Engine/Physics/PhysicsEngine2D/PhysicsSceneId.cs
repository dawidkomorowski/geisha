using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct PhysicsSceneId : IUnmanaged<PhysicsSceneId>
{
    private readonly int _value;

    public static PhysicsSceneId Null => default;

    public PhysicsSceneId(int index, int version)
    {
        _value = index + 1;
        Version = version;
    }

    public int Index => _value - 1;
    public int Version { get; }

    public bool IsNull => !IsNotNull;
    public bool IsNotNull => _value > 0;
}