namespace Geisha.Extensions.Tiled;

public readonly record struct GlobalTileId
{
    public GlobalTileId(uint value)
    {
        Value = value;
    }

    public uint Value { get; }
}