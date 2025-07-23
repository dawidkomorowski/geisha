using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Geisha.Extensions.Tiled.IntegrationTests;

public class TileMapIntegrationTests
{
    [Test]
    public void LoadFromFile_Empty()
    {
        // Arrange
        var filePath = Path.Combine("Tiled", "TileMaps", "empty.tmx");

        // Act
        var tileMap = TileMap.LoadFromFile(filePath);

        // Assert

        // Assert map properties
        Assert.That(tileMap.Version, Is.EqualTo("1.10"));
        Assert.That(tileMap.TiledVersion, Is.EqualTo("1.11.2"));
        Assert.That(tileMap.Orientation, Is.EqualTo(Orientation.Orthogonal));
        Assert.That(tileMap.RenderOrder, Is.EqualTo(RenderOrder.RightDown));
        Assert.That(tileMap.Width, Is.EqualTo(30));
        Assert.That(tileMap.Height, Is.EqualTo(20));
        Assert.That(tileMap.TileWidth, Is.EqualTo(32));
        Assert.That(tileMap.TileHeight, Is.EqualTo(32));
        Assert.That(tileMap.IsInfinite, Is.False);

        // Assert tile sets
        Assert.That(tileMap.TileSets, Is.Empty);

        // Assert tile layers
        Assert.That(tileMap.TileLayers, Has.Count.EqualTo(1));
        var tileLayer = tileMap.TileLayers[0];

        Assert.That(tileLayer.Id, Is.EqualTo(1));
        Assert.That(tileLayer.Name, Is.EqualTo("Tile Layer 1"));
        Assert.That(tileLayer.Width, Is.EqualTo(30));
        Assert.That(tileLayer.Height, Is.EqualTo(20));

        for (var w = 0; w < tileLayer.Width; w++)
        {
            for (var h = 0; h < tileLayer.Height; h++)
            {
                Assert.That(tileLayer.Tiles[w][h], Is.Null);
            }
        }
    }

    [Test]
    public void LoadFromFile_Complex()
    {
        // Arrange
        var filePath = Path.Combine("Tiled", "TileMaps", "complex.tmx");

        // Act
        var tileMap = TileMap.LoadFromFile(filePath);

        // Assert

        // Assert map properties
        Assert.That(tileMap.Version, Is.EqualTo("1.10"));
        Assert.That(tileMap.TiledVersion, Is.EqualTo("1.11.2"));
        Assert.That(tileMap.Orientation, Is.EqualTo(Orientation.Orthogonal));
        Assert.That(tileMap.RenderOrder, Is.EqualTo(RenderOrder.RightDown));
        Assert.That(tileMap.Width, Is.EqualTo(20));
        Assert.That(tileMap.Height, Is.EqualTo(20));
        Assert.That(tileMap.TileWidth, Is.EqualTo(18));
        Assert.That(tileMap.TileHeight, Is.EqualTo(18));
        Assert.That(tileMap.IsInfinite, Is.False);

        // Assert tile sets
        Assert.That(tileMap.TileSets, Has.Count.EqualTo(2));

        var tileSet1 = tileMap.TileSets[0];
        Assert.That(tileSet1.FirstGlobalTileId.Value, Is.EqualTo(1));
        Assert.That(tileSet1.Source, Is.EqualTo("../TileSets/tiles.tsx"));
        Assert.That(tileSet1.Version, Is.EqualTo("1.10"));
        Assert.That(tileSet1.TiledVersion, Is.EqualTo("1.11.2"));
        Assert.That(tileSet1.Name, Is.EqualTo("tiles"));
        Assert.That(tileSet1.TileWidth, Is.EqualTo(18));
        Assert.That(tileSet1.TileHeight, Is.EqualTo(18));
        Assert.That(tileSet1.Spacing, Is.EqualTo(1));
        Assert.That(tileSet1.TileCount, Is.EqualTo(180));
        Assert.That(tileSet1.Columns, Is.EqualTo(20));

        var tileSet2 = tileMap.TileSets[1];
        Assert.That(tileSet2.FirstGlobalTileId.Value, Is.EqualTo(181));
        Assert.That(tileSet2.Source, Is.EqualTo("../TileSets/characters.tsx"));
        Assert.That(tileSet2.Version, Is.EqualTo("1.10"));
        Assert.That(tileSet2.TiledVersion, Is.EqualTo("1.11.2"));
        Assert.That(tileSet2.Name, Is.EqualTo("characters"));
        Assert.That(tileSet2.TileWidth, Is.EqualTo(24));
        Assert.That(tileSet2.TileHeight, Is.EqualTo(24));
        Assert.That(tileSet2.Spacing, Is.EqualTo(1));
        Assert.That(tileSet2.TileCount, Is.EqualTo(27));
        Assert.That(tileSet2.Columns, Is.EqualTo(9));

        // Assert tile layers
        Assert.That(tileMap.TileLayers, Has.Count.EqualTo(1));
        var tileLayer = tileMap.TileLayers[0];

        Assert.That(tileLayer.Id, Is.EqualTo(1));
        Assert.That(tileLayer.Name, Is.EqualTo("Tile Layer 1"));
        Assert.That(tileLayer.Width, Is.EqualTo(20));
        Assert.That(tileLayer.Height, Is.EqualTo(20));

        // Assert basic tiles
        Assert.That(tileLayer.Tiles[0][0], Is.Not.Null);
        Assert.That(tileLayer.Tiles[0][0].GlobalTileId.Value, Is.EqualTo(1));
        Assert.That(tileLayer.Tiles[0][0].LocalTileId, Is.EqualTo(0));

        Assert.That(tileLayer.Tiles[1][0], Is.Not.Null);
        Assert.That(tileLayer.Tiles[1][0].GlobalTileId.Value, Is.EqualTo(91));
        Assert.That(tileLayer.Tiles[1][0].LocalTileId, Is.EqualTo(90));

        Assert.That(tileLayer.Tiles[2][0], Is.Not.Null);
        Assert.That(tileLayer.Tiles[2][0].GlobalTileId.Value, Is.EqualTo(180));
        Assert.That(tileLayer.Tiles[2][0].LocalTileId, Is.EqualTo(179));

        // Assert flipping and rotation flags
        Assert.That(tileLayer.Tiles[0][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[0][1].GlobalTileId.Value, Is.EqualTo(128));
        Assert.That(tileLayer.Tiles[0][1].GlobalTileId.HasFlippingFlags, Is.False);
        Assert.That(tileLayer.Tiles[0][1].GlobalTileId.FlippedHorizontally, Is.False);
        Assert.That(tileLayer.Tiles[0][1].GlobalTileId.FlippedVertically, Is.False);
        Assert.That(tileLayer.Tiles[0][1].GlobalTileId.FlippedDiagonally, Is.False);
        Assert.That(tileLayer.Tiles[0][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[0][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[1][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[1][1].GlobalTileId.Value, Is.EqualTo(2147483776));
        Assert.That(tileLayer.Tiles[1][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[1][1].GlobalTileId.FlippedHorizontally, Is.True);
        Assert.That(tileLayer.Tiles[1][1].GlobalTileId.FlippedVertically, Is.False);
        Assert.That(tileLayer.Tiles[1][1].GlobalTileId.FlippedDiagonally, Is.False);
        Assert.That(tileLayer.Tiles[1][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[1][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[2][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[2][1].GlobalTileId.Value, Is.EqualTo(1073741952));
        Assert.That(tileLayer.Tiles[2][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[2][1].GlobalTileId.FlippedHorizontally, Is.False);
        Assert.That(tileLayer.Tiles[2][1].GlobalTileId.FlippedVertically, Is.True);
        Assert.That(tileLayer.Tiles[2][1].GlobalTileId.FlippedDiagonally, Is.False);
        Assert.That(tileLayer.Tiles[2][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[2][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[3][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[3][1].GlobalTileId.Value, Is.EqualTo(3221225600));
        Assert.That(tileLayer.Tiles[3][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[3][1].GlobalTileId.FlippedHorizontally, Is.True);
        Assert.That(tileLayer.Tiles[3][1].GlobalTileId.FlippedVertically, Is.True);
        Assert.That(tileLayer.Tiles[3][1].GlobalTileId.FlippedDiagonally, Is.False);
        Assert.That(tileLayer.Tiles[3][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[3][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[4][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[4][1].GlobalTileId.Value, Is.EqualTo(2684354688));
        Assert.That(tileLayer.Tiles[4][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[4][1].GlobalTileId.FlippedHorizontally, Is.True);
        Assert.That(tileLayer.Tiles[4][1].GlobalTileId.FlippedVertically, Is.False);
        Assert.That(tileLayer.Tiles[4][1].GlobalTileId.FlippedDiagonally, Is.True);
        Assert.That(tileLayer.Tiles[4][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[4][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[5][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[5][1].GlobalTileId.Value, Is.EqualTo(3758096512));
        Assert.That(tileLayer.Tiles[5][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[5][1].GlobalTileId.FlippedHorizontally, Is.True);
        Assert.That(tileLayer.Tiles[5][1].GlobalTileId.FlippedVertically, Is.True);
        Assert.That(tileLayer.Tiles[5][1].GlobalTileId.FlippedDiagonally, Is.True);
        Assert.That(tileLayer.Tiles[5][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[5][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[6][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[6][1].GlobalTileId.Value, Is.EqualTo(1610612864));
        Assert.That(tileLayer.Tiles[6][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[6][1].GlobalTileId.FlippedHorizontally, Is.False);
        Assert.That(tileLayer.Tiles[6][1].GlobalTileId.FlippedVertically, Is.True);
        Assert.That(tileLayer.Tiles[6][1].GlobalTileId.FlippedDiagonally, Is.True);
        Assert.That(tileLayer.Tiles[6][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[6][1].LocalTileId, Is.EqualTo(127));

        Assert.That(tileLayer.Tiles[7][1], Is.Not.Null);
        Assert.That(tileLayer.Tiles[7][1].GlobalTileId.Value, Is.EqualTo(536871040));
        Assert.That(tileLayer.Tiles[7][1].GlobalTileId.HasFlippingFlags, Is.True);
        Assert.That(tileLayer.Tiles[7][1].GlobalTileId.FlippedHorizontally, Is.False);
        Assert.That(tileLayer.Tiles[7][1].GlobalTileId.FlippedVertically, Is.False);
        Assert.That(tileLayer.Tiles[7][1].GlobalTileId.FlippedDiagonally, Is.True);
        Assert.That(tileLayer.Tiles[7][1].GlobalTileId.RotatedHexagonal120, Is.False);
        Assert.That(tileLayer.Tiles[7][1].LocalTileId, Is.EqualTo(127));

        // Assert tiles from second tile set
        Assert.That(tileLayer.Tiles[9][2], Is.Not.Null);
        Assert.That(tileLayer.Tiles[9][2].GlobalTileId.Value, Is.EqualTo(183));
        Assert.That(tileLayer.Tiles[9][2].GlobalTileId.HasFlippingFlags, Is.False);
        Assert.That(tileLayer.Tiles[9][2].LocalTileId, Is.EqualTo(2));

        Assert.That(tileLayer.Tiles[11][2], Is.Not.Null);
        Assert.That(tileLayer.Tiles[11][2].GlobalTileId.Value, Is.EqualTo(2147483831));
        Assert.That(tileLayer.Tiles[11][2].GlobalTileId.FlippedHorizontally, Is.True);
        Assert.That(tileLayer.Tiles[11][2].LocalTileId, Is.EqualTo(2));

        // Assert tile properties
        Assert.That(tileLayer.Tiles[0][0].Properties["Bool Property"].Name, Is.EqualTo("Bool Property"));
        Assert.That(tileLayer.Tiles[0][0].Properties["Bool Property"].Type, Is.EqualTo(PropertyType.Bool));
        Assert.That(tileLayer.Tiles[0][0].Properties["Bool Property"].Value, Is.EqualTo("true"));
        Assert.That(tileLayer.Tiles[0][0].Properties["Bool Property"].BoolValue, Is.True);

        Assert.That(tileLayer.Tiles[0][0].Properties["Float Property"].Name, Is.EqualTo("Float Property"));
        Assert.That(tileLayer.Tiles[0][0].Properties["Float Property"].Type, Is.EqualTo(PropertyType.Float));
        Assert.That(tileLayer.Tiles[0][0].Properties["Float Property"].Value, Is.EqualTo("3.14"));
        Assert.That(tileLayer.Tiles[0][0].Properties["Float Property"].FloatValue, Is.EqualTo(3.14));

        Assert.That(tileLayer.Tiles[0][0].Properties["Int Property"].Name, Is.EqualTo("Int Property"));
        Assert.That(tileLayer.Tiles[0][0].Properties["Int Property"].Type, Is.EqualTo(PropertyType.Int));
        Assert.That(tileLayer.Tiles[0][0].Properties["Int Property"].Value, Is.EqualTo("69"));
        Assert.That(tileLayer.Tiles[0][0].Properties["Int Property"].FloatValue, Is.EqualTo(69));

        Assert.That(tileLayer.Tiles[0][0].Properties["String Property"].Name, Is.EqualTo("String Property"));
        Assert.That(tileLayer.Tiles[0][0].Properties["String Property"].Type, Is.EqualTo(PropertyType.String));
        Assert.That(tileLayer.Tiles[0][0].Properties["String Property"].Value, Is.EqualTo("This is a string property"));
        Assert.That(tileLayer.Tiles[0][0].Properties["String Property"].StringValue, Is.EqualTo("This is a string property"));

        Assert.That(tileLayer.Tiles[2][0].Properties["Number"].Name, Is.EqualTo("Number"));
        Assert.That(tileLayer.Tiles[2][0].Properties["Number"].Type, Is.EqualTo(PropertyType.String));
        Assert.That(tileLayer.Tiles[2][0].Properties["Number"].Value, Is.EqualTo("Nine"));
        Assert.That(tileLayer.Tiles[2][0].Properties["Number"].StringValue, Is.EqualTo("Nine"));

        // Assert all remaining tiles are null
        var skipTiles = new HashSet<(int x, int y)>
        {
            (0, 0), (1, 0), (2, 0), (0, 1), (1, 1), (2, 1), (3, 1), (4, 1), (5, 1), (6, 1), (7, 1), (9, 2), (11, 2)
        };
        for (var w = 0; w < tileLayer.Width; w++)
        {
            for (var h = 0; h < tileLayer.Height; h++)
            {
                if (skipTiles.Contains((w, h)))
                {
                    Assert.That(tileLayer.Tiles[w][h], Is.Not.Null, $"Invalid tile at ({w},{h}).");
                }
                else
                {
                    Assert.That(tileLayer.Tiles[w][h], Is.Null, $"Invalid tile at ({w},{h}).");
                }
            }
        }
    }
}