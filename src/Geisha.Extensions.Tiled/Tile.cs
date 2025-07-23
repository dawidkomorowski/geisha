namespace Geisha.Extensions.Tiled;

public sealed class Tile
{
    internal Tile(GlobalTileId globalTileId)
    {
        GlobalTileId = globalTileId;
        LocalTileId = globalTileId.ClearFlippingFlags().Value - 1;
    }

    public GlobalTileId GlobalTileId { get; }
    public uint LocalTileId { get; }
}