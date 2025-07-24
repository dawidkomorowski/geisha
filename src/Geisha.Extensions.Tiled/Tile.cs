namespace Geisha.Extensions.Tiled;

public sealed class Tile
{
    private readonly TileSet.Tile _sourceTile;

    internal Tile(TileMap tileMap, GlobalTileId globalTileId)
    {
        TileMap = tileMap;
        GlobalTileId = globalTileId;
        LocalTileId = ComputeLocalTileId(out var tileSet);
        TileSet = tileSet;
        _sourceTile = tileSet.GetTile(LocalTileId);
    }

    public GlobalTileId GlobalTileId { get; }
    public uint LocalTileId { get; }
    public string Type => _sourceTile.Type;
    public Properties Properties => _sourceTile.Properties;
    public TileSet TileSet { get; }
    public TileMap TileMap { get; }

    private uint ComputeLocalTileId(out TileSet matchingTileSet)
    {
        var gid = GlobalTileId.ClearFlippingFlags().Value;
        for (var i = TileMap.TileSets.Count - 1; i >= 0; --i)
        {
            var tileSet = TileMap.TileSets[i];
            if (gid >= tileSet.FirstGlobalTileId.Value)
            {
                matchingTileSet = tileSet;
                return gid - tileSet.FirstGlobalTileId.Value;
            }
        }

        throw new InvalidTiledMapException(
            $"Global tile ID {GlobalTileId.Value} does not belong to any tile set in the tile map '{TileMap}'." +
            " Ensure that the tile map is correctly defined and that the global tile ID is valid.");
    }
}