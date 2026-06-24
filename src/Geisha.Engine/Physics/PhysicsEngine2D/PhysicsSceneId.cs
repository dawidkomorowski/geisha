namespace Geisha.Engine.Physics.PhysicsEngine2D;

// TODO: Maybe using int8 or int16 would be enough for internal values and would yield better performance?
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

    // TODO: Rename to IsNull or similar and keep the Valid semantic for real validity check by querying physics database?
    public bool IsValid => _value > 0;
}