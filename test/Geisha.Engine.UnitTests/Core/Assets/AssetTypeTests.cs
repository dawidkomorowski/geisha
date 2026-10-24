using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets;

public class AssetTypeTests
{
    [Test]
    public void Constructor_CreatesAssetTypeWithEmptyString_GivenNoParameters()
    {
        // Arrange
        // Act
        var assetType = new AssetType();
        var actual = assetType.Value;

        // Assert
        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void Constructor_ThrowsException_GivenNull()
    {
        // Arrange
        // Act
        // Assert
        Assert.That(() => new AssetType(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_CreatesAssetTypeWithValue_GivenNonNullString()
    {
        // Arrange
        const string assetTypeString = "Some Asset Type";

        // Act
        var assetType = new AssetType(assetTypeString);
        var actual = assetType.Value;

        // Assert
        Assert.That(actual, Is.EqualTo(assetTypeString));
    }
}