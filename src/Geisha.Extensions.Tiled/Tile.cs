namespace Geisha.Extensions.Tiled;

public sealed class Tile
{
    internal Tile(TileMap tileMap, GlobalTileId globalTileId)
    {
        TileMap = tileMap;
        GlobalTileId = globalTileId;
        LocalTileId = ComputeLocalTileId();
    }

    public GlobalTileId GlobalTileId { get; }
    public uint LocalTileId { get; }
    public TileMap TileMap { get; }

    private uint ComputeLocalTileId()
    {
        var gid = GlobalTileId.ClearFlippingFlags().Value;
        for (var i = TileMap.TileSets.Count - 1; i >= 0; --i)
        {
            var tileSet = TileMap.TileSets[i];
            if (gid >= tileSet.FirstGlobalTileId.Value)
            {
                return gid - tileSet.FirstGlobalTileId.Value;
            }
        }

        throw new InvalidTiledMapException(
            $"Global tile ID {GlobalTileId.Value} does not belong to any tile set in the tile map '{TileMap}'." +
            " Ensure that the tile map is correctly defined and that the global tile ID is valid.");
    }
}