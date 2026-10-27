using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets;

public class AssetTypeTests
{
    [Test]
    public void DefaultValue_IsInvalid()
    {
        // Arrange
        // Act
        var assetType1 = new AssetType();
        var assetType2 = default(AssetType);

        // Assert
        Assert.That(assetType1.IsValid, Is.False);
        Assert.That(assetType2.IsValid, Is.False);
    }

    [Test]
    public void DefaultValue_Equality()
    {
        // Arrange
        var assetType1 = new AssetType();
        var assetType2 = default(AssetType);

        // Act
        var actual = assetType1.Equals(assetType2);

        // Assert
        Assert.That(actual, Is.True);
    }

    [Test]
    public void DefaultValue_ShouldNotEqualNonDefault()
    {
        // Arrange
        var assetType1 = default(AssetType);
        var assetType2 = new AssetType(string.Empty);

        // Act
        var actual = assetType1.Equals(assetType2);

        // Assert
        Assert.That(actual, Is.False);
    }

    [Test]
    public void Constructor_ThrowsException_GivenNull()
    {
        // Arrange
        // Act
        // Assert
        Assert.That(() => new AssetType(null!), Throws.ArgumentNullException);
    }

    [TestCase("")]
    [TestCase("Test.Asset.Type")]
    public void Constructor_CreatesAssetTypeWithValue_GivenNonNullString(string assetTypeValue)
    {
        // Arrange
        // Act
        var assetType = new AssetType(assetTypeValue);

        // Assert
        Assert.That(assetType.IsValid, Is.True);
        Assert.That(assetType.Value, Is.EqualTo(assetTypeValue));
    }

    [Test]
    public void Value_ThrowsException_WhenAssetTypeIsInvalid()
    {
        // Arrange
        var assetType1 = new AssetType();
        var assetType2 = default(AssetType);

        // Act
        // Assert
        Assert.That(() => assetType1.Value, Throws.InvalidOperationException);
        Assert.That(() => assetType2.Value, Throws.InvalidOperationException);
    }

    [TestCase(null, "AssetType { IsValid = False, Value =  }")]
    [TestCase("Test.Asset.Type", "AssetType { IsValid = True, Value = Test.Asset.Type }")]
    public void ToString_Test(string? value, string expected)
    {
        // Arrange
        var assetType = value is null ? default : new AssetType(value);

        // Act
        var actual = assetType.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}