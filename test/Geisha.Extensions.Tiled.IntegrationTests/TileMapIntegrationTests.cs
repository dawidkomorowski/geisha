using NUnit.Framework;

namespace Geisha.Extensions.Tiled.IntegrationTests;

public class TileMapIntegrationTests
{
    [Test]
    public void LoadFromFile()
    {
        // Arrange
        const string filePath = "path/to/tilemap.tmx"; // Replace with the actual path to your TMX file

        // Act
        var tileMap = TileMap.LoadFromFile(filePath);

        // Assert
        Assert.IsNotNull(tileMap, "TileMap should not be null after loading from file.");
    }
}