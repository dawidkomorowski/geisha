using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets;

[TestFixture]
public class AssetIdTests
{
    [Test]
    public void Empty_ShouldHaveValueEqualGuidEmpty()
    {
        // Arrange
        // Act
        var actual = AssetId.Empty;

        // Assert
        Assert.That(actual.Value, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void Empty_ShouldBeEqualToDefaultAssetId()
    {
        // Arrange
        // Act
        var actual = AssetId.Empty;

        // Assert
        Assert.That(actual, Is.EqualTo(default(AssetId)));
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
    public void Parse_ShouldCreateAssetId_GivenValidGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var assetIdString = guid.ToString();

        // Act
        var actual = AssetId.Parse(assetIdString);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(guid));
    }

    [Test]
    public void Parse_ShouldThrowArgumentNullException_GivenNull()
    {
        // Arrange
        // Act
        // Assert
        Assert.That(() => AssetId.Parse(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Parse_ShouldThrowFormatException_GivenInvalidString()
    {
        // Arrange
        // Act
        // Assert
        Assert.That(() => AssetId.Parse("invalid"), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void TryParse_ShouldReturnTrueAndCreateAssetId_GivenValidGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var assetIdString = guid.ToString();

        // Act
        var success = AssetId.TryParse(assetIdString, out var actual);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(actual.Value, Is.EqualTo(guid));
    }

    [Test]
    public void TryParse_ShouldReturnFalseAndDefaultValue_GivenInvalidString()
    {
        // Arrange
        // Act
        var success = AssetId.TryParse("invalid", out var actual);

        // Assert
        Assert.That(success, Is.False);
        Assert.That(actual, Is.EqualTo(default(AssetId)));
    }

    [Test]
    public void TryParse_ShouldReturnFalseAndDefaultValue_GivenNull()
    {
        // Arrange
        // Act
        var success = AssetId.TryParse(null, out var actual);

        // Assert
        Assert.That(success, Is.False);
        Assert.That(actual, Is.EqualTo(default(AssetId)));
    }
}