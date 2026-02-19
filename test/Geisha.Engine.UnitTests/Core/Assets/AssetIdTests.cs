using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets;

[TestFixture]
public class AssetIdTests
{
    [Test]
    public void CreateUnique_ShouldCreateUniqueAssetIds()
    {
        // Arrange
        // Act
        var assetId1 = AssetId.CreateUnique();
        var assetId2 = AssetId.CreateUnique();

        // Assert
        Assert.That(assetId1, Is.Not.EqualTo(assetId2));
    }

    [Test]
    public void Constructor_ShouldCreateAssetId_GivenSpecifiedGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var actual = new AssetId(guid);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(guid));
    }
}