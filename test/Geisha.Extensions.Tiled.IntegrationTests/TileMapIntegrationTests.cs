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
        Assert.That(tileMap.TileSets, Has.Count.EqualTo(1));
        var tileSet = tileMap.TileSets[0];

        Assert.That(tileSet.FirstGlobalTileId.Value, Is.EqualTo(1));
        Assert.That(tileSet.Source, Is.EqualTo("../TileSets/tiles.tsx"));
        Assert.That(tileSet.Version, Is.EqualTo("1.10"));
        Assert.That(tileSet.TiledVersion, Is.EqualTo("1.11.2"));
        Assert.That(tileSet.Name, Is.EqualTo("tiles"));
        Assert.That(tileSet.TileWidth, Is.EqualTo(18));
        Assert.That(tileSet.TileHeight, Is.EqualTo(18));
        Assert.That(tileSet.Spacing, Is.EqualTo(1));
        Assert.That(tileSet.TileCount, Is.EqualTo(180));
        Assert.That(tileSet.Columns, Is.EqualTo(20));

        // Assert tile layers
        Assert.That(tileMap.TileLayers, Has.Count.EqualTo(1));
        var tileLayer = tileMap.TileLayers[0];

        Assert.That(tileLayer.Id, Is.EqualTo(1));
        Assert.That(tileLayer.Name, Is.EqualTo("Tile Layer 1"));
        Assert.That(tileLayer.Width, Is.EqualTo(20));
        Assert.That(tileLayer.Height, Is.EqualTo(20));

        for (var w = 0; w < tileLayer.Width; w++)
        {
            for (var h = 0; h < tileLayer.Height; h++)
            {
                Assert.That(tileLayer.Tiles[w][h], Is.Null);
            }
        }
    }
}