namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly record struct PhysicsSceneId
{
    private readonly int _value;

    public static PhysicsSceneId Invalid => default;

    public PhysicsSceneId(int index, int version)
    {
        _value = index + 1;
        Version = version;
    }

    public int Index => _value - 1;
    public int Version { get; }
    public bool IsValid => _value > 0;
}