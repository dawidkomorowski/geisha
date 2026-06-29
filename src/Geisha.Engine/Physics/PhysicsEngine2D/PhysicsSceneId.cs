using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

// TODO: Maybe using int8 or int16 would be enough for internal values and would yield better performance?
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