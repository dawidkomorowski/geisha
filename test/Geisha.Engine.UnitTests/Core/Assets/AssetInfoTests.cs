using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets;

[TestFixture]
public class AssetInfoTests
{
    [Test]
    public void Constructor_ShouldCreateDefaultAssetInfo_GivenNoParameters()
    {
        // Arrange
        // Act
        var actual = new AssetInfo();

        // Assert
        Assert.That(actual.AssetId, Is.EqualTo(default(AssetId)));
        Assert.That(actual.AssetType, Is.EqualTo(default(AssetType)));
        Assert.That(actual.AssetFilePath, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldThrowArgumentNullException_GivenNullAssetFilePath()
    {
        // Arrange
        // Act
        // Assert
        Assert.That(() => new AssetInfo(default, default, null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_ShouldCreateAssetInfo_GivenAllParameters()
    {
        // Arrange
        var assetId = AssetId.CreateUnique();
        var assetType = new AssetType("Asset Type");
        const string assetFilePath = "some file path";

        // Act
        var actual = new AssetInfo(assetId, assetType, assetFilePath);

        // Assert
        Assert.That(actual.AssetId, Is.EqualTo(assetId));
        Assert.That(actual.AssetType, Is.EqualTo(assetType));
        Assert.That(actual.AssetFilePath, Is.EqualTo(assetFilePath));
    }
}